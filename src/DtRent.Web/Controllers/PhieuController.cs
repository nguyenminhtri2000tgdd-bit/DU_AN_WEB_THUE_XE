using DtRent.Application.Exceptions;
using DtRent.Application.ThueXe;
using DtRent.Application.TruyVan;
using DtRent.Domain.Exceptions;
using DtRent.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DtRent.Web.Controllers;

public class PhieuController : Controller
{
    private static readonly TimeSpan MuiGioVietNam = TimeSpan.FromHours(7);

    private readonly ThueXeDichVu _thueXe;
    private readonly XeTruyVanDichVu _xeTruyVan;
    private readonly PhieuTruyVanDichVu _phieuTruyVan;

    public PhieuController(
        ThueXeDichVu thueXe,
        XeTruyVanDichVu xeTruyVan,
        PhieuTruyVanDichVu phieuTruyVan)
    {
        _thueXe = thueXe;
        _xeTruyVan = xeTruyVan;
        _phieuTruyVan = phieuTruyVan;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var danhSach = await _phieuTruyVan.LayDanhSachAsync(ct);
        return View(danhSach.Select(PhieuDongVm.Tu).ToList());
    }

    public async Task<IActionResult> ChiTiet(long id, CancellationToken ct)
    {
        try
        {
            return View(PhieuChiTietVm.Tu(await _phieuTruyVan.LayChiTietAsync(id, ct)));
        }
        catch (KhongTimThayException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Lap(int xeId, CancellationToken ct)
    {
        XeChiTietVm xe;
        try
        {
            xe = XeChiTietVm.Tu(await _xeTruyVan.LayChiTietAsync(xeId, ct));
        }
        catch (KhongTimThayException)
        {
            return NotFound();
        }

        var form = new LapPhieuForm
        {
            XeId = xeId,
            SoNgayThue = 1,
            ThoiDiemNhan = BayGioVietNam()
        };
        return View(await DungVmAsync(form, xe, null, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lap(
        [Bind(Prefix = nameof(LapPhieuVm.Form))] LapPhieuForm form, CancellationToken ct)
    {
        if (form.XeId is null)
            return BadRequest();

        XeChiTietVm xe;
        try
        {
            xe = XeChiTietVm.Tu(await _xeTruyVan.LayChiTietAsync(form.XeId.Value, ct));
        }
        catch (KhongTimThayException)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
            return View(await DungVmAsync(form, xe, null, ct));

        try
        {
            // Giá trị nhập từ ô datetime-local là giờ Việt Nam.
            var thoiDiemNhan = new DateTimeOffset(
                DateTime.SpecifyKind(form.ThoiDiemNhan!.Value, DateTimeKind.Unspecified), MuiGioVietNam);

            var ketQua = await _thueXe.LapPhieuThueAsync(new LapPhieuThueYeuCau(
                form.KhachHangId!.Value,
                form.XeId.Value,
                form.NhanVienId!.Value,
                thoiDiemNhan,
                form.SoNgayThue!.Value,
                string.IsNullOrWhiteSpace(form.GhiChu) ? null : form.GhiChu.Trim()), ct);

            TempData["ThongBao"] = $"Đã lập phiếu thuê {ketQua.MaPhieu}.";
            return RedirectToAction(nameof(ChiTiet), new { id = ketQua.PhieuThueId });
        }
        catch (NghiepVuThueXeException ex)
        {
            return View(await DungVmAsync(form, xe, ex.Message, ct));
        }
    }

        [HttpGet]
    public async Task<IActionResult> TraXe(long id, CancellationToken ct)
    {
        PhieuChiTietVm phieu;
        try
        {
            phieu = PhieuChiTietVm.Tu(await _phieuTruyVan.LayChiTietAsync(id, ct));
        }
        catch (KhongTimThayException)
        {
            return NotFound();
        }

        var form = new TraXeForm
        {
            PhieuThueId = id,
            ThoiDiemTra = BayGioVietNam(),
            SoKmTra = phieu.SoKmNhan
        };
        return View(await DungTraXeVmAsync(form, phieu, null, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TraXe(
        [Bind(Prefix = nameof(TraXeVm.Form))] TraXeForm form, CancellationToken ct)
    {
        PhieuChiTietVm phieu;
        try
        {
            phieu = PhieuChiTietVm.Tu(await _phieuTruyVan.LayChiTietAsync(form.PhieuThueId, ct));
        }
        catch (KhongTimThayException)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
            return View(await DungTraXeVmAsync(form, phieu, null, ct));

        try
        {
            // Giá trị nhập từ ô datetime-local là giờ Việt Nam.
            var thoiDiemTra = new DateTimeOffset(
                DateTime.SpecifyKind(form.ThoiDiemTra!.Value, DateTimeKind.Unspecified), MuiGioVietNam);

            await _thueXe.TraXeAsync(new TraXeYeuCau(
                form.PhieuThueId,
                form.NhanVienId!.Value,
                thoiDiemTra,
                form.SoKmTra!.Value,
                form.XeHuHong,
                form.TinhTrangPinTra,
                string.IsNullOrWhiteSpace(form.GhiChu) ? null : form.GhiChu.Trim()), ct);

            TempData["ThongBao"] = "Đã ghi nhận trả xe và tính tiền.";
            return RedirectToAction(nameof(ChiTiet), new { id = form.PhieuThueId });
        }
        catch (NghiepVuThueXeException ex)
        {
            return View(await DungTraXeVmAsync(form, phieu, ex.Message, ct));
        }
    }
    private async Task<LapPhieuVm> DungVmAsync(
        LapPhieuForm form, XeChiTietVm xe, string? loi, CancellationToken ct)
    {
        var khach = await _phieuTruyVan.LayKhachHangAsync(ct);
        var nhanVien = await _phieuTruyVan.LayNhanVienAsync(ct);

        return new LapPhieuVm
        {
            Form = form,
            Xe = xe,
            LoiNghiepVu = loi,
            KhachHang = khach
                .Select(k => new SelectListItem(
                    $"{k.HoTen} ({k.MaKhach}, {k.SoDienThoai}, {k.TenHang})", k.Id.ToString()))
                .ToList(),
            NhanVien = nhanVien
                .Select(n => new SelectListItem($"{n.HoTen} ({n.MaNhanVien})", n.Id.ToString()))
                .ToList()
        };
    }

        private async Task<TraXeVm> DungTraXeVmAsync(
        TraXeForm form, PhieuChiTietVm phieu, string? loi, CancellationToken ct)
    {
        var nhanVien = await _phieuTruyVan.LayNhanVienAsync(ct);

        return new TraXeVm
        {
            Form = form,
            Phieu = phieu,
            LoiNghiepVu = loi,
            NhanVien = nhanVien
                .Select(n => new SelectListItem($"{n.HoTen} ({n.MaNhanVien})", n.Id.ToString()))
                .ToList()
        };
    }

    private static DateTime BayGioVietNam()
    {
        var bayGio = DateTimeOffset.UtcNow.ToOffset(MuiGioVietNam).DateTime;
        return new DateTime(bayGio.Year, bayGio.Month, bayGio.Day, bayGio.Hour, bayGio.Minute, 0);
    }
}