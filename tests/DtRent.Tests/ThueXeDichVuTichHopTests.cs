using DtRent.Application.Exceptions;
using DtRent.Application.ThueXe;
using DtRent.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DtRent.Tests;

public class ThueXeDichVuTichHopTests
{
    // Dùng ngày năm 2030 để không đụng dữ liệu thật; luôn hoàn tác nên không để lại gì.
    private static readonly DateTimeOffset Nhan = new(2030, 3, 1, 8, 0, 0, TimeSpan.FromHours(7));
    private const string NhanVien = "NV002";

    private static async Task<LapPhieuThueKetQua> LapPhieuAsync(
        PhienTichHop phien, string maKhach, string maXe, DateTimeOffset nhan, int soNgay) =>
        await phien.DichVu.LapPhieuThueAsync(new LapPhieuThueYeuCau(
            await phien.KhachIdAsync(maKhach),
            await phien.XeIdAsync(maXe),
            await phien.NhanVienIdAsync(NhanVien),
            nhan,
            soNgay));

    [Fact]
    public async Task LapPhieu_HopLe_TaoPhieuVaDoiTrangThaiXe()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE001", "SAN_SANG", 12000);

        var kq = await LapPhieuAsync(phien, "KH001", "XE001", Nhan, 2);

        Assert.StartsWith("PT20300301-", kq.MaPhieu);
        Assert.Equal(5_000_000m, kq.TienCoc);

        var phieu = await phien.Db.PhieuThues.AsNoTracking().SingleAsync(p => p.Id == kq.PhieuThueId);
        Assert.Equal("DANG_THUE", phieu.TrangThai);
        Assert.Equal(800_000m, phieu.DonGiaNgay);
        Assert.Equal(0m, phieu.PhanTramGiam);
        Assert.Equal(12000, phieu.SoKmNhan);
        Assert.Equal(Nhan.AddDays(2).UtcDateTime, phieu.ThoiDiemHenTra);

