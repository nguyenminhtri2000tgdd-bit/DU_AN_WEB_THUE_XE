using DtRent.Domain.Enums;
using DtRent.Domain.Exceptions;
using DtRent.Domain.Models;
using DtRent.Domain.Services;
using Xunit;

namespace DtRent.Tests;

public class KiemTraDieuKienThueTests
{
    private static readonly DateTimeOffset Nhan = new(2026, 10, 23, 8, 0, 0, TimeSpan.FromHours(7));
    private static readonly DateTimeOffset HenTra = Nhan.AddDays(2);

    [Fact]
    public void TC07_KhachChiCoGplxMoTo_ThueOtoDien_BiTuChoi()
    {
        var khach = DuLieuMau.Khach(HangKhachHang.VangLai, new GplxKhach("A1", null));
        var xe = DuLieuMau.Xe(DuLieuMau.OtoDien5Cho());

        Assert.Throws<GplxKhongPhuHopException>(() => KiemTraDieuKienThue.KiemTra(khach, xe, Nhan, HenTra));
    }

    [Fact]
    public void TC08_XeDangBaoTri_BiTuChoi()
    {
        var khach = DuLieuMau.Khach(HangKhachHang.VangLai, new GplxKhach("B2", null));
        var xe = DuLieuMau.Xe(DuLieuMau.OtoDien5Cho(), TrangThaiXe.DangBaoTri);

        var loi = Assert.Throws<XeKhongSanSangException>(() => KiemTraDieuKienThue.KiemTra(khach, xe, Nhan, HenTra));
        Assert.Equal("XE_KHONG_SAN_SANG", loi.MaLoi);
    }

    [Fact]
    public void TC09_KhachGplxB1_ThueOtoXangChiNhanB2_BiTuChoi()
    {
        var khach = DuLieuMau.Khach(HangKhachHang.VangLai, new GplxKhach("B1", null));
        var xe = DuLieuMau.Xe(DuLieuMau.OtoXang5Cho());

        Assert.Throws<GplxKhongPhuHopException>(() => KiemTraDieuKienThue.KiemTra(khach, xe, Nhan, HenTra));
    }

    [Fact]
    public void TC10_GplxHetHanTruocNgayHenTra_BiTuChoi()
    {
        // GPLX hết hạn 24/10, nhưng hẹn trả 25/10
        var khach = DuLieuMau.Khach(HangKhachHang.VangLai, new GplxKhach("B2", new DateOnly(2026, 10, 24)));
        var xe = DuLieuMau.Xe(DuLieuMau.OtoDien5Cho());

        Assert.Throws<GplxKhongPhuHopException>(() => KiemTraDieuKienThue.KiemTra(khach, xe, Nhan, HenTra));
    }

    [Fact]
    public void TC11_HenTraKhongSauThoiDiemNhan_BiTuChoi()
    {
        var khach = DuLieuMau.Khach(HangKhachHang.VangLai, new GplxKhach("B2", null));
        var xe = DuLieuMau.Xe(DuLieuMau.OtoDien5Cho());

        Assert.Throws<ThoiGianThueKhongHopLeException>(() => KiemTraDieuKienThue.KiemTra(khach, xe, Nhan, Nhan));
    }

    [Fact]
    public void TC12_KhachDuDieuKien_KhongNemLoi()
    {
        var khach = DuLieuMau.Khach(HangKhachHang.Bac, new GplxKhach("B1", new DateOnly(2031, 5, 1)));
        var xe = DuLieuMau.Xe(DuLieuMau.OtoDien5Cho());

        var loi = Record.Exception(() => KiemTraDieuKienThue.KiemTra(khach, xe, Nhan, HenTra));
        Assert.Null(loi);
    }

    [Fact]
    public void TC13_MayTrangThaiXe_ChoThueRoiNhanVeHuHong()
    {
        var xe = DuLieuMau.Xe(DuLieuMau.OtoDien5Cho());

        xe.BatDauChoThue();
        Assert.Equal(TrangThaiXe.DangChoThue, xe.TrangThai);

        // đang cho thuê thì không cho thuê tiếp
        Assert.Throws<XeKhongSanSangException>(() => xe.BatDauChoThue());

        xe.NhanXeVe(biHuHong: true);
        Assert.Equal(TrangThaiXe.DangBaoTri, xe.TrangThai);

        xe.HoanThanhBaoTri();
        Assert.Equal(TrangThaiXe.SanSang, xe.TrangThai);
    }
}