using DtRent.Domain.Enums;
using DtRent.Domain.Models;
using DtRent.Domain.Pricing;
using DtRent.Infrastructure.Persistence.Entities;

namespace DtRent.Application.ThueXe;

/// <summary>Chuyển entity EF (dữ liệu) sang đối tượng Domain (quy tắc nghiệp vụ).</summary>
internal static class ChuyenDoiDomain
{
    public static NhomXeOto ToDomain(this NhomXe nhom) => new(
        nhom.Ma,
        nhom.Ten,
        nhom.ChinhSachGia,
        new BangGia(nhom.GiaThueNgay, nhom.KmToiDaMoiNgay, nhom.PhiVuotKm, nhom.PhiTreGio),
        nhom.TienCoc,
        nhom.HangGplxMas.Select(h => h.Ma));

    public static XeOto ToDomain(this Xe xe) => new(
        xe.MaXe,
        xe.BienSo,
        xe.NhomXe.ToDomain(),
        TrangThaiXeExtensions.TuMaCsdl(xe.TrangThai));

    public static HangKhachHang ToDomain(this HangThanhVien hang) =>
        new(hang.Ma, hang.Ten, hang.PhanTramGiam);

    public static KhachThue ToDomain(this KhachHang khach) => new(
        khach.MaKhach,
        khach.HoTen,
        khach.SoDienThoai,
        khach.HangThanhVien.ToDomain(),
        khach.GplxKhachHangs.Select(g => new GplxKhach(g.HangGplxMa, g.NgayHetHan)));
}