using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class HangGplx
{
    public string Ma { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public virtual ICollection<GplxKhachHang> GplxKhachHangs { get; set; } = new List<GplxKhachHang>();

    public virtual ICollection<NhomXe> NhomXes { get; set; } = new List<NhomXe>();
}
