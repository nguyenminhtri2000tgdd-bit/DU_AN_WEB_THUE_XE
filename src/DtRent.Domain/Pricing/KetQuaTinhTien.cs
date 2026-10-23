namespace DtRent.Domain.Pricing;

/// <summary>Chi tiết tiền thuê trước khi giảm giá (khớp các cột của bảng tra_xe).</summary>
public sealed record KetQuaTinhTien(
    decimal TienThue,
    int KmVuot,
    decimal PhiVuotKm,
    int SoGioTre,
    decimal PhiTre)
{
    public decimal TongTruocGiam => TienThue + PhiVuotKm + PhiTre;
}

/// <summary>Kết quả cuối cùng sau khi áp dụng giảm giá theo hạng khách.</summary>
public sealed record KetQuaThanhToan(
    KetQuaTinhTien ChiTiet,
    decimal PhanTramGiam,
    decimal TienGiam)
{
    public decimal TongPhaiTra => ChiTiet.TongTruocGiam - TienGiam;
}