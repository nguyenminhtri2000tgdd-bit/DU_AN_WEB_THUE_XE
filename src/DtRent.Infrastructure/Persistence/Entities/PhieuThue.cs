using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class PhieuThue
{
    public long Id { get; set; }

    public string MaPhieu { get; set; } = null!;

    public long KhachHangId { get; set; }

    public int XeId { get; set; }

    public int NhanVienId { get; set; }

    public DateTime ThoiDiemNhan { get; set; }

    public DateTime ThoiDiemHenTra { get; set; }

    public short SoNgayThue { get; set; }

    public int SoKmNhan { get; set; }

    public decimal? TinhTrangPinNhan { get; set; }

    public decimal DonGiaNgay { get; set; }

    public int? KmToiDaMoiNgay { get; set; }

    public decimal? PhiVuotKm { get; set; }

    public decimal PhiTreGio { get; set; }

    public decimal TienCoc { get; set; }

    public decimal PhanTramGiam { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual NhanVien NhanVien { get; set; } = null!;

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual TraXe? TraXe { get; set; }

    public virtual Xe Xe { get; set; } = null!;
}
