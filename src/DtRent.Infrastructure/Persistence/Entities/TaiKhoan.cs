using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class TaiKhoan
{
    public int Id { get; set; }

    public int NhanVienId { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public bool DangHoatDong { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual NhanVien NhanVien { get; set; } = null!;
}
