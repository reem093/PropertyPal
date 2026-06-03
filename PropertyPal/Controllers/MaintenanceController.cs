using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;
using PropertyPal.Api.Models;
using PropertyPal.Hubs;
using PropertyPal.ViewModels;

namespace PropertyPal.Controllers;

[Authorize]
public class MaintenanceController : Controller
{
    private readonly PropertyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHubContext<MaintenanceHub> _hub;
    public MaintenanceController(PropertyDbContext context, UserManager<ApplicationUser> userManager, IHubContext<MaintenanceHub> hub)
    { _context = context; _userManager = userManager; _hub = hub; }

    // Opens the tenant maintenance form and loads only units currently leased by that tenant.
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> Create()
    {
        var tenantId = _userManager.GetUserId(User)!;
        var units = await _context.Leases.Include(l => l.Unit).ThenInclude(u => u.Property).Where(l => l.TenantId == tenantId && l.IsActive).Select(l => l.Unit).ToListAsync();
        return View(new MaintenanceCreateVm { Units = units });
    }

    // Creates a maintenance ticket, validates the tenant owns the selected unit, then broadcasts the update through SignalR.
    [HttpPost, Authorize(Roles = "Tenant"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaintenanceCreateVm vm)
    {
        var tenantId = _userManager.GetUserId(User)!;
        var allowed = await _context.Leases.AnyAsync(l => l.TenantId == tenantId && l.UnitId == vm.UnitId && l.IsActive);
        if (!allowed) ModelState.AddModelError("UnitId", "Choose one of your active leased units.");
        if (!ModelState.IsValid)
        {
            vm.Units = await _context.Leases.Include(l => l.Unit).Where(l => l.TenantId == tenantId && l.IsActive).Select(l => l.Unit).ToListAsync();
            return View(vm);
        }
        var req = new MaintenanceRequest
        {
            TenantId = tenantId,
            UnitId = vm.UnitId,
            Category = vm.Category,
            Priority = vm.Priority,
            Status = 0,
            TicketNumber = $"REQ-{DateTime.Now:yyyyMMddHHmmss}",
            Title = vm.Title,
            Description = vm.Description,
            CreatedAt = DateTime.Now
        };
        _context.MaintenanceRequests.Add(req);
        await _context.SaveChangesAsync();
        await _hub.Clients.All.SendAsync("MaintenanceChanged", req.TicketNumber, "Submitted");
        TempData["Success"] = $"Request submitted. Ticket: {req.TicketNumber}";
        return RedirectToAction(nameof(My));
    }

    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> My()
    {
        var tenantId = _userManager.GetUserId(User)!;
        return View(await _context.MaintenanceRequests.Include(r => r.Unit).Where(r => r.TenantId == tenantId).OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Index()
    {
        var requests = await _context.MaintenanceRequests.Include(r => r.Unit).ThenInclude(u => u.Property).OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(requests);
    }

    // Manager assignment page: loads the request and suggests staff who have the matching maintenance skill.
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Assign(int id)
    {
        var req = await _context.MaintenanceRequests.Include(r => r.Unit).FirstOrDefaultAsync(r => r.RequestId == id);
        if (req == null) return NotFound();
        ViewBag.Request = req;
        ViewBag.Staff = await StaffWithSkill(req.Category).ToListAsync();
        return View(new AssignmentVm { RequestId = id });
    }

    // Saves the selected staff assignment, creates an update history record, notifies staff, and refreshes the live board.
    [HttpPost, Authorize(Roles = "PropertyManager"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(AssignmentVm vm)
    {
        var req = await _context.MaintenanceRequests.FindAsync(vm.RequestId);
        if (req == null) return NotFound();
        req.AssignedStaffId = vm.StaffId;
        req.Status = 1;
        _context.MaintenanceUpdates.Add(new MaintenanceUpdate { RequestId = req.RequestId, UpdatedBy = _userManager.GetUserId(User)!, Notes = vm.Note ?? "Assigned to maintenance staff.", CreatedAt = DateTime.Now });
        _context.Notifications.Add(new Notification { UserId = vm.StaffId, RequestId = req.RequestId, Message = $"New assignment: {req.Title}", CreatedAt = DateTime.Now });
        await _context.SaveChangesAsync();
        await _hub.Clients.All.SendAsync("MaintenanceChanged", req.TicketNumber, "Assigned");
        TempData["Success"] = "Request assigned based on staff skill.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "MaintenanceStaff")]
    public async Task<IActionResult> Assigned()
    {
        var staffId = _userManager.GetUserId(User)!;
        return View(await _context.MaintenanceRequests.Include(r => r.Unit).Where(r => r.AssignedStaffId == staffId).OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    // Maintenance staff use this to move tickets through the workflow and notify the tenant about progress.
    [HttpPost, Authorize(Roles = "MaintenanceStaff"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, int status, string? notes)
    {
        var staffId = _userManager.GetUserId(User)!;
        var req = await _context.MaintenanceRequests.FirstOrDefaultAsync(r => r.RequestId == id && r.AssignedStaffId == staffId);
        if (req == null) return NotFound();
        req.Status = status;
        if (status >= 3) req.CompletedAt = DateTime.Now;
        _context.MaintenanceUpdates.Add(new MaintenanceUpdate { RequestId = req.RequestId, UpdatedBy = staffId, Notes = notes ?? StatusText(status), CreatedAt = DateTime.Now });
        _context.Notifications.Add(new Notification { UserId = req.TenantId, RequestId = req.RequestId, Message = $"Your request {req.TicketNumber} is now {StatusText(status)}.", CreatedAt = DateTime.Now });
        await _context.SaveChangesAsync();
        await _hub.Clients.All.SendAsync("MaintenanceChanged", req.TicketNumber, StatusText(status));
        return RedirectToAction(nameof(Assigned));
    }

    [Authorize(Roles = "PropertyManager,MaintenanceStaff")]
    public async Task<IActionResult> Board() => View(await _context.MaintenanceRequests.Include(r => r.Unit).OrderByDescending(r => r.CreatedAt).ToListAsync());

    // Converts a request category into a skill name, then finds available staff who have that skill.
    private IQueryable<ApplicationUser> StaffWithSkill(int category)
    {
        var skillName = category switch { 1 => "Plumbing", 2 => "Electrical", 3 => "HVAC", 4 => "Carpentry", _ => "Plumbing" };
        var skillIds = _context.Skills.Where(s => s.Name == skillName).Select(s => s.SkillId);
        var staffIds = _context.StaffSkills.Where(ss => skillIds.Contains(ss.SkillId)).Select(ss => ss.StaffId);
        return _context.Users.Where(u => staffIds.Contains(u.Id) && u.IsAvailable);
    }
    public static string StatusText(int value) => value switch { 0 => "Submitted", 1 => "Assigned", 2 => "In Progress", 3 => "Resolved", 4 => "Closed", _ => "Unknown" };
}
