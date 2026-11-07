using System.Globalization;

namespace DtRent.Web.Helpers;

public static class DinhDang
{
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    public const string AnhMacDinh = "/assets/xe-placeholder.svg";

    public static string Tien(decimal soTien) => soTien.ToString("N0", Vi) + " đ";

    public static string So(int so) => so.ToString("N0", Vi);

    public static string NgayGio(DateTimeOffset thoiDiem) =>
    thoiDiem.ToString("HH:mm, dd/MM/yyyy", CultureInfo.InvariantCulture);

    public static string Iso(DateTimeOffset thoiDiem) =>
        thoiDiem.ToString("yyyy-MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture);

    public static string NhienLieu(string ma) => ma switch
    {
        "DIEN" => "Điện",
        "XANG" => "Xăng",
        "DAU" => "Dầu",
        _ => ma
    };

    public static string Anh(string? duongDan) =>
        string.IsNullOrWhiteSpace(duongDan) ? AnhMacDinh : duongDan;
}