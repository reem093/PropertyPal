# PropertyPal - Property Leasing & Maintenance Platform

Brief B implementation for IT8118 Advanced Programming.

## Projects

- `PropertyPal.Api` - ASP.NET Core Web API, shared EF Core data layer, Identity, JWT endpoints, public maintenance lookup, reporting API endpoints, SignalR hub.
- `PropertyPal` - ASP.NET Core MVC application with role-based UI for Tenants, Property Managers, and Maintenance Staff.
- `PropertyPal.Reporting` - Separate read-only reporting client that authenticates to the Web API with JWT and reads reports using HttpClient only.

## Demo accounts

| Role | Email | Password | Notes |
|---|---|---|---|
| Property Manager | manager@propertypal.com | Manager123! | Full access, user/process/report role |
| Tenant | tenant1@example.com | Tenant123! | Has active lease and maintenance ticket |
| Tenant | tenant2@example.com | Tenant123! | Demo tenant |
| Maintenance Staff | staff1@propertypal.com | Staff123! | Plumbing skill |
| Maintenance Staff | staff2@propertypal.com | Staff123! | Electrical skill |

Tenant 1 phone for public lookup: `39990001`.
Sample ticket: `REQ-2025-001`.

## Main features

### Tenant App
- Browse vacant units.
- Submit lease applications.
- Create maintenance requests for active leased units.
- Track maintenance request status.
- Public maintenance lookup page uses `HttpClient` to call the Web API.

### Property Manager
- Manage property units.
- Process applications: Application → Screening → Approved/Rejected → Lease Active.
- Prevent leasing a unit to multiple tenants simultaneously.
- Manage active leases.
- Record payments and flag overdue payments.
- Assign maintenance requests to staff based on matching skill.
- Monitor response time and live maintenance board.

### Maintenance Staff
- View assigned maintenance requests.
- Update lifecycle: Submitted → Assigned → In Progress → Resolved → Closed.

### API endpoints

| Route | Method | Purpose | Auth |
|---|---:|---|---|
| `/api/auth/login` | POST | JWT login | Public |
| `/api/public/maintenance/lookup` | GET | Public ticket lookup by ticket + phone | Public |
| `/api/units/vacant` | GET | Vacant unit list | Public |
| `/api/units` | GET | All units | PropertyManager JWT |
| `/api/maintenance-requests` | GET | Maintenance request list | PropertyManager/MaintenanceStaff JWT |
| `/api/maintenance-requests/{id}` | GET | Maintenance request detail | JWT |
| `/api/reports/occupancy` | GET | Occupancy report | PropertyManager JWT |
| `/api/reports/maintenance-backlog` | GET | Maintenance backlog report | PropertyManager JWT |
| `/api/reports/overdue-payments` | GET | Overdue payment report | PropertyManager JWT |

## Run locally

1. Open `PropertyPal.sln` in Visual Studio 2022.
2. Restore NuGet packages.
3. Set multiple startup projects:
   - `PropertyPal.Api`
   - `PropertyPal`
   - `PropertyPal.Reporting`
4. Run the solution. The database is created and seeded automatically in Development mode.

## Configuration

- MVC public lookup uses `ApiBaseUrl` in `PropertyPal/appsettings.json`.
- Reporting app uses `ApiBaseUrl` and `ReportingLogin` in `PropertyPal.Reporting/appsettings.json`.
- JWT development settings are in `appsettings.json`; production secrets should be configured in Azure App Service settings.
