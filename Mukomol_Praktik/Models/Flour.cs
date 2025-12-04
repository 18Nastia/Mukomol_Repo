using System;
using System.Collections.Generic;

namespace Mukomol_Praktik.Models;

public partial class Flour
{
    public int IdFlour { get; set; }

    public string NameFlour { get; set; } = null!;

    public string? Gost { get; set; }

    public string? Gluten { get; set; }

    public string? Idk { get; set; }

    public string? White { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
