using DtRent.Domain.Pricing;

namespace DtRent.Domain.Models;

public sealed class NhomXeOto
{
    private readonly HashSet<string> _hangGplxChapNhan;

    public NhomXeOto(string ma, string ten, string maChinhSachGia, BangGia bangGia,
                     decimal tienCoc, IEnumerable<string> hangGplxChapNhan)
    {
        if (string.IsNullOrWhiteSpace(ma))
            throw new ArgumentException("Mã nhóm xe không được để trống.", nameof(ma));
        if (string.IsNullOrWhiteSpace(ten))
            throw new ArgumentException("Tên nhóm xe không được để trống.", nameof(ten));
        ArgumentNullException.ThrowIfNull(bangGia);
        if (tienCoc < 0)
            throw new ArgumentOutOfRangeException(nameof(tienCoc), "Tiền cọc không được âm.");
        ArgumentNullException.ThrowIfNull(hangGplxChapNhan);

        _hangGplxChapNhan = new HashSet<string>(hangGplxChapNhan, StringComparer.OrdinalIgnoreCase);
        if (_hangGplxChapNhan.Count == 0)
            throw new ArgumentException("Nhóm xe phải chấp nhận ít nhất một hạng GPLX.", nameof(hangGplxChapNhan));

        Ma = ma;
        Ten = ten;
        BangGia = bangGia;
        TienCoc = tienCoc;
        ChinhSachGia = ChinhSachGiaFactory.Tao(maChinhSachGia);
    }

    public string Ma { get; }
    public string Ten { get; }
    public BangGia BangGia { get; }
    public decimal TienCoc { get; }
    public IChinhSachGia ChinhSachGia { get; }
    public IReadOnlyCollection<string> HangGplxChapNhan => _hangGplxChapNhan;

    public KetQuaTinhTien TinhTienThue(int soNgayThue, int soKmDaDi, TimeSpan thoiGianTre) =>
        ChinhSachGia.Tinh(BangGia, soNgayThue, soKmDaDi, thoiGianTre);
}