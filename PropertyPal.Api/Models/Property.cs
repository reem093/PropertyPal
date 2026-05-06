using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Property
{
    public int PropertyId { get; set; }

    public string ManagerId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
