using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PropertyPal.Api.Data;
using PropertyPal.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyPal.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PropertyDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // 1. Create roles
            string[] roleNames = { "PropertyManager", "Tenant", "MaintenanceStaff" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // 2. Seed Users
            // Property Manager
            var manager = await userManager.FindByEmailAsync("manager@propertypal.com");
            if (manager == null)
            {
                manager = new ApplicationUser
                {
                    UserName = "manager@propertypal.com",
                    Email = "manager@propertypal.com",
                    FirstName = "John",
                    LastName = "Manager",
                    IsAvailable = true
                };
                await userManager.CreateAsync(manager, "Manager123!");
                await userManager.AddToRoleAsync(manager, "PropertyManager");
            }

            // Tenant 1
            var tenant1 = await userManager.FindByEmailAsync("tenant1@example.com");
            if (tenant1 == null)
            {
                tenant1 = new ApplicationUser
                {
                    UserName = "tenant1@example.com",
                    Email = "tenant1@example.com",
                    FirstName = "Alice",
                    LastName = "Johnson",
                    IsAvailable = true
                };
                await userManager.CreateAsync(tenant1, "Tenant123!");
                await userManager.AddToRoleAsync(tenant1, "Tenant");
            }

            // Tenant 2
            var tenant2 = await userManager.FindByEmailAsync("tenant2@example.com");
            if (tenant2 == null)
            {
                tenant2 = new ApplicationUser
                {
                    UserName = "tenant2@example.com",
                    Email = "tenant2@example.com",
                    FirstName = "Bob",
                    LastName = "Smith",
                    IsAvailable = true
                };
                await userManager.CreateAsync(tenant2, "Tenant123!");
                await userManager.AddToRoleAsync(tenant2, "Tenant");
            }

            // Maintenance Staff 1
            var staff1 = await userManager.FindByEmailAsync("staff1@propertypal.com");
            if (staff1 == null)
            {
                staff1 = new ApplicationUser
                {
                    UserName = "staff1@propertypal.com",
                    Email = "staff1@propertypal.com",
                    FirstName = "Mike",
                    LastName = "Repair",
                    IsAvailable = true
                };
                await userManager.CreateAsync(staff1, "Staff123!");
                await userManager.AddToRoleAsync(staff1, "MaintenanceStaff");
            }

            // 3. Skills (for maintenance staff)
            if (!context.Skills.Any())
            {
                context.Skills.AddRange(
                    new Skill { Name = "Plumbing" },
                    new Skill { Name = "Electrical" },
                    new Skill { Name = "HVAC" },
                    new Skill { Name = "Carpentry" }
                );
                await context.SaveChangesAsync();
            }

            // Assign skills to staff
            var plumbing = context.Skills.FirstOrDefault(s => s.Name == "Plumbing");
            if (plumbing != null && !context.StaffSkills.Any(ss => ss.StaffId == staff1.Id && ss.SkillId == plumbing.SkillId))
            {
                context.StaffSkills.Add(new StaffSkill { StaffId = staff1.Id, SkillId = plumbing.SkillId });
                await context.SaveChangesAsync();
            }

            // 4. Amenities
            if (!context.Amenities.Any())
            {
                context.Amenities.AddRange(
                    new Amenity { Name = "Swimming Pool" },
                    new Amenity { Name = "Gym" },
                    new Amenity { Name = "Parking" },
                    new Amenity { Name = "Balcony" }
                );
                await context.SaveChangesAsync();
            }

            // 5. Properties
            if (!context.Properties.Any())
            {
                context.Properties.Add(new Property
                {
                    ManagerId = manager.Id,
                    Name = "Sunset Apartments",
                    Location = "123 Main St, Cityville",
                    Description = "Luxury apartments with great views"
                });
                await context.SaveChangesAsync();
            }

            var property = context.Properties.FirstOrDefault();

            // 6. Units
            if (property != null && !context.Units.Any())
            {
                context.Units.AddRange(
                    new Unit
                    {
                        PropertyId = property.PropertyId,
                        UnitNumber = "101",
                        RentAmount = 1200m,
                        Type = 1,   // 1 = Apartment
                        Status = 0, // 0 = Available
                        Size = 75.5m
                    },
                    new Unit
                    {
                        PropertyId = property.PropertyId,
                        UnitNumber = "102",
                        RentAmount = 1350m,
                        Type = 1,
                        Status = 1, // 1 = Occupied
                        Size = 85.0m
                    }
                );
                await context.SaveChangesAsync();
            }

            // Link amenities to units (optional)
            var unit101 = context.Units.FirstOrDefault(u => u.UnitNumber == "101");
            var parking = context.Amenities.FirstOrDefault(a => a.Name == "Parking");
            if (unit101 != null && parking != null && !context.Set<Dictionary<string, object>>("UnitAmenity").Any())
            {
                // Use raw SQL for simplicity, or you can add via navigation properties
                await context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO UnitAmenities (UnitId, AmenityId) VALUES ({0}, {1})",
                    unit101.UnitId, parking.AmenityId);
            }

            // 7. Applications (for occupied unit)
            var unit102 = context.Units.FirstOrDefault(u => u.UnitNumber == "102");
            if (unit102 != null && !context.Applications.Any())
            {
                context.Applications.Add(new Application
                {
                    TenantId = tenant1.Id,
                    UnitId = unit102.UnitId,
                    Status = 1, // Approved
                    CreatedAt = DateTime.Now.AddDays(-30)
                });
                await context.SaveChangesAsync();
            }

            // 8. Lease for the approved application
            var application = context.Applications.FirstOrDefault();
            if (application != null && !context.Leases.Any())
            {
                context.Leases.Add(new Lease
                {
                    TenantId = tenant1.Id,
                    UnitId = unit102.UnitId,
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddMonths(11),
                    IsActive = true
                });
                await context.SaveChangesAsync();
            }

            // 9. Maintenance requests
            if (!context.MaintenanceRequests.Any())
            {
                context.MaintenanceRequests.Add(new MaintenanceRequest
                {
                    TenantId = tenant1.Id,
                    UnitId = unit102.UnitId,
                    Category = 1,     // e.g., Plumbing
                    Priority = 2,     // Medium
                    Status = 0,       // Submitted
                    TicketNumber = "REQ-2025-001",
                    Title = "Leaky faucet",
                    Description = "Kitchen faucet dripping",
                    AssignedStaffId = staff1.Id,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    CompletedAt = null
                });
                await context.SaveChangesAsync();
            }

            // 10. Payments
            var lease = context.Leases.FirstOrDefault();
            if (lease != null && !context.Payments.Any())
            {
                context.Payments.Add(new Payment
                {
                    LeaseId = lease.LeaseId,
                    Amount = 1350m,
                    Status = 1,   // Paid
                    DueDate = DateTime.Now.AddDays(-5),
                    PaidDate = DateTime.Now.AddDays(-5),
                    TransactionReference = "TXN-001",
                    CreatedAt = DateTime.Now.AddDays(-5)
                });
                await context.SaveChangesAsync();
            }

            // 11. Notifications (example)
            if (!context.Notifications.Any())
            {
                context.Notifications.Add(new Notification
                {
                    UserId = tenant1.Id,
                    Message = "Your maintenance request #REQ-2025-001 has been assigned.",
                    CreatedAt = DateTime.Now.AddDays(-4)
                });
                await context.SaveChangesAsync();
            }
        }
    }
}