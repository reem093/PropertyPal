using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class MaintenanceAttachment
{
    public int AttachmentId { get; set; }

    public int RequestId { get; set; }

    public string FileUrl { get; set; } = null!;

    public virtual MaintenanceRequest Request { get; set; } = null!;
}
