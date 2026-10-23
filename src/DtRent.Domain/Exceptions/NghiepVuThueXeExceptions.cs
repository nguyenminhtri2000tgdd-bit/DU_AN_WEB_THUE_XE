using DtRent.Domain.Enums;

namespace DtRent.Domain.Exceptions;

/// <summary>Lớp cơ sở của mọi lỗi vi phạm quy tắc nghiệp vụ thuê xe.</summary>
public abstract class NghiepVuThueXeException : Exception
{
    protected NghiepVuThueXeException(string maLoi, string message) : base(message)
        => MaLoi = maLoi;

    public string MaLoi { get; }
}

public sealed class XeKhongSanSangException : NghiepVuThueXeException
{
    public XeKhongSanSangException(string bienSo, TrangThaiXe trangThai)
        : base("XE_KHONG_SAN_SANG",
               $"Xe {bienSo} không thể cho thuê vì đang ở trạng thái \"{trangThai.HienThi()}\".")
    {
    }
}

public sealed class GplxKhongPhuHopException : NghiepVuThueXeException
{
    public GplxKhongPhuHopException(string tenKhach, string tenNhomXe, IEnumerable<string> hangChapNhan)
        : base("GPLX_KHONG_PHU_HOP",
               $"Khách {tenKhach} chưa có GPLX còn hiệu lực để thuê nhóm xe \"{tenNhomXe}\" " +
               $"(yêu cầu một trong các hạng: {string.Join(", ", hangChapNhan)}).")
    {
    }
}

public sealed class ThoiGianThueKhongHopLeException : NghiepVuThueXeException
{
    public ThoiGianThueKhongHopLeException(string message)
        : base("THOI_GIAN_THUE_KHONG_HOP_LE", message)
    {
    }
}