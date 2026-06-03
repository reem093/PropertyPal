# Important Code Comments Guide

This file summarizes the important blocks that were commented in the MVC project.
Use it when explaining the project to the doctor or when revising the documentation.

## Program.cs
- Service registration block: connects MVC, Identity, roles, Entity Framework, HttpClient, and SignalR.
- Database block: registers `PropertyDbContext`, which is shared with the API project.
- Identity block: enables login users and role-based access such as `Tenant`, `PropertyManager`, and `MaintenanceStaff`.
- Pipeline block: controls HTTPS, routing, authentication, authorization, static files, MVC routes, Razor Pages, and SignalR hub mapping.
- Seeder block: creates demo users/roles/data at startup so testing is easier.

## LeasesController.cs
- Role access block: allows Property Manager, Tenant, and Maintenance Staff to open the leases page.
- Tenant privacy block: tenants only see leases where `TenantId` matches their logged-in user id.
- Search block: searches by unit number, property name, location, or tenant id.
- Status filter block: filters active, inactive, or all leases.
- Payment filter block: filters leases with payments, without payments, or overdue unpaid payments.

## Views/Leases/Index.cshtml
- Search/filter form: lets all roles search and filter the lease table.
- Empty-state block: shows a helpful message when no records match.
- Table loop: prints one row for each filtered lease.
- Payment action block: only Property Managers see the `Record Payment` button.

## ApplicationsController.cs
- Apply block: lets tenants apply for vacant units.
- Application save block: validates availability and creates the application record.
- Manager decision block: approves/rejects applications; approval creates a lease and marks the unit occupied.

## MaintenanceController.cs
- Create request block: tenants create maintenance tickets only for their active leased units.
- SignalR block: broadcasts ticket changes to the live maintenance board.
- Assign block: managers assign tickets to staff with matching skills.
- Update status block: staff update progress and notify tenants.

## PaymentsController.cs
- Create payment block: pre-fills invoice data for a selected lease.
- Mark paid block: updates status and saves the paid date.

## UnitsController.cs
- Vacant units block: public listing for units available to apply for.
- Create unit block: managers add new rental units and connect them to a property.
