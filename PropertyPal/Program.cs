using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;
using PropertyPal.Api.Models;   // for ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// Register MVC, Identity, database, SignalR, and other services before the app starts.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Use the shared PropertyDbContext from the API project so MVC and API use the same database schema.
builder.Services.AddDbContext<PropertyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure ASP.NET Identity with roles. Roles are used throughout controllers to protect pages by user type.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Set true later if email confirmation needed
})
    .AddRoles<IdentityRole>()          // Enable roles
    .AddEntityFrameworkStores<PropertyDbContext>();

builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline: errors, security, routing, authentication, authorization, and endpoints.
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
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapHub<PropertyPal.Hubs.MaintenanceHub>("/maintenanceHub");

// Seed demo roles and users at startup so the project has ready test data after the database is created.
using (var scope = app.Services.CreateScope())
{
    await PropertyPal.Api.Data.DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();