using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;
using PropertyPal.Api.Models;   // for ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Use PropertyDbContext from the API project (shared)
builder.Services.AddDbContext<PropertyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Single Identity registration using ApplicationUser and PropertyDbContext
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Set true later if email confirmation needed
})
    .AddRoles<IdentityRole>()          // Enable roles
    .AddEntityFrameworkStores<PropertyDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();