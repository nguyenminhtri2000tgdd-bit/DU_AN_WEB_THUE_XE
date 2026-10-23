using System;
using System.Collections.Generic;

namespace DtRent.Infrastructure.Persistence.Entities;

public partial class AnhXe
{
    public int Id { get; set; }

    public int XeId { get; set; }

    public string DuongDan { get; set; } = null!;

    public bool LaAnhChinh { get; set; }

    public virtual Xe Xe { get; set; } = null!;
}
