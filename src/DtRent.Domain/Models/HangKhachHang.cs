using DtRent.Domain.Pricing;

namespace DtRent.Domain.Models;

/// <summary>Hạng khách (vãng lai, Bạc, Vàng) với phần trăm giảm lấy từ dữ liệu.</summary>
public sealed class HangKhachHang
{
    public static readonly HangKhachHang VangLai = new("VANG_LAI", "Khách vãng lai", 0m);
    public static readonly HangKhachHang Bac = new("BAC", "Thành viên hạng Bạc", 5m);
    public static readonly HangKhachHang Vang = new("VANG", "Thành viên hạng Vàng", 10m);

    public HangKhachHang(string ma, string ten, decimal phanTramGiam)
    {
        if (string.IsNullOrWhiteSpace(ma))
            throw new ArgumentException("Mã hạng không được để trống.", nameof(ma));
        if (string.IsNullOrWhiteSpace(ten))
            throw new ArgumentException("Tên hạng không được để trống.", nameof(ten));
        if (phanTramGiam < 0 || phanTramGiam > 100)
            throw new ArgumentOutOfRangeException(nameof(phanTramGiam),
                "Phần trăm giảm phải nằm trong khoảng 0 đến 100.");

        Ma = ma;
        Ten = ten;
        PhanTramGiam = phanTramGiam;
    }

    public string Ma { get; }
    public string Ten { get; }
    public decimal PhanTramGiam { get; }

    /// <summary>Giảm giá trên tổng tiền thuê (gồm cả phí vượt km và phí trễ), làm tròn đến đồng.</summary>
    public KetQuaThanhToan ApDungGiamGia(KetQuaTinhTien chiTiet)
    {
        ArgumentNullException.ThrowIfNull(chiTiet);

        decimal tienGiam = Math.Round(
            chiTiet.TongTruocGiam * PhanTramGiam / 100m, 0, MidpointRounding.AwayFromZero);

        return new KetQuaThanhToan(chiTiet, PhanTramGiam, tienGiam);
    }
}