namespace DtRent.Domain.Pricing;

/// <summary>Chiến lược (Strategy) tính tiền thuê. Mỗi nhóm xe chọn một chính sách qua mã.</summary>
public interface IChinhSachGia
{
    KetQuaTinhTien Tinh(BangGia bangGia, int soNgayThue, int soKmDaDi, TimeSpan thoiGianTre);
}

/// <summary>
/// Phần chung (Template Method): tiền thuê theo ngày, phạt trễ theo giờ (phần lẻ làm tròn lên).
/// Lớp con chỉ quyết định cách tính số km vượt.
/// </summary>
public abstract class ChinhSachGiaCoSo : IChinhSachGia
{
    public KetQuaTinhTien Tinh(BangGia bangGia, int soNgayThue, int soKmDaDi, TimeSpan thoiGianTre)
    {
        ArgumentNullException.ThrowIfNull(bangGia);
        if (soNgayThue < 1)
            throw new ArgumentOutOfRangeException(nameof(soNgayThue), "Số ngày thuê phải từ 1 trở lên.");
        if (soKmDaDi < 0)
            throw new ArgumentOutOfRangeException(nameof(soKmDaDi), "Số km đã đi không được âm.");

        decimal tienThue = bangGia.DonGiaNgay * soNgayThue;

        int kmVuot = TinhKmVuot(bangGia, soNgayThue, soKmDaDi);
        decimal phiVuotKm = kmVuot * (bangGia.PhiVuotKm ?? 0m);

        int soGioTre = TinhSoGioTre(thoiGianTre);
        decimal phiTre = soGioTre * bangGia.PhiTreGio;

        return new KetQuaTinhTien(tienThue, kmVuot, phiVuotKm, soGioTre, phiTre);
    }

    protected abstract int TinhKmVuot(BangGia bangGia, int soNgayThue, int soKmDaDi);

    private static int TinhSoGioTre(TimeSpan thoiGianTre) =>
        thoiGianTre <= TimeSpan.Zero ? 0 : (int)Math.Ceiling(thoiGianTre.TotalHours);
}

/// <summary>Giá theo ngày, mỗi ngày được đi tối đa N km, vượt thì tính phí theo km.</summary>
public sealed class ChinhSachGiaNgayCoGioiHanKm : ChinhSachGiaCoSo
{
    protected override int TinhKmVuot(BangGia bangGia, int soNgayThue, int soKmDaDi)
    {
        int kmToiDaMoiNgay = bangGia.KmToiDaMoiNgay
            ?? throw new InvalidOperationException(
                "Chính sách có giới hạn km yêu cầu bảng giá phải có km tối đa mỗi ngày.");

        return Math.Max(0, soKmDaDi - kmToiDaMoiNgay * soNgayThue);
    }
}

/// <summary>Giá theo ngày, không giới hạn km.</summary>
public sealed class ChinhSachGiaNgayKhongGioiHanKm : ChinhSachGiaCoSo
{
    protected override int TinhKmVuot(BangGia bangGia, int soNgayThue, int soKmDaDi) => 0;
}

/// <summary>Chọn chính sách theo mã lưu ở cột nhom_xe.chinh_sach_gia.</summary>
public static class ChinhSachGiaFactory
{
    public const string NgayCoGioiHanKm = "NGAY_CO_GIOI_HAN_KM";
    public const string NgayKhongGioiHanKm = "NGAY_KHONG_GIOI_HAN_KM";

    private static readonly IReadOnlyDictionary<string, IChinhSachGia> DanhSach =
        new Dictionary<string, IChinhSachGia>
        {
            [NgayCoGioiHanKm] = new ChinhSachGiaNgayCoGioiHanKm(),
            [NgayKhongGioiHanKm] = new ChinhSachGiaNgayKhongGioiHanKm()
        };

    public static IChinhSachGia Tao(string ma) =>
        DanhSach.TryGetValue(ma, out var chinhSach)
            ? chinhSach
            : throw new ArgumentException($"Chưa có chính sách giá cho mã \"{ma}\".", nameof(ma));
}