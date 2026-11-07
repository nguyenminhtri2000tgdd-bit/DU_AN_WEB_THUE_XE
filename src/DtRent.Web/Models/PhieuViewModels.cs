using System.ComponentModel.DataAnnotations;
using DtRent.Application.TruyVan;
using DtRent.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DtRent.Web.Models;

public static class TrangThaiPhieuHienThi
{
    public static (string Chu, string Css) Tu(string maCsdl) => maCsdl switch
    {
        "DANG_THUE" => ("Đang thuê", "busy"),
        "DA_TRA" => ("Đã trả", "ok"),
        "DA_HUY" => ("Đã hủy", "maintenance"),
        _ => (maCsdl, "ok")
    };
}

/// <summary>Dữ liệu người dùng nhập trên form lập phiếu thuê.</summary>
public sealed class LapPhieuForm
{
    [Required(ErrorMessage = "Vui lòng chọn xe.")]
    public int? XeId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
    public long? KhachHangId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn nhân viên lập phiếu.")]
    public int? NhanVienId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thời điểm nhận xe.")]
    public DateTime? ThoiDiemNhan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số ngày thuê.")]
    [Range(1, 365, ErrorMessage = "Số ngày thuê phải từ 1 đến 365.")]
    public int? SoNgayThue { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

public sealed class LapPhieuVm
{
    public LapPhieuForm Form { get; set; } = new();
    public XeChiTietVm Xe { get; init; } = null!;
    public IReadOnlyList<SelectListItem> KhachHang { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> NhanVien { get; init; } = Array.Empty<SelectListItem>();
    public string? LoiNghiepVu { get; set; }
}

public sealed class PhieuDongVm
{
    public long Id { get; init; }
    public string MaPhieu { get; init; } = "";
    public string KhachHoTen { get; init; } = "";
    public string TenXe { get; init; } = "";
    public string BienSo { get; init; } = "";
    public string NhanText { get; init; } = "";
    public string NhanIso { get; init; } = "";
    public string HenTraText { get; init; } = "";
    public string HenTraIso { get; init; } = "";
    public string TrangThaiChu { get; init; } = "";
    public string TrangThaiCss { get; init; } = "ok";

    public static PhieuDongVm Tu(PhieuTomTat p)
    {
        var (chu, css) = TrangThaiPhieuHienThi.Tu(p.TrangThai);
        return new PhieuDongVm
        {
            Id = p.Id,
            MaPhieu = p.MaPhieu,
            KhachHoTen = p.HoTenKhach,
            TenXe = p.TenXe,
            BienSo = p.BienSo,
            NhanText = DinhDang.NgayGio(p.ThoiDiemNhan),
            NhanIso = DinhDang.Iso(p.ThoiDiemNhan),
            HenTraText = DinhDang.NgayGio(p.ThoiDiemHenTra),
            HenTraIso = DinhDang.Iso(p.ThoiDiemHenTra),
            TrangThaiChu = chu,
            TrangThaiCss = css
        };
    }
}

public sealed record DongTinhTienVm(string Khoan, string CachTinh, string ThanhTien);

public sealed class KetQuaTraXeVm
{
    public string TraText { get; init; } = "";
    public string TraIso { get; init; } = "";
    public string KmDaDiText { get; init; } = "";
    public string SoKmTraText { get; init; } = "";
    public string XeSauTra { get; init; } = "";
    public string? PinTraText { get; init; }
    public IReadOnlyList<DongTinhTienVm> Dong { get; init; } = Array.Empty<DongTinhTienVm>();
    public string ChenhLechNhan { get; init; } = "";
    public string ChenhLechText { get; init; } = "";
    public string? GhiChu { get; init; }
}   

public sealed class PhieuChiTietVm
{
    public long Id { get; init; }
    public string MaPhieu { get; init; } = "";
    public string TrangThaiChu { get; init; } = "";
    public string TrangThaiCss { get; init; } = "ok";
    public bool DangThue { get; init; }
    public string KhachHoTen { get; init; } = "";
    public string KhachMa { get; init; } = "";
    public string KhachSdt { get; init; } = "";
    public string HangText { get; init; } = "";
    public int XeId { get; init; }
    public string TenXe { get; init; } = "";
    public string BienSo { get; init; } = "";
    public string NhanVien { get; init; } = "";
    public string NhanText { get; init; } = "";
    public string NhanIso { get; init; } = "";
    public string HenTraText { get; init; } = "";
    public string HenTraIso { get; init; } = "";
    public string SoNgayText { get; init; } = "";
    public int SoKmNhan { get; init; }
    public string SoKmNhanText { get; init; } = "";
    public bool CoPin { get; init; }
    public string? PinNhanText { get; init; }
    public string GiaNgayText { get; init; } = "";
    public string GioiHanKmText { get; init; } = "";
    public string? PhiVuotKmText { get; init; }
    public string PhiTreText { get; init; } = "";
    public string TienCocText { get; init; } = "";
    public string TienCoBanText { get; init; } = "";
    public string GiamText { get; init; } = "";
    public string TienGiamText { get; init; } = "";
    public string TienSauGiamText { get; init; } = "";
    public string? GhiChu { get; init; }
    public KetQuaTraXeVm? TraXe { get; init; }

    public static PhieuChiTietVm Tu(PhieuChiTiet c)
    {
        var (chu, css) = TrangThaiPhieuHienThi.Tu(c.TrangThai);

        decimal tienCoBan = c.SoNgayThue * c.DonGiaNgay;
        decimal tienGiam = Math.Round(tienCoBan * c.PhanTramGiam / 100m, 0, MidpointRounding.AwayFromZero);

        return new PhieuChiTietVm
        {
            Id = c.Id,
            MaPhieu = c.MaPhieu,
            TrangThaiChu = chu,
            TrangThaiCss = css,
            DangThue = c.TrangThai == "DANG_THUE",
            KhachHoTen = c.HoTenKhach,
            KhachMa = c.MaKhach,
            KhachSdt = c.SoDienThoaiKhach,
            HangText = $"{c.TenHangKhach} (giảm {c.PhanTramGiam:0.#}%)",
            XeId = c.XeId,
            TenXe = c.TenXe,
            BienSo = c.BienSo,
            NhanVien = c.HoTenNhanVien,
            NhanText = DinhDang.NgayGio(c.ThoiDiemNhan),
            NhanIso = DinhDang.Iso(c.ThoiDiemNhan),
            HenTraText = DinhDang.NgayGio(c.ThoiDiemHenTra),
            HenTraIso = DinhDang.Iso(c.ThoiDiemHenTra),
            SoNgayText = $"{c.SoNgayThue} ngày",
            SoKmNhan = c.SoKmNhan,
            SoKmNhanText = $"{DinhDang.So(c.SoKmNhan)} km",
            CoPin = c.TinhTrangPinNhan is not null,
            PinNhanText = c.TinhTrangPinNhan is null ? null : $"{c.TinhTrangPinNhan:0.#}%",
            GiaNgayText = DinhDang.Tien(c.DonGiaNgay),
            GioiHanKmText = c.KmToiDaMoiNgay is null
                ? "Không giới hạn km"
                : $"{DinhDang.So(c.KmToiDaMoiNgay.Value)} km mỗi ngày",
            PhiVuotKmText = c.PhiVuotKm is null ? null : $"{DinhDang.Tien(c.PhiVuotKm.Value)} mỗi km vượt",
            PhiTreText = $"{DinhDang.Tien(c.PhiTreGio)} mỗi giờ",
            TienCocText = DinhDang.Tien(c.TienCoc),
            TienCoBanText = $"{c.SoNgayThue} × {DinhDang.Tien(c.DonGiaNgay)} = {DinhDang.Tien(tienCoBan)}",
            GiamText = $"{c.PhanTramGiam:0.#}%",
            TienGiamText = DinhDang.Tien(tienGiam),
            TienSauGiamText = DinhDang.Tien(tienCoBan - tienGiam),
            GhiChu = c.GhiChu,
            TraXe = c.TraXe is null ? null : TaoKetQuaTraXe(c, c.TraXe)       
        };
    }

    private static KetQuaTraXeVm TaoKetQuaTraXe(PhieuChiTiet c, TraXeThongTin t)
    {
        var tre = t.ThoiDiemTraThucTe - c.ThoiDiemHenTra;
        string treText = tre <= TimeSpan.Zero
            ? "đúng hạn"
            : $"trễ {(int)tre.TotalHours} giờ {tre.Minutes} phút";

        string cachVuotKm = c.KmToiDaMoiNgay is null
            ? "Không giới hạn km"
            : $"{DinhDang.So(t.KmVuot)} km vượt × {DinhDang.Tien(c.PhiVuotKm ?? 0m)} " +
            $"(đã đi {DinhDang.So(t.KmDaDi)} km, hạn mức {DinhDang.So(c.KmToiDaMoiNgay.Value * c.SoNgayThue)} km)";

        string cachTre = t.SoGioTre == 0
            ? "Trả đúng hạn"
            : $"{t.SoGioTre} giờ × {DinhDang.Tien(c.PhiTreGio)} ({treText}, phần lẻ làm tròn lên)";

        var dong = new List<DongTinhTienVm>
        {
            new("Tiền thuê", $"{c.SoNgayThue} ngày × {DinhDang.Tien(c.DonGiaNgay)}", DinhDang.Tien(t.TienThue)),
            new("Phí vượt km", cachVuotKm, DinhDang.Tien(t.PhiVuotKm)),
            new("Phí trả trễ", cachTre, DinhDang.Tien(t.PhiTre)),
            new("Tổng trước giảm giá", "Cộng ba khoản trên", DinhDang.Tien(t.TongTruocGiam)),
            new($"Giảm giá hạng khách ({c.PhanTramGiam:0.#}%)", "Tính trên tổng trước giảm giá", "−" + DinhDang.Tien(t.TienGiam)),
            new("Tổng phải trả", "Sau giảm giá", DinhDang.Tien(t.TongPhaiTra)),
            new("Tiền cọc đã thu", "Thu khi nhận xe", DinhDang.Tien(c.TienCoc))
        };

        decimal chenhLech = t.TongPhaiTra - c.TienCoc;
        string chenhLechNhan = chenhLech > 0
            ? "Khách thanh toán thêm"
            : chenhLech < 0
                ? "Hoàn lại tiền cọc cho khách"
                : "Không phát sinh thanh toán thêm";

        return new KetQuaTraXeVm
        {
            TraText = DinhDang.NgayGio(t.ThoiDiemTraThucTe),
            TraIso = DinhDang.Iso(t.ThoiDiemTraThucTe),
            KmDaDiText = $"{DinhDang.So(t.KmDaDi)} km",
            SoKmTraText = $"{DinhDang.So(t.SoKmTra)} km",
            XeSauTra = t.XeHuHong ? "Đang bảo trì (xe bị hư hỏng)" : "Sẵn sàng",
            PinTraText = t.TinhTrangPinTra is null ? null : $"{t.TinhTrangPinTra:0.#}%",
            Dong = dong,
            ChenhLechNhan = chenhLechNhan,
            ChenhLechText = DinhDang.Tien(Math.Abs(chenhLech)),
            GhiChu = t.GhiChu
        };
    }
}    



/// <summary>Dữ liệu người dùng nhập trên form trả xe.</summary>
public sealed class TraXeForm
{
    public long PhieuThueId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn nhân viên nhận xe.")]
    public int? NhanVienId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thời điểm trả xe thực tế.")]
    public DateTime? ThoiDiemTra { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số km hiện tại của xe.")]
    [Range(0, 10_000_000, ErrorMessage = "Số km không hợp lệ.")]
    public int? SoKmTra { get; set; }

    public bool XeHuHong { get; set; }

    [Range(0, 100, ErrorMessage = "Tình trạng pin phải từ 0 đến 100 (%).")]
    public int? TinhTrangPinTra { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

public sealed class TraXeVm
{
    public TraXeForm Form { get; set; } = new();
    public PhieuChiTietVm Phieu { get; init; } = null!;
    public IReadOnlyList<SelectListItem> NhanVien { get; init; } = Array.Empty<SelectListItem>();
    public string? LoiNghiepVu { get; set; }
}