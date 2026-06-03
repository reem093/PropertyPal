# Database setup

The solution uses Entity Framework Core with `PropertyDbContext` in `PropertyPal.Api` as the shared data layer. During local development the MVC and API projects call `DbSeeder.SeedAsync(...)`, which uses `EnsureCreatedAsync()` and creates demo roles/users/test data in the LocalDB database configured in `appsettings.json`.

Default local connection string:
`Server=(localdb)\\MSSQLLocalDB;Database=PropertyDB;Trusted_Connection=True;TrustServerCertificate=True;`

For formal submission, generate SQL from Visual Studio Package Manager Console:

```powershell
Update-Database -Project PropertyPal.Api -StartupProject PropertyPal.Api
Script-Migration -Project PropertyPal.Api -StartupProject PropertyPal.Api -Output DatabaseScripts/create_schema.sql
```

Demo credentials are listed in the root README.
