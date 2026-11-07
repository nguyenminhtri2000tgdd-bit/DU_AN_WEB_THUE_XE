using DtRent.Application.Exceptions;
using DtRent.Application.TruyVan;
using DtRent.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DtRent.Web.Controllers;

public class XeController : Controller
{
    private readonly XeTruyVanDichVu _truyVan;

    public XeController(XeTruyVanDichVu truyVan) => _truyVan = truyVan;

    public async Task<IActionResult> Index(string? nhom, string? trangThai, CancellationToken ct)
    {
        var danhSach = await _truyVan.LayDanhSachAsync(nhom, trangThai, ct);

        var vm = new XeDanhSachVm
        {
            Xe = danhSach.Select(XeCardVm.Tu).ToList(),
            NhomXe = await _truyVan.LayNhomXeAsync(ct),
            NhomDangChon = nhom,
            TrangThaiDangChon = trangThai
        };
        return View(vm);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        try
        {
            var chiTiet = await _truyVan.LayChiTietAsync(id, ct);
            return View(XeChiTietVm.Tu(chiTiet));
        }
        catch (KhongTimThayException)
        {
            return NotFound();
        }
    }
}