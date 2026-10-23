namespace DtRent.Domain.Models;

/// <summary>Một GPLX của khách: hạng và ngày hết hạn (để trống nếu không thời hạn).</summary>
public sealed record GplxKhach(string HangGplx, DateOnly? NgayHetHan)
{
    public bool ConHieuLuc(DateOnly ngay) => NgayHetHan is null || NgayHetHan.Value >= ngay;
}

public sealed class KhachThue
{
    private readonly List<GplxKhach> _gplx = new();

    public KhachThue(string ma, string hoTen, string soDienThoai,
                     HangKhachHang hang, IEnumerable<GplxKhach>? gplx = null)
    {
        if (string.IsNullOrWhiteSpace(ma))
            throw new ArgumentException("Mã khách không được để trống.", nameof(ma));
        if (string.IsNullOrWhiteSpace(hoTen))
            throw new ArgumentException("Họ tên không được để trống.", nameof(hoTen));
        if (soDienThoai is null || soDienThoai.Length != 10 || !soDienThoai.All(char.IsAsciiDigit))
            throw new ArgumentException("Số điện thoại phải gồm đúng 10 chữ số.", nameof(soDienThoai));
        ArgumentNullException.ThrowIfNull(hang);

        Ma = ma;
        HoTen = hoTen;
        SoDienThoai = soDienThoai;
        Hang = hang;
        if (gplx is not null) _gplx.AddRange(gplx);
    }

    public string Ma { get; }
    public string HoTen { get; }
    public string SoDienThoai { get; }
    public HangKhachHang Hang { get; }
    public IReadOnlyList<GplxKhach> DanhSachGplx => _gplx;

    /// <summary>Khách có ít nhất một GPLX thuộc các hạng được chấp nhận và còn hiệu lực đến ngày chỉ định.</summary>
    public bool CoGplxPhuHop(IReadOnlyCollection<string> hangChapNhan, DateOnly ngay) =>
        _gplx.Any(g => g.ConHieuLuc(ngay) && hangChapNhan.Contains(g.HangGplx));
}