using System.ComponentModel.DataAnnotations;
using PropertyPal.Api.Models;

namespace PropertyPal.ViewModels;

public class MaintenanceCreateVm
{
    [Required] public int UnitId { get; set; }
    [Required, StringLength(120)] public string Title { get; set; } = string.Empty;
    [StringLength(1000)] public string? Description { get; set; }
    [Required] public int Category { get; set; }
    [Required] public int Priority { get; set; }
    public List<Unit> Units { get; set; } = new();
}

public class AssignmentVm
{
    public int RequestId { get; set; }
    [Required] public string StaffId { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class PublicLookupVm
{
    [Required] public string TicketNumber { get; set; } = string.Empty;
    [Required, Phone] public string Phone { get; set; } = string.Empty;
    public string? ResultJson { get; set; }
    public string? Error { get; set; }
}