        var xe = await phien.Db.Xes.AsNoTracking().SingleAsync(x => x.MaXe == "XE001");
        Assert.Equal("DANG_CHO_THUE", xe.TrangThai);
    }

    [Fact]
    public async Task LapPhieu_KhachChiCoGplxMoTo_BiTuChoi_VaKhongGhiPhieu()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE001", "SAN_SANG", 12000);

        await Assert.ThrowsAsync<GplxKhongPhuHopException>(
            () => LapPhieuAsync(phien, "KH004", "XE001", Nhan, 2));

        var xeId = await phien.XeIdAsync("XE001");
        Assert.Equal(0, await phien.Db.PhieuThues.CountAsync(p => p.XeId == xeId));
        var xe = await phien.Db.Xes.AsNoTracking().SingleAsync(x => x.Id == xeId);
        Assert.Equal("SAN_SANG", xe.TrangThai);
    }

    [Fact]
    public async Task LapPhieu_XeDangBaoTri_BiTuChoi()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE003", "DANG_BAO_TRI", 8000);

        var loi = await Assert.ThrowsAsync<XeKhongSanSangException>(
            () => LapPhieuAsync(phien, "KH001", "XE003", Nhan, 1));
        Assert.Equal("XE_KHONG_SAN_SANG", loi.MaLoi);
    }

    [Fact]
    public async Task LapPhieu_KhachHangVang_LuuPhanTramGiamLenPhieu()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE002", "SAN_SANG", 5000);

        var kq = await LapPhieuAsync(phien, "KH003", "XE002", Nhan, 1);

        var phieu = await phien.Db.PhieuThues.AsNoTracking().SingleAsync(p => p.Id == kq.PhieuThueId);
        Assert.Equal(10m, phieu.PhanTramGiam);
    }

    [Fact]
    public async Task TraXe_Tre2Gio10Phut_TinhTienDung_VaXeVeSanSang()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE001", "SAN_SANG", 12000);
        var phieuKq = await LapPhieuAsync(phien, "KH001", "XE001", Nhan, 2);

        // 2 ngày x 800.000 + 3 giờ trễ x 80.000 = 1.840.000 (đi 300 km, chưa vượt hạn mức 400 km)
        var kq = await phien.DichVu.TraXeAsync(new TraXeYeuCau(
            phieuKq.PhieuThueId,
            await phien.NhanVienIdAsync(NhanVien),
            Nhan.AddDays(2).AddMinutes(130),
            SoKmTra: 12300));

        Assert.Equal(1_840_000m, kq.ThanhToan.TongPhaiTra);

        var traXe = await phien.Db.TraXes.AsNoTracking().SingleAsync(t => t.Id == kq.TraXeId);
        Assert.Equal(300, traXe.KmDaDi);
        Assert.Equal(3, traXe.SoGioTre);
        Assert.Equal(0, traXe.KmVuot);
        Assert.Equal(1_840_000m, traXe.TongPhaiTra);

        var xe = await phien.Db.Xes.AsNoTracking().SingleAsync(x => x.MaXe == "XE001");
        Assert.Equal("SAN_SANG", xe.TrangThai);
        Assert.Equal(12300, xe.SoKmHienTai);

        var phieu = await phien.Db.PhieuThues.AsNoTracking().SingleAsync(p => p.Id == phieuKq.PhieuThueId);
        Assert.Equal("DA_TRA", phieu.TrangThai);
    }

    [Fact]
    public async Task TraXe_KhachVang_GiamGia10Phan_Tram_VaDungGiaChup_DuGiaNhomXeDaDoi()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE002", "SAN_SANG", 5000);
        var phieuKq = await LapPhieuAsync(phien, "KH003", "XE002", Nhan, 2);

        // Đổi giá nhóm xe SAU khi đã lập phiếu: hợp đồng cũ vẫn phải tính theo giá chụp trên phiếu.
        await phien.Db.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE nhom_xe SET gia_thue_ngay = 999999 WHERE ma = {"OTO_DIEN_5CHO"}");
        phien.Db.ChangeTracker.Clear();

        var kq = await phien.DichVu.TraXeAsync(new TraXeYeuCau(
            phieuKq.PhieuThueId,
            await phien.NhanVienIdAsync(NhanVien),
            Nhan.AddDays(2).AddMinutes(130),
            SoKmTra: 5300));

        // (1.600.000 + 240.000) x 90% = 1.656.000
        Assert.Equal(1_840_000m, kq.ThanhToan.ChiTiet.TongTruocGiam);
        Assert.Equal(184_000m, kq.ThanhToan.TienGiam);
        Assert.Equal(1_656_000m, kq.ThanhToan.TongPhaiTra);
    }

    [Fact]
    public async Task TraXe_PhieuDaTra_KhongTraLanHai()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE001", "SAN_SANG", 12000);
        var phieuKq = await LapPhieuAsync(phien, "KH001", "XE001", Nhan, 1);
        var nhanVienId = await phien.NhanVienIdAsync(NhanVien);
        var yeuCau = new TraXeYeuCau(phieuKq.PhieuThueId, nhanVienId, Nhan.AddDays(1), SoKmTra: 12100);

        await phien.DichVu.TraXeAsync(yeuCau);

        await Assert.ThrowsAsync<PhieuThueKhongConHieuLucException>(
            () => phien.DichVu.TraXeAsync(yeuCau));
    }

    [Fact]
    public async Task TraXe_XeHuHong_ChuyenSangDangBaoTri()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE001", "SAN_SANG", 12000);
        var phieuKq = await LapPhieuAsync(phien, "KH001", "XE001", Nhan, 1);

        await phien.DichVu.TraXeAsync(new TraXeYeuCau(
            phieuKq.PhieuThueId,
            await phien.NhanVienIdAsync(NhanVien),
            Nhan.AddDays(1),
            SoKmTra: 12100,
            XeHuHong: true));

        var xe = await phien.Db.Xes.AsNoTracking().SingleAsync(x => x.MaXe == "XE001");
        Assert.Equal("DANG_BAO_TRI", xe.TrangThai);
    }

    [Fact]
    public async Task TraXe_SoKmTraNhoHonSoKmNhan_BiTuChoi_VaKhongGhiTraXe()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE001", "SAN_SANG", 12000);
        var phieuKq = await LapPhieuAsync(phien, "KH001", "XE001", Nhan, 1);

        await Assert.ThrowsAsync<SoKmTraKhongHopLeException>(
            () => phien.DichVu.TraXeAsync(new TraXeYeuCau(
                phieuKq.PhieuThueId,
                phien.NhanVienIdAsync(NhanVien).GetAwaiter().GetResult(),
                Nhan.AddDays(1),
                SoKmTra: 11000)));

        Assert.Equal(0, await phien.Db.TraXes.CountAsync(t => t.PhieuThueId == phieuKq.PhieuThueId));
    }

    [Fact]
    public async Task LapPhieu_TrungLich_CsdlChanVaDoiThanhXeTrungLichException()
    {
        await using var phien = await PhienTichHop.TaoAsync();
        await phien.DatXeAsync("XE001", "SAN_SANG", 12000);

        await LapPhieuAsync(phien, "KH001", "XE001", Nhan, 2);

        // Cố tình đặt lại xe thành "Sẵn sàng" bằng SQL để vượt qua kiểm tra trạng thái của Domain,
        // buộc ràng buộc EXCLUDE của PostgreSQL phải ra tay.
        await phien.Db.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE xe SET trang_thai = {"SAN_SANG"} WHERE ma_xe = {"XE001"}");
        phien.Db.ChangeTracker.Clear();

        // Phiếu thứ hai chồng lên khoảng thời gian của phiếu thứ nhất (bắt đầu sau 1 ngày, thuê 2 ngày).
        var loi = await Assert.ThrowsAsync<XeTrungLichException>(
            () => LapPhieuAsync(phien, "KH003", "XE001", Nhan.AddDays(1), 2));
        Assert.Equal("XE_TRUNG_LICH", loi.MaLoi);
    }
}