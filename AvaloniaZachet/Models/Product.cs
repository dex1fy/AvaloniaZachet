using System;
using System.Collections.Generic;

namespace AvaloniaZachet.Models;

public partial class Product
{
    public int Id { get; set; }

    public int IdProductType { get; set; }

    public string Name { get; set; } = null!;

    public string Article { get; set; } = null!;

    public int MinCostPartner { get; set; }

    public int IdMaterialType { get; set; }

    public virtual MaterialType IdMaterialTypeNavigation { get; set; } = null!;

    public virtual ProductType IdProductTypeNavigation { get; set; } = null!;

    public virtual ICollection<ProductWorkshop> ProductWorkshops { get; set; } = new List<ProductWorkshop>();
}
