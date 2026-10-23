using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class ThanhToan
{
    public long Id { get; set; }

    public long PhieuThueId { get; set; }

    public int NhanVienId { get; set; }

    public string LoaiGiaoDich { get; set; } = null!;

    public string PhuongThuc { get; set; } = null!;

    public decimal SoTien { get; set; }

    public string ChiTiet { get; set; } = null!;

    public DateTime ThoiDiem { get; set; }

    public virtual NhanVien NhanVien { get; set; } = null!;

    public virtual PhieuThue PhieuThue { get; set; } = null!;
}
