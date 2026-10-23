using DtRent.Domain.Enums;
using DtRent.Domain.Models;
using DtRent.Domain.Pricing;

namespace DtRent.Tests;

internal static class DuLieuMau
{
    /// <summary>800.000 đ/ngày, tối đa 200 km/ngày, vượt 2.000 đ/km, trễ 80.000 đ/giờ; nhận GPLX B1, B2.</summary>
    public static NhomXeOto OtoDien5Cho() => new(
        "OTO_DIEN_5CHO", "Ô tô điện 5 chỗ", ChinhSachGiaFactory.NgayCoGioiHanKm,
        new BangGia(donGiaNgay: 800_000m, kmToiDaMoiNgay: 200, phiVuotKm: 2_000m, phiTreGio: 80_000m),
        tienCoc: 5_000_000m, hangGplxChapNhan: new[] { "B1", "B2" });

    /// <summary>650.000 đ/ngày, không giới hạn km, trễ 65.000 đ/giờ; chỉ nhận GPLX B2.</summary>
    public static NhomXeOto OtoXang5Cho() => new(
        "OTO_XANG_5CHO", "Ô tô xăng 5 chỗ", ChinhSachGiaFactory.NgayKhongGioiHanKm,
        new BangGia(donGiaNgay: 650_000m, kmToiDaMoiNgay: null, phiVuotKm: null, phiTreGio: 65_000m),
        tienCoc: 3_000_000m, hangGplxChapNhan: new[] { "B2" });

    public static KhachThue Khach(HangKhachHang hang, params GplxKhach[] gplx) =>
        new("KH001", "Lê Văn An", "0912345001", hang, gplx);

    public static XeOto Xe(NhomXeOto nhom, TrangThaiXe trangThai = TrangThaiXe.SanSang) =>
        new("XE001", "66A-123.45", nhom, trangThai);
}