using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class MaintenanceUpdate
{
    public int UpdateId { get; set; }

    public int RequestId { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual MaintenanceRequest Request { get; set; } = null!;
}
