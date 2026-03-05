using System;
using System.Collections.Generic;

namespace AvaloniaZachet.Models;

public partial class ProductWorkshop
{
    public int Id { get; set; }

    public int IdProduct { get; set; }

    public int IdWorkshops { get; set; }

    public double Time { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual Workshop IdWorkshopsNavigation { get; set; } = null!;
}
