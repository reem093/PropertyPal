using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Application
{
    public int ApplicationId { get; set; }

    public string TenantId { get; set; } = null!;

    public int UnitId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Unit Unit { get; set; } = null!;
}
