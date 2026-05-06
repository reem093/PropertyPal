using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Amenity
{
    public int AmenityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
