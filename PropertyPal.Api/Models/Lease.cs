using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Lease
{
    public int LeaseId { get; set; }

    public string TenantId { get; set; } = null!;

    public int UnitId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Unit Unit { get; set; } = null!;
}
