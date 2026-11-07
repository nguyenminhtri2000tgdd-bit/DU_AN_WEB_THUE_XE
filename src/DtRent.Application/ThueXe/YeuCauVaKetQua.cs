using DtRent.Domain.Pricing;

namespace DtRent.Application.ThueXe;

/// <summary>Hẹn trả = thời điểm nhận + số ngày thuê (đúng với cột so_ngay_thue của phiếu).</summary>
public sealed record LapPhieuThueYeuCau(
    long KhachHangId,
    int XeId,
    int NhanVienId,
    DateTimeOffset ThoiDiemNhan,
    int SoNgayThue,
    string? GhiChu = null);

public sealed record LapPhieuThueKetQua(
    long PhieuThueId,
    string MaPhieu,
    DateTimeOffset ThoiDiemHenTra,
    decimal TienCoc);

public sealed record TraXeYeuCau(
    long PhieuThueId,
    int NhanVienId,
    DateTimeOffset ThoiDiemTraThucTe,
    int SoKmTra,
    bool XeHuHong = false,
    decimal? TinhTrangPinTra = null,
    string? GhiChu = null);

public sealed record TraXeKetQua(
    long TraXeId,
    string MaPhieu,
    KetQuaThanhToan ThanhToan);