using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class UnitImage
{
    public int ImageId { get; set; }

    public int UnitId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;
}
