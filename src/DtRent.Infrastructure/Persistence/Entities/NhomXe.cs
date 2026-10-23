using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class NhomXe
{
    public short Id { get; set; }

    public string Ma { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string LoaiNhienLieu { get; set; } = null!;

    public short SoCho { get; set; }

    public string ChinhSachGia { get; set; } = null!;

    public decimal GiaThueNgay { get; set; }

    public int? KmToiDaMoiNgay { get; set; }

    public decimal? PhiVuotKm { get; set; }

    public decimal PhiTreGio { get; set; }

    public decimal TienCoc { get; set; }

    public virtual ICollection<Xe> Xes { get; set; } = new List<Xe>();

    public virtual ICollection<HangGplx> HangGplxMas { get; set; } = new List<HangGplx>();
}
