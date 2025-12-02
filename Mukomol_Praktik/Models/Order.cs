using System;
using System.Collections.Generic;

namespace Mukomol_Praktik;

public partial class Order
{
    public int IdOrder { get; set; }

    public int IdPartner { get; set; }

    public DateOnly DateOrder { get; set; }

    public DateOnly? DateShipping { get; set; }

    public string StatusOrder { get; set; } = null!;

    public string? CommentOrder { get; set; }

    public virtual Partner IdPartnerNavigation { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
