using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public int PropertyId { get; set; }

    public string UnitNumber { get; set; } = null!;

    public decimal RentAmount { get; set; }

    public int Type { get; set; }

    public int Status { get; set; }

    public decimal? Size { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    public virtual Property Property { get; set; } = null!;

    public virtual ICollection<UnitImage> UnitImages { get; set; } = new List<UnitImage>();

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
}
