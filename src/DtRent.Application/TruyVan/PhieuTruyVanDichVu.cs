using DtRent.Application.Exceptions;
using DtRent.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DtRent.Application.TruyVan;

public sealed record KhachLuaChon(long Id, string MaKhach, string HoTen, string SoDienThoai, string TenHang);

public sealed record NhanVienLuaChon(int Id, string MaNhanVien, string HoTen);

public sealed record PhieuTomTat(
    long Id,
    string MaPhieu,
    string TrangThai,
    string HoTenKhach,
    string BienSo,
    string TenXe,
    DateTimeOffset ThoiDiemNhan,
    DateTimeOffset ThoiDiemHenTra);

public sealed record TraXeThongTin(
    DateTimeOffset ThoiDiemTraThucTe,
    int SoKmTra,
    int KmDaDi,
    int SoGioTre,
    int KmVuot,
    decimal TienThue,
    decimal PhiVuotKm,
    decimal PhiTre,
    decimal TongTruocGiam,
    decimal TienGiam,
    decimal TongPhaiTra,
    bool XeHuHong,
    decimal? TinhTrangPinTra,
    string? GhiChu);

public sealed record PhieuChiTiet(
    long Id,
    string MaPhieu,
    string TrangThai,
    string MaKhach,
    string HoTenKhach,
    string SoDienThoaiKhach,
    string TenHangKhach,
    decimal PhanTramGiam,
    int XeId,
    string TenXe,
    string BienSo,
    string HoTenNhanVien,
    DateTimeOffset ThoiDiemNhan,
    DateTimeOffset ThoiDiemHenTra,
    int SoNgayThue,
    int SoKmNhan,
    decimal DonGiaNgay,
    int? KmToiDaMoiNgay,
    decimal? PhiVuotKm,
    decimal PhiTreGio,
    decimal TienCoc,
    string? GhiChu,
    decimal? TinhTrangPinNhan,
    TraXeThongTin? TraXe);
/// <summary>Truy vấn chỉ-đọc phục vụ form lập phiếu, danh sách và chi tiết phiếu thuê.</summary>
public sealed class PhieuTruyVanDichVu
{
    private static readonly TimeSpan MuiGioVietNam = TimeSpan.FromHours(7);

    private readonly DtRentDbContext _db;

    public PhieuTruyVanDichVu(DtRentDbContext db) => _db = db;

    private static DateTimeOffset SangGioVietNam(DateTime utc) =>
        new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc)).ToOffset(MuiGioVietNam);

    public async Task<IReadOnlyList<KhachLuaChon>> LayKhachHangAsync(CancellationToken ct = default) =>
        await _db.KhachHangs.AsNoTracking()
            .OrderBy(k => k.HoTen)
            .Select(k => new KhachLuaChon(k.Id, k.MaKhach, k.HoTen, k.SoDienThoai, k.HangThanhVien.Ten))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<NhanVienLuaChon>> LayNhanVienAsync(CancellationToken ct = default) =>
        await _db.NhanViens.AsNoTracking()
            .Where(n => n.DangHoatDong)
            .OrderBy(n => n.HoTen)
            .Select(n => new NhanVienLuaChon(n.Id, n.MaNhanVien, n.HoTen))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<PhieuTomTat>> LayDanhSachAsync(CancellationToken ct = default)
    {
        var dong = await _db.PhieuThues.AsNoTracking()
            .OrderByDescending(p => p.NgayTao)
            .Take(200)
            .Select(p => new
            {
                p.Id,
                p.MaPhieu,
                p.TrangThai,
                HoTenKhach = p.KhachHang.HoTen,
                p.Xe.BienSo,
                p.Xe.HangSanXuat,
                p.Xe.DongXe,
                p.Xe.NamSanXuat,
                p.ThoiDiemNhan,
                p.ThoiDiemHenTra
            })
            .ToListAsync(ct);

        return dong.Select(d => new PhieuTomTat(
            d.Id,
            d.MaPhieu,
            d.TrangThai,
            d.HoTenKhach,
            d.BienSo,
            $"{d.HangSanXuat} {d.DongXe} ({d.NamSanXuat})",
            SangGioVietNam(d.ThoiDiemNhan),
            SangGioVietNam(d.ThoiDiemHenTra))).ToList();
    }

    public async Task<PhieuChiTiet> LayChiTietAsync(long id, CancellationToken ct = default)
    {
        var p = await _db.PhieuThues.AsNoTracking()
            .Include(x => x.KhachHang).ThenInclude(k => k.HangThanhVien)
            .Include(x => x.Xe)
            .Include(x => x.NhanVien)
            .Include(x => x.TraXe)
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KhongTimThayException("phiếu thuê", id);

        var t = p.TraXe;
        TraXeThongTin? traXe = t is null
            ? null
            : new TraXeThongTin(
                SangGioVietNam(t.ThoiDiemTraThucTe), t.SoKmTra, t.KmDaDi, t.SoGioTre, t.KmVuot,
                t.TienThue, t.PhiVuotKm, t.PhiTre, t.TongTruocGiam, t.TienGiam, t.TongPhaiTra,
                t.XeHuHong, t.TinhTrangPinTra, t.GhiChu);

        return new PhieuChiTiet(
            p.Id, p.MaPhieu, p.TrangThai,
            p.KhachHang.MaKhach, p.KhachHang.HoTen, p.KhachHang.SoDienThoai,
            p.KhachHang.HangThanhVien.Ten, p.PhanTramGiam,
            p.XeId, $"{p.Xe.HangSanXuat} {p.Xe.DongXe} ({p.Xe.NamSanXuat})", p.Xe.BienSo,
            p.NhanVien.HoTen,
            SangGioVietNam(p.ThoiDiemNhan), SangGioVietNam(p.ThoiDiemHenTra),
            p.SoNgayThue, p.SoKmNhan,
            p.DonGiaNgay, p.KmToiDaMoiNgay, p.PhiVuotKm, p.PhiTreGio, p.TienCoc,
            p.GhiChu,
            p.TinhTrangPinNhan,
            traXe);
    }
}