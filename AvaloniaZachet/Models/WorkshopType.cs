using System;
using System.Collections.Generic;

namespace AvaloniaZachet.Models;

public partial class WorkshopType
{
    public int Id { get; set; }

    public string WorkshopType1 { get; set; } = null!;

    public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
}
