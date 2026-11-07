using DtRent.Application.TruyVan;
using DtRent.Domain.Enums;
using DtRent.Web.Helpers;

namespace DtRent.Web.Models;

/// <summary>Đổi mã trạng thái trong CSDL thành chữ hiển thị và hậu tố class của nhãn (badge--ok / busy / maintenance).</summary>
public static class TrangThaiHienThi
{
    public static (string Chu, string Css) Tu(string maCsdl)
    {
        var trangThai = TrangThaiXeExtensions.TuMaCsdl(maCsdl);
        string css = trangThai switch
        {
            TrangThaiXe.SanSang => "ok",
            TrangThaiXe.DangChoThue => "busy",
            TrangThaiXe.DangBaoTri => "maintenance",
            _ => "ok"
        };
        return (trangThai.HienThi(), css);
    }
}

public sealed class XeCardVm
{
    public int Id { get; init; }
    public string TenXe { get; init; } = "";
    public string BienSo { get; init; } = "";
    public string TenNhom { get; init; } = "";
    public int SoCho { get; init; }
    public string NhienLieu { get; init; } = "";
    public string GiaText { get; init; } = "";
    public string TrangThaiChu { get; init; } = "";
    public string TrangThaiCss { get; init; } = "ok";
    public string AnhUrl { get; init; } = DinhDang.AnhMacDinh;

    public static XeCardVm Tu(XeTomTat x)
    {
        var (chu, css) = TrangThaiHienThi.Tu(x.TrangThai);
        return new XeCardVm
        {
            Id = x.Id,
            TenXe = $"{x.HangSanXuat} {x.DongXe} ({x.NamSanXuat})",
            BienSo = x.BienSo,
            TenNhom = x.TenNhom,
            SoCho = x.SoCho,
            NhienLieu = DinhDang.NhienLieu(x.LoaiNhienLieu),
            GiaText = DinhDang.Tien(x.GiaThueNgay),
            TrangThaiChu = chu,
            TrangThaiCss = css,
            AnhUrl = DinhDang.Anh(x.DuongDanAnh)
        };
    }
}

public sealed class XeDanhSachVm
{
    public IReadOnlyList<XeCardVm> Xe { get; init; } = Array.Empty<XeCardVm>();
    public IReadOnlyList<NhomXeLoc> NhomXe { get; init; } = Array.Empty<NhomXeLoc>();
    public string? NhomDangChon { get; init; }
    public string? TrangThaiDangChon { get; init; }
}

public sealed class XeChiTietVm
{
    public int Id { get; init; }
    public string TenXe { get; init; } = "";
    public string BienSo { get; init; } = "";
    public string TenNhom { get; init; } = "";
    public string HangSanXuat { get; init; } = "";
    public string DongXe { get; init; } = "";
    public short NamSanXuat { get; init; }
    public int SoCho { get; init; }
    public string NhienLieu { get; init; } = "";
    public string? PinText { get; init; }
    public string SoKmText { get; init; } = "";
    public string GiaNgayText { get; init; } = "";
    public string GioiHanKmText { get; init; } = "";
    public string? PhiVuotKmText { get; init; }
    public string PhiTreText { get; init; } = "";
    public string TienCocText { get; init; } = "";
    public string HangGplxText { get; init; } = "";
    public string TrangThaiChu { get; init; } = "";
    public string TrangThaiCss { get; init; } = "ok";
    public bool SanSang { get; init; }
    public string AnhUrl { get; init; } = DinhDang.AnhMacDinh;

    public static XeChiTietVm Tu(XeChiTiet c)
    {
        var (chu, css) = TrangThaiHienThi.Tu(c.TrangThai);
        return new XeChiTietVm
        {
            Id = c.Id,
            TenXe = $"{c.HangSanXuat} {c.DongXe} ({c.NamSanXuat})",
            BienSo = c.BienSo,
            TenNhom = c.TenNhom,
            HangSanXuat = c.HangSanXuat,
            DongXe = c.DongXe,
            NamSanXuat = c.NamSanXuat,
            SoCho = c.SoCho,
            NhienLieu = DinhDang.NhienLieu(c.LoaiNhienLieu),
            PinText = c.TinhTrangPin is null ? null : $"{c.TinhTrangPin:0.#}%",
            SoKmText = $"{DinhDang.So(c.SoKmHienTai)} km",
            GiaNgayText = DinhDang.Tien(c.GiaThueNgay),
            GioiHanKmText = c.KmToiDaMoiNgay is null
                ? "Không giới hạn km"
                : $"{DinhDang.So(c.KmToiDaMoiNgay.Value)} km mỗi ngày",
            PhiVuotKmText = c.PhiVuotKm is null ? null : $"{DinhDang.Tien(c.PhiVuotKm.Value)} mỗi km vượt",
            PhiTreText = $"{DinhDang.Tien(c.PhiTreGio)} mỗi giờ",
            TienCocText = DinhDang.Tien(c.TienCoc),
            HangGplxText = string.Join(", ", c.HangGplxChapNhan),
            TrangThaiChu = chu,
            TrangThaiCss = css,
            SanSang = c.TrangThai == "SAN_SANG",
            AnhUrl = DinhDang.Anh(c.DuongDanAnh)
        };
    }
}