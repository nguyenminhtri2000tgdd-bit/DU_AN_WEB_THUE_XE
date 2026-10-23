using DtRent.Domain.Exceptions;
using DtRent.Domain.Models;

namespace DtRent.Domain.Services;

public static class KiemTraDieuKienThue
{
    /// <summary>
    /// Kiểm tra điều kiện lập phiếu thuê. Ném <see cref="NghiepVuThueXeException"/> khi không đủ điều kiện.
    /// GPLX phải còn hiệu lực đến hết ngày hẹn trả.
    /// </summary>
    public static void KiemTra(KhachThue khach, XeOto xe,
                               DateTimeOffset thoiDiemNhan, DateTimeOffset thoiDiemHenTra)
    {
        ArgumentNullException.ThrowIfNull(khach);
        ArgumentNullException.ThrowIfNull(xe);

        if (thoiDiemHenTra <= thoiDiemNhan)
            throw new ThoiGianThueKhongHopLeException("Thời điểm hẹn trả phải sau thời điểm nhận xe.");

        if (!xe.SanSangChoThue)
            throw new XeKhongSanSangException(xe.BienSo, xe.TrangThai);

        var ngayHenTra = DateOnly.FromDateTime(thoiDiemHenTra.DateTime);
        if (!khach.CoGplxPhuHop(xe.Nhom.HangGplxChapNhan, ngayHenTra))
            throw new GplxKhongPhuHopException(khach.HoTen, xe.Nhom.Ten, xe.Nhom.HangGplxChapNhan);
    }
}