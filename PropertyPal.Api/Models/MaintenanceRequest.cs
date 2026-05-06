using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class MaintenanceRequest
{
    public int RequestId { get; set; }

    public string TenantId { get; set; } = null!;

    public int UnitId { get; set; }

    public int Category { get; set; }

    public int Priority { get; set; }

    public int Status { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? AssignedStaffId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<MaintenanceAttachment> MaintenanceAttachments { get; set; } = new List<MaintenanceAttachment>();

    public virtual ICollection<MaintenanceUpdate> MaintenanceUpdates { get; set; } = new List<MaintenanceUpdate>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Unit Unit { get; set; } = null!;
}
