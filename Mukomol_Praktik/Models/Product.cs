using System;
using System.Collections.Generic;

namespace Mukomol_Praktik.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public int? IdFlour { get; set; }

    public int? IdPasta { get; set; }

    public int IdOrder { get; set; }

    public int Amount { get; set; }

    public virtual Flour? IdFlourNavigation { get; set; }

    public virtual Order IdOrderNavigation { get; set; } = null!;

    public virtual Pastum? IdPastaNavigation { get; set; }
}
