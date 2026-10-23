namespace DtRent.Domain.Pricing;

/// <summary>
/// Bản chụp các quy tắc giá của một nhóm xe (khớp các cột giá của bảng nhom_xe và phieu_thue).
/// </summary>
public sealed class BangGia
{
    public BangGia(decimal donGiaNgay, int? kmToiDaMoiNgay, decimal? phiVuotKm, decimal phiTreGio)
    {
        if (donGiaNgay <= 0)
            throw new ArgumentOutOfRangeException(nameof(donGiaNgay), "Đơn giá ngày phải lớn hơn 0.");
        if (kmToiDaMoiNgay is <= 0)
            throw new ArgumentOutOfRangeException(nameof(kmToiDaMoiNgay), "Km tối đa mỗi ngày phải lớn hơn 0.");
        if (phiVuotKm is < 0)
            throw new ArgumentOutOfRangeException(nameof(phiVuotKm), "Phí vượt km không được âm.");
        if (phiTreGio < 0)
            throw new ArgumentOutOfRangeException(nameof(phiTreGio), "Phí trễ mỗi giờ không được âm.");
        if (kmToiDaMoiNgay.HasValue != phiVuotKm.HasValue)
            throw new ArgumentException("Km tối đa mỗi ngày và phí vượt km phải cùng có giá trị hoặc cùng để trống.");

        DonGiaNgay = donGiaNgay;
        KmToiDaMoiNgay = kmToiDaMoiNgay;
        PhiVuotKm = phiVuotKm;
        PhiTreGio = phiTreGio;
    }

    public decimal DonGiaNgay { get; }
    public int? KmToiDaMoiNgay { get; }
    public decimal? PhiVuotKm { get; }
    public decimal PhiTreGio { get; }
}