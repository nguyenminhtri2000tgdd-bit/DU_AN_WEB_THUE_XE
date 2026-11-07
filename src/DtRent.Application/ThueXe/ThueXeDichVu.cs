using System.Globalization;
using DtRent.Application.Exceptions;
using DtRent.Domain.Enums;
using DtRent.Domain.Exceptions;
using DtRent.Domain.Models;
using DtRent.Domain.Pricing;
using DtRent.Domain.Services;
using DtRent.Infrastructure.Persistence;
using DtRent.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DtRent.Application.ThueXe;

public sealed class ThueXeDichVu
{
    private static readonly TimeSpan MuiGioVietNam = TimeSpan.FromHours(7);

    private readonly DtRentDbContext _db;

    public ThueXeDichVu(DtRentDbContext db) => _db = db;

    // ------------------------------------------------------------------
    // LẬP PHIẾU THUÊ
    // ------------------------------------------------------------------
    public async Task<LapPhieuThueKetQua> LapPhieuThueAsync(
        LapPhieuThueYeuCau yeuCau, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(yeuCau);
        if (yeuCau.SoNgayThue < 1 || yeuCau.SoNgayThue > 365)
            throw new ThoiGianThueKhongHopLeException("Số ngày thuê phải từ 1 đến 365.");

        var thoiDiemNhan = yeuCau.ThoiDiemNhan.ToOffset(MuiGioVietNam);
        var thoiDiemHenTra = thoiDiemNhan.AddDays(yeuCau.SoNgayThue);

        await KiemTraNhanVienAsync(yeuCau.NhanVienId, ct);

        var khachEntity = await _db.KhachHangs
            .Include(k => k.HangThanhVien)
            .Include(k => k.GplxKhachHangs)
            .SingleOrDefaultAsync(k => k.Id == yeuCau.KhachHangId, ct)
            ?? throw new KhongTimThayException("khách hàng", yeuCau.KhachHangId);

        var xeEntity = await _db.Xes
            .Include(x => x.NhomXe).ThenInclude(n => n.HangGplxMas)
            .SingleOrDefaultAsync(x => x.Id == yeuCau.XeId, ct)
            ?? throw new KhongTimThayException("xe", yeuCau.XeId);

        // Quy tắc nghiệp vụ nằm ở Domain: trạng thái xe, GPLX phù hợp và còn hạn, thời gian hợp lệ.
        var khach = khachEntity.ToDomain();
        var xe = xeEntity.ToDomain();
        KiemTraDieuKienThue.KiemTra(khach, xe, thoiDiemNhan, thoiDiemHenTra);
        xe.BatDauChoThue();

        var nhom = xeEntity.NhomXe;
        var phieu = new PhieuThue
        {
            MaPhieu = TaoMaPhieu(thoiDiemNhan),
            KhachHangId = khachEntity.Id,
            XeId = xeEntity.Id,
            NhanVienId = yeuCau.NhanVienId,
            ThoiDiemNhan = thoiDiemNhan.UtcDateTime,
            ThoiDiemHenTra = thoiDiemHenTra.UtcDateTime,
            SoNgayThue = (short)yeuCau.SoNgayThue,
            SoKmNhan = xeEntity.SoKmHienTai,
            TinhTrangPinNhan = xeEntity.TinhTrangPin,
            // Bản chụp giá và giảm giá tại thời điểm lập phiếu
            DonGiaNgay = nhom.GiaThueNgay,
            KmToiDaMoiNgay = nhom.KmToiDaMoiNgay,
            PhiVuotKm = nhom.PhiVuotKm,
            PhiTreGio = nhom.PhiTreGio,
            TienCoc = nhom.TienCoc,
            PhanTramGiam = khachEntity.HangThanhVien.PhanTramGiam,
            TrangThai = "DANG_THUE",
            GhiChu = yeuCau.GhiChu
        };

        xeEntity.TrangThai = xe.TrangThai.MaCsdl();
        _db.PhieuThues.Add(phieu);

        try
        {
            // Một lần SaveChanges = một giao dịch: thêm phiếu và đổi trạng thái xe cùng thành công hoặc cùng thất bại.
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ExclusionViolation })
        {
            throw new XeTrungLichException(xeEntity.BienSo);
        }

