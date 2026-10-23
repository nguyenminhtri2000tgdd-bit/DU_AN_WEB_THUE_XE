using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class TraXe
{
    public long Id { get; set; }

    public long PhieuThueId { get; set; }

    public int NhanVienId { get; set; }

    public DateTime ThoiDiemTraThucTe { get; set; }

    public int SoKmTra { get; set; }

    public int KmDaDi { get; set; }

    public int SoGioTre { get; set; }

    public int KmVuot { get; set; }

    public decimal TienThue { get; set; }

    public decimal PhiVuotKm { get; set; }

    public decimal PhiTre { get; set; }

    public decimal TongTruocGiam { get; set; }

    public decimal TienGiam { get; set; }

    public decimal TongPhaiTra { get; set; }

    public bool XeHuHong { get; set; }

    public decimal? TinhTrangPinTra { get; set; }

    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual NhanVien NhanVien { get; set; } = null!;

    public virtual PhieuThue PhieuThue { get; set; } = null!;
}
