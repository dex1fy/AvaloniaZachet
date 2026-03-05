using System;
using System.Collections.Generic;

namespace AvaloniaZachet.Models;

public partial class Workshop
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int IdWorkshopType { get; set; }

    public int CountPerson { get; set; }

    public virtual WorkshopType IdWorkshopTypeNavigation { get; set; } = null!;

    public virtual ICollection<ProductWorkshop> ProductWorkshops { get; set; } = new List<ProductWorkshop>();
}
