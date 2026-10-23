using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class NhanVien
{
    public int Id { get; set; }

    public string MaNhanVien { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public bool DangHoatDong { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<BaoTri> BaoTris { get; set; } = new List<BaoTri>();

    public virtual ICollection<PhieuThue> PhieuThues { get; set; } = new List<PhieuThue>();

    public virtual TaiKhoan? TaiKhoan { get; set; }

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual ICollection<TraXe> TraXes { get; set; } = new List<TraXe>();
}
