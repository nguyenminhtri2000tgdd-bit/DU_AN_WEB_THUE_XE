using DtRent.Application.Exceptions;
using DtRent.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DtRent.Application.TruyVan;

public sealed record NhomXeLoc(string Ma, string Ten);

public sealed record XeTomTat(
    int Id,
    string BienSo,
    string HangSanXuat,
    string DongXe,
    short NamSanXuat,
    string MaNhom,
    string TenNhom,
    short SoCho,
    string LoaiNhienLieu,
    decimal GiaThueNgay,
    string TrangThai,
    string? DuongDanAnh);

public sealed record XeChiTiet(
    int Id,
    string MaXe,
    string BienSo,
    string HangSanXuat,
    string DongXe,
    short NamSanXuat,
    string TenNhom,
    short SoCho,
    string LoaiNhienLieu,
    decimal GiaThueNgay,
    int? KmToiDaMoiNgay,
    decimal? PhiVuotKm,
    decimal PhiTreGio,
    decimal TienCoc,
    string TrangThai,
    decimal? TinhTrangPin,
    int SoKmHienTai,
    IReadOnlyList<string> HangGplxChapNhan,
    string? DuongDanAnh);

/// <summary>Truy vấn chỉ-đọc phục vụ trang danh sách và chi tiết xe.</summary>
public sealed class XeTruyVanDichVu
{
    private readonly DtRentDbContext _db;

    public XeTruyVanDichVu(DtRentDbContext db) => _db = db;

    public async Task<IReadOnlyList<NhomXeLoc>> LayNhomXeAsync(CancellationToken ct = default) =>
        await _db.NhomXes.AsNoTracking()
            .OrderBy(n => n.Ten)
            .Select(n => new NhomXeLoc(n.Ma, n.Ten))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<XeTomTat>> LayDanhSachAsync(
        string? maNhom, string? trangThai, CancellationToken ct = default)
    {
        var truyVan = _db.Xes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(maNhom))
            truyVan = truyVan.Where(x => x.NhomXe.Ma == maNhom);
        if (!string.IsNullOrWhiteSpace(trangThai))
            truyVan = truyVan.Where(x => x.TrangThai == trangThai);

        return await truyVan
            .OrderBy(x => x.NhomXe.Ten)
            .ThenBy(x => x.MaXe)
            .Select(x => new XeTomTat(
                x.Id,
                x.BienSo,
                x.HangSanXuat,
                x.DongXe,
                x.NamSanXuat,
                x.NhomXe.Ma,
                x.NhomXe.Ten,
                x.NhomXe.SoCho,
                x.NhomXe.LoaiNhienLieu,
                x.NhomXe.GiaThueNgay,
                x.TrangThai,
                _db.AnhXes
                    .Where(a => a.XeId == x.Id && a.LaAnhChinh)
                    .Select(a => a.DuongDan)
                    .FirstOrDefault()))
            .ToListAsync(ct);
    }

    public async Task<XeChiTiet> LayChiTietAsync(int id, CancellationToken ct = default)
    {
        var xe = await _db.Xes.AsNoTracking()
            .Include(x => x.NhomXe).ThenInclude(n => n.HangGplxMas)
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KhongTimThayException("xe", id);

        string? anh = await _db.AnhXes.AsNoTracking()
            .Where(a => a.XeId == id && a.LaAnhChinh)
            .Select(a => a.DuongDan)
            .FirstOrDefaultAsync(ct);

        var nhom = xe.NhomXe;
        return new XeChiTiet(
            xe.Id, xe.MaXe, xe.BienSo, xe.HangSanXuat, xe.DongXe, xe.NamSanXuat,
            nhom.Ten, nhom.SoCho, nhom.LoaiNhienLieu,
            nhom.GiaThueNgay, nhom.KmToiDaMoiNgay, nhom.PhiVuotKm, nhom.PhiTreGio, nhom.TienCoc,
            xe.TrangThai, xe.TinhTrangPin, xe.SoKmHienTai,
            nhom.HangGplxMas.Select(h => h.Ma).OrderBy(m => m).ToList(),
            anh);
    }
}