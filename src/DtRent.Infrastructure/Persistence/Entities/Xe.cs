using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class Xe
{
    public int Id { get; set; }

    public string MaXe { get; set; } = null!;

    public short NhomXeId { get; set; }

    public string HangSanXuat { get; set; } = null!;

    public string DongXe { get; set; } = null!;

    public short NamSanXuat { get; set; }

    public string BienSo { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public int SoKmHienTai { get; set; }

    public decimal? TinhTrangPin { get; set; }

    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual AnhXe? AnhXe { get; set; }

    public virtual ICollection<BaoTri> BaoTris { get; set; } = new List<BaoTri>();

    public virtual NhomXe NhomXe { get; set; } = null!;

    public virtual ICollection<PhieuThue> PhieuThues { get; set; } = new List<PhieuThue>();
}
