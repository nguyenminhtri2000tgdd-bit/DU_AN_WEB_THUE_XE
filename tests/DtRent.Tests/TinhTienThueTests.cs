using DtRent.Domain.Models;
using DtRent.Domain.Pricing;
using Xunit;

namespace DtRent.Tests;

public class TinhTienThueTests
{
    private static decimal TongPhaiTra(NhomXeOto nhom, HangKhachHang hang,
                                       int soNgay, int soKm, TimeSpan tre) =>
        hang.ApDungGiamGia(nhom.TinhTienThue(soNgay, soKm, tre)).TongPhaiTra;

    [Fact]
    public void TC01_VangLai_OtoDien_1Ngay_150Km_KhongVuotKm()
    {
        var tong = TongPhaiTra(DuLieuMau.OtoDien5Cho(), HangKhachHang.VangLai, 1, 150, TimeSpan.Zero);
        Assert.Equal(800_000m, tong);
    }

    [Fact]
    public void TC02_VangLai_OtoDien_1Ngay_350Km_VuotKmTinhPhi()
    {
        // 800.000 + (350 - 200) x 2.000
        var tong = TongPhaiTra(DuLieuMau.OtoDien5Cho(), HangKhachHang.VangLai, 1, 350, TimeSpan.Zero);
        Assert.Equal(1_100_000m, tong);
    }

    [Fact]
    public void TC03_VangLai_OtoDien_2Ngay_Tre2Gio10Phut_LamTronLen3Gio()
    {
        // 2 x 800.000 + 3 x 80.000
        var tong = TongPhaiTra(DuLieuMau.OtoDien5Cho(), HangKhachHang.VangLai, 2, 300, new TimeSpan(2, 10, 0));
        Assert.Equal(1_840_000m, tong);
    }

    [Fact]
    public void TC04_Vang_NhuTC03_GiamGia10Phan_Tram()
    {
        var tong = TongPhaiTra(DuLieuMau.OtoDien5Cho(), HangKhachHang.Vang, 2, 300, new TimeSpan(2, 10, 0));
        Assert.Equal(1_656_000m, tong);
    }

    [Fact]
    public void TC05_Bac_OtoDien_2Ngay_380Km_KhongVuotKm_Giam5Phan_Tram()
    {
        // 2 x 800.000 = 1.600.000, 380 km chưa vượt hạn mức 400 km, giảm 5%
        var tong = TongPhaiTra(DuLieuMau.OtoDien5Cho(), HangKhachHang.Bac, 2, 380, TimeSpan.Zero);
        Assert.Equal(1_520_000m, tong);
    }

    [Fact]
    public void TC06_VangLai_OtoXang_3Ngay_2000Km_KhongGioiHanKm()
    {
        var tong = TongPhaiTra(DuLieuMau.OtoXang5Cho(), HangKhachHang.VangLai, 3, 2000, TimeSpan.Zero);
        Assert.Equal(1_950_000m, tong);
    }

    [Theory]
    [InlineData(-30, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(60, 1)]
    [InlineData(61, 2)]
    [InlineData(130, 3)]
    public void GioTre_LamTronLen(int phutTre, int soGioTinhPhi)
    {
        var ketQua = DuLieuMau.OtoDien5Cho().TinhTienThue(1, 0, TimeSpan.FromMinutes(phutTre));
        Assert.Equal(soGioTinhPhi, ketQua.SoGioTre);
    }

    [Fact]
    public void BangGia_KmToiDaVaPhiVuotKm_PhaiCungCoHoacCungKhong()
    {
        Assert.Throws<ArgumentException>(() => new BangGia(800_000m, 200, null, 0m));
    }
}