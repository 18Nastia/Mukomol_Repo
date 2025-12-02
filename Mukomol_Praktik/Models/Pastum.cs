using System;
using System.Collections.Generic;

namespace Mukomol_Praktik.Models;

public partial class Pastum
{
    public int IdPasta { get; set; }

    public string TypePasta { get; set; } = null!;

    public string? Brand { get; set; }

    public int? Packaging { get; set; }

    public string? ImagePasta { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
