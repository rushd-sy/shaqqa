# Real Estate API

## User / Identity Architecture

This project splits user data across two tables intentionally:

- **AspNetUsers** (Identity) — owns authentication concerns: Email, 
  PhoneNumber, PasswordHash, roles (via AspNetUserRoles), lockout, 
  security stamps, etc.
- **UserProfiles** (Domain `User` entity) — owns app-specific/business 
  data: FullName, IsActive, and navigation to Properties, Favorites, 
  Notifications, and Reports.

These two tables share the same primary key (`UserProfiles.Id == 
AspNetUsers.Id`), forming a 1:1 relationship (shared PK pattern). 
This is intentional, not a coincidence or bug.

**When creating a new user**, always:
1. Create the `ApplicationUser` first via `UserManager.CreateAsync(...)`
2. Create the domain `User` using the **same Guid** returned by step 1
3. Do both within a single transaction/unit of work

Never let EF auto-generate `UserProfiles.Id` — it must always be set 
explicitly to match `AspNetUsers.Id`.

## Database Setup

### Windows (LocalDB)
"ConnectionStrings:DefaultConnection": 
"Server=(localdb)\\MSSQLLocalDB;Database=RealEstateDb;Trusted_Connection=True;TrustServerCertificate=True"

### macOS / Linux (Docker)
1. Run SQL Server container:
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
     -p 1433:1433 --name sql_server_realestate \
     -d mcr.microsoft.com/mssql/server:2022-latest

2. Set the connection string:
   "ConnectionStrings:DefaultConnection":
   "Server=localhost,1433;Database=RealEstateDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"

Set this via `dotnet user-secrets` — do not commit real credentials.
