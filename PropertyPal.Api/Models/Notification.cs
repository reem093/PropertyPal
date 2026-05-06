using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string UserId { get; set; } = null!;

    public int? RequestId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual MaintenanceRequest? Request { get; set; }
}
