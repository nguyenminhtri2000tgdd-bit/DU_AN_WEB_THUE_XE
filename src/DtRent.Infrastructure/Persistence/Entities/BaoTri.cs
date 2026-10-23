using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class BaoTri
{
    public int Id { get; set; }

    public int XeId { get; set; }

    public int NhanVienId { get; set; }

    public string Loai { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public DateOnly NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public decimal ChiPhi { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual NhanVien NhanVien { get; set; } = null!;

    public virtual Xe Xe { get; set; } = null!;
}
