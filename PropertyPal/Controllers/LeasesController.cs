using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;
using System.Security.Claims;

namespace PropertyPal.Controllers;

// Leases are useful for more than one role:
// - PropertyManager can review every lease and record payments.
// - Tenant can review only their own lease records.
// - MaintenanceStaff can view lease/unit details when handling requests.
[Authorize(Roles = "PropertyManager,Tenant,MaintenanceStaff")]
public class LeasesController : Controller
{
    private readonly PropertyDbContext _context;

    public LeasesController(PropertyDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, string status = "active", string? paymentFilter = "all")
    {
        // Start with the related data the page needs. Include prevents extra database calls
        // when the Razor view displays unit, property, and payment information for each lease.
        var leasesQuery = _context.Leases
            .Include(l => l.Unit)
                .ThenInclude(u => u.Property)
            .Include(l => l.Payments)
            .AsQueryable();

        // Tenants should not see other tenants' lease information. Managers and maintenance
        // staff can see the wider lease list because they need it for operations and support.
        if (User.IsInRole("Tenant") && !User.IsInRole("PropertyManager") && !User.IsInRole("MaintenanceStaff"))
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            leasesQuery = leasesQuery.Where(l => l.TenantId == currentUserId);
        }

        // Status filter controls whether the list shows active leases, inactive leases, or both.
        // The default is active because those are the records users normally need first.
        if (status == "active")
        {
            leasesQuery = leasesQuery.Where(l => l.IsActive);
        }
        else if (status == "inactive")
        {
            leasesQuery = leasesQuery.Where(l => !l.IsActive);
        }

        // The search box checks common values a user may know: unit number, property name,
        // property location, and tenant id. EF.Functions.Like makes SQL Server do the matching.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            leasesQuery = leasesQuery.Where(l =>
                EF.Functions.Like(l.Unit.UnitNumber, term) ||
                EF.Functions.Like(l.Unit.Property.Name, term) ||
                EF.Functions.Like(l.Unit.Property.Location, term) ||
                EF.Functions.Like(l.TenantId, term));
        }

        // Payment filter helps managers quickly find leases with no payment records or overdue
        // unpaid payments. Tenants can also use it to understand their own payment situation.
        if (paymentFilter == "with-payments")
        {
            leasesQuery = leasesQuery.Where(l => l.Payments.Any());
        }
        else if (paymentFilter == "without-payments")
        {
            leasesQuery = leasesQuery.Where(l => !l.Payments.Any());
        }
        else if (paymentFilter == "overdue")
        {
            var today = DateTime.Today;
            leasesQuery = leasesQuery.Where(l => l.Payments.Any(p => p.PaidDate == null && p.DueDate < today));
        }

        // Keep the selected filters in ViewData so the Razor page can keep the form values
        // after the user submits a search or filter request.
        ViewData["Search"] = search;
        ViewData["Status"] = status;
        ViewData["PaymentFilter"] = paymentFilter;

        var leases = await leasesQuery
            .OrderBy(l => l.Unit.Property.Name)
            .ThenBy(l => l.Unit.UnitNumber)
            .ThenByDescending(l => l.StartDate)
            .ToListAsync();

        return View(leases);
    }
}
