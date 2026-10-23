using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class KhachHang
{
    public long Id { get; set; }

    public string MaKhach { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? SoCccd { get; set; }

    public string? DiaChi { get; set; }

    public short HangThanhVienId { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<GplxKhachHang> GplxKhachHangs { get; set; } = new List<GplxKhachHang>();

    public virtual HangThanhVien HangThanhVien { get; set; } = null!;

    public virtual ICollection<PhieuThue> PhieuThues { get; set; } = new List<PhieuThue>();
}
