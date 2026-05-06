using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int RequestId { get; set; }

    public string TenantId { get; set; } = null!;

    public string Message { get; set; } = null!;

    public int Rating { get; set; }

    public virtual MaintenanceRequest Request { get; set; } = null!;
}
