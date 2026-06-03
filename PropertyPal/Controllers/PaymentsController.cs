using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;
using PropertyPal.Api.Models;

namespace PropertyPal.Controllers;

[Authorize(Roles = "PropertyManager")]
public class PaymentsController : Controller
{
    private readonly PropertyDbContext _context;
    public PaymentsController(PropertyDbContext context) => _context = context;
    // Shows all payment records ordered by due date so managers can follow upcoming and overdue payments.
    public async Task<IActionResult> Index() => View(await _context.Payments.Include(p => p.Lease).ThenInclude(l => l.Unit).OrderBy(p => p.DueDate).ToListAsync());

    // Pre-fills a new invoice/payment form for a selected lease.
    public IActionResult Create(int leaseId) => View(new Payment { LeaseId = leaseId, DueDate = DateTime.Today.AddDays(30), CreatedAt = DateTime.Now, Status = 0, TransactionReference = $"INV-{DateTime.Now:yyyyMMddHHmm}" });

    // Saves a payment record. If it is already marked paid, the paid date is saved automatically.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Payment payment)
    {
        if (!ModelState.IsValid) return View(payment);
        payment.CreatedAt = DateTime.Now;
        if (payment.Status == 1) payment.PaidDate = DateTime.Now;
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Quick action used by managers to mark an existing payment as paid.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return NotFound();
        payment.Status = 1;
        payment.PaidDate = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