        return new LapPhieuThueKetQua(phieu.Id, phieu.MaPhieu, thoiDiemHenTra, phieu.TienCoc);
    }

    // ------------------------------------------------------------------
    // TRẢ XE VÀ TÍNH TIỀN
    // ------------------------------------------------------------------
    public async Task<TraXeKetQua> TraXeAsync(TraXeYeuCau yeuCau, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(yeuCau);

        await KiemTraNhanVienAsync(yeuCau.NhanVienId, ct);

        var phieu = await _db.PhieuThues
            .Include(p => p.KhachHang).ThenInclude(k => k.HangThanhVien)
            .Include(p => p.Xe).ThenInclude(x => x.NhomXe).ThenInclude(n => n.HangGplxMas)
            .SingleOrDefaultAsync(p => p.Id == yeuCau.PhieuThueId, ct)
            ?? throw new KhongTimThayException("phiếu thuê", yeuCau.PhieuThueId);

        if (phieu.TrangThai != "DANG_THUE")
            throw new PhieuThueKhongConHieuLucException(phieu.MaPhieu, phieu.TrangThai);

        var thoiDiemNhan = new DateTimeOffset(DateTime.SpecifyKind(phieu.ThoiDiemNhan, DateTimeKind.Utc));
        var thoiDiemHenTra = new DateTimeOffset(DateTime.SpecifyKind(phieu.ThoiDiemHenTra, DateTimeKind.Utc));
        var thoiDiemTra = yeuCau.ThoiDiemTraThucTe;

        if (thoiDiemTra <= thoiDiemNhan)
            throw new ThoiGianThueKhongHopLeException("Thời điểm trả xe phải sau thời điểm nhận xe.");
        if (yeuCau.SoKmTra < phieu.SoKmNhan)
            throw new SoKmTraKhongHopLeException(phieu.SoKmNhan, yeuCau.SoKmTra);

        int kmDaDi = yeuCau.SoKmTra - phieu.SoKmNhan;
        var thoiGianTre = thoiDiemTra - thoiDiemHenTra;

        // Tính tiền theo GIÁ CHỤP trên phiếu, bằng chiến lược của nhóm xe.
        var bangGia = new BangGia(phieu.DonGiaNgay, phieu.KmToiDaMoiNgay, phieu.PhiVuotKm, phieu.PhiTreGio);
        var chinhSach = ChinhSachGiaFactory.Tao(phieu.Xe.NhomXe.ChinhSachGia);
        var chiTiet = chinhSach.Tinh(bangGia, phieu.SoNgayThue, kmDaDi, thoiGianTre);

        var hangApDung = new HangKhachHang(
            phieu.KhachHang.HangThanhVien.Ma,
            phieu.KhachHang.HangThanhVien.Ten,
            phieu.PhanTramGiam);
        var thanhToan = hangApDung.ApDungGiamGia(chiTiet);

        // Máy trạng thái của xe: Đang cho thuê -> Sẵn sàng (hoặc Đang bảo trì nếu hư hỏng).
        var xe = phieu.Xe.ToDomain();
        xe.NhanXeVe(yeuCau.XeHuHong);

        var traXe = new TraXe
        {
            PhieuThueId = phieu.Id,
            NhanVienId = yeuCau.NhanVienId,
            ThoiDiemTraThucTe = thoiDiemTra.UtcDateTime,
            SoKmTra = yeuCau.SoKmTra,
            KmDaDi = kmDaDi,
            SoGioTre = chiTiet.SoGioTre,
            KmVuot = chiTiet.KmVuot,
            TienThue = chiTiet.TienThue,
            PhiVuotKm = chiTiet.PhiVuotKm,
            PhiTre = chiTiet.PhiTre,
            TongTruocGiam = chiTiet.TongTruocGiam,
            TienGiam = thanhToan.TienGiam,
            TongPhaiTra = thanhToan.TongPhaiTra,
            XeHuHong = yeuCau.XeHuHong,
            TinhTrangPinTra = yeuCau.TinhTrangPinTra,
            GhiChu = yeuCau.GhiChu
        };

        phieu.TrangThai = "DA_TRA";
        phieu.Xe.TrangThai = xe.TrangThai.MaCsdl();
        phieu.Xe.SoKmHienTai = yeuCau.SoKmTra;
        if (yeuCau.TinhTrangPinTra is not null)
            phieu.Xe.TinhTrangPin = yeuCau.TinhTrangPinTra;

        _db.TraXes.Add(traXe);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Hai nhân viên cùng bấm "Trả xe" cho một phiếu: ràng buộc UNIQUE trên tra_xe.phieu_thue_id chặn lần thứ hai.
            throw new PhieuThueKhongConHieuLucException(phieu.MaPhieu, "DA_TRA");
        }

        return new TraXeKetQua(traXe.Id, phieu.MaPhieu, thanhToan);
    }

    // ------------------------------------------------------------------
    private async Task KiemTraNhanVienAsync(int nhanVienId, CancellationToken ct)
    {
        bool hopLe = await _db.NhanViens.AnyAsync(n => n.Id == nhanVienId && n.DangHoatDong, ct);
        if (!hopLe)
            throw new KhongTimThayException("nhân viên đang hoạt động", nhanVienId);
    }

    private static string TaoMaPhieu(DateTimeOffset thoiDiem)
    {
        string ngay = thoiDiem.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        string duoi = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"PT{ngay}-{duoi}";
    }
}