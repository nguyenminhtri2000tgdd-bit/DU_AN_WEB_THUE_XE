using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class GplxKhachHang
{
    public long KhachHangId { get; set; }

    public string HangGplxMa { get; set; } = null!;

    public string SoGplx { get; set; } = null!;

    public DateOnly? NgayHetHan { get; set; }

    public virtual HangGplx HangGplxMaNavigation { get; set; } = null!;

    public virtual KhachHang KhachHang { get; set; } = null!;
}
