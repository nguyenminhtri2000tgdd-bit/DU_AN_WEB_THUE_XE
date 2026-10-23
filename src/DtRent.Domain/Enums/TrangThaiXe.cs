namespace DtRent.Domain.Enums;

public enum TrangThaiXe
{
    SanSang,
    DangChoThue,
    DangBaoTri
}

public static class TrangThaiXeExtensions
{
    public static string HienThi(this TrangThaiXe trangThai) => trangThai switch
    {
        TrangThaiXe.SanSang => "Sẵn sàng",
        TrangThaiXe.DangChoThue => "Đang cho thuê",
        TrangThaiXe.DangBaoTri => "Đang bảo trì",
        _ => trangThai.ToString()
    };

    /// <summary>Chuyển mã lưu trong CSDL (SAN_SANG, DANG_CHO_THUE, DANG_BAO_TRI) thành enum.</summary>
    public static TrangThaiXe TuMaCsdl(string ma) => ma switch
    {
        "SAN_SANG" => TrangThaiXe.SanSang,
        "DANG_CHO_THUE" => TrangThaiXe.DangChoThue,
        "DANG_BAO_TRI" => TrangThaiXe.DangBaoTri,
        _ => throw new ArgumentException($"Mã trạng thái xe không hợp lệ: {ma}", nameof(ma))
    };
}