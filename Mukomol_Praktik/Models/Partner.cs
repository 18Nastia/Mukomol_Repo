using System;
using System.Collections.Generic;

namespace Mukomol_Praktik.Models;

public partial class Partner
{
    public int IdPartner { get; set; }

    public string LogoPartner { get; set; } = null!;

    public string NameCompany { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
