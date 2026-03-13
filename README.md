# UserAccessManager

A complete ASP.NET Web API solution (.NET 10) for the **User Access Management** system. It handles user management, application role assignment, and access request workflows against a SQL Server database called `UserManagement`.

---

## Repository Structure

```
UserAccessManager/
├── src/
│   ├── api/                              ← .NET 10 Web API solution
│   │   ├── UserAccessManager.API.sln
│   │   ├── UserAccessManager.API/        ← ASP.NET Web API (Controllers, Middleware)
│   │   ├── UserAccessManager.Core/       ← Domain Models, Interfaces, Validators
│   │   └── UserAccessManager.Infrastructure/  ← Dapper Repositories, DI wiring
│   └── database/
│       ├── Tables/                       ← CREATE TABLE scripts
│       ├── StoredProcedures/             ← Stored procedure scripts
│       └── Seeds/                        ← Sample seed data
└── README.md
```

---

## Technology Stack

| Component        | Technology                        |
|------------------|-----------------------------------|
| Framework        | .NET 10 / ASP.NET Core Web API    |
| Data Access      | Dapper + Microsoft.Data.SqlClient |
| Validation       | FluentValidation                  |
| Documentation    | Swashbuckle / Swagger UI          |
| Logging          | Serilog (Console + File sinks)    |
| Hosting          | IIS (in-process via web.config)   |

---

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/users` | List all users (paginated, searchable) |
| GET | `/api/users/{id}` | Get user by ID |
| GET | `/api/users/by-username/{username}` | Get user by username |
| POST | `/api/users` | Create a new user |
| PUT | `/api/users/{id}` | Update user details |
| PATCH | `/api/users/{id}/status` | Activate/deactivate user |
| GET | `/api/staging-users` | List staging users (paginated) |
| GET | `/api/staging-users/{id}` | Get staging user by ID |
| POST | `/api/staging-users/promote/{id}` | Promote staging user to Users table |
| GET | `/api/applications` | List all applications |
| GET | `/api/applications/{id}` | Get application by ID |
| POST | `/api/applications` | Create a new application |
| PUT | `/api/applications/{id}` | Update application |
| GET | `/api/users/{userId}/roles` | Get roles for a user |
| POST | `/api/users/{userId}/roles` | Assign role to user |
| DELETE | `/api/users/{userId}/roles/{appId}` | Remove role from user |
| GET | `/api/access-requests` | List access requests (filterable by status) |
| GET | `/api/access-requests/{id}` | Get access request by ID |
| POST | `/api/access-requests` | Submit access request |
| PATCH | `/api/access-requests/{id}/status` | Update request status |
| POST | `/api/auth/user-roles` | Lookup user roles via stored procedure |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- SQL Server (2016+ for `STRING_SPLIT` support)
- Visual Studio 2022 / VS Code / Rider

### 1. Set Up the Database

Run the SQL scripts in this order:

```
src/database/Tables/Users.sql
src/database/Tables/StaggingUsers.sql
src/database/Tables/ApplicationName.sql
src/database/Tables/UserRoles.sql
src/database/Tables/ApplicationAccessRequests.sql
src/database/StoredProcedures/GetUserRolesByUserName.sql
src/database/StoredProcedures/ManageApplicationAccessRequests.sql
src/database/Seeds/SeedApplicationNames.sql   ← optional seed data
```

### 2. Configure the Connection String

Edit `src/api/UserAccessManager.API/appsettings.json` (or use `appsettings.Development.json` for local):

```json
{
  "ConnectionStrings": {
    "UserManagement": "Server=YOUR_SERVER;Database=UserManagement;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For SQL Server Authentication:
```
Server=YOUR_SERVER;Database=UserManagement;User Id=your_user;Password=your_password;TrustServerCertificate=True;
```

### 3. Build the Solution

```bash
cd src/api
dotnet build
```

### 4. Run Locally

```bash
cd src/api/UserAccessManager.API
dotnet run
```

Swagger UI is available at: `https://localhost:{port}/swagger`

### 5. Configure CORS

Update `AllowedOrigins` in `appsettings.json` to list the URLs of your front-end apps:

```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "https://your-app.com"
  ]
}
```

---

## Deploy to IIS

1. **Publish** the API project:
   ```bash
   cd src/api/UserAccessManager.API
   dotnet publish -c Release -o ./publish
   ```

2. **Copy** the `publish/` folder contents to your IIS site's physical path.

3. **Install** the [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) on the IIS server.

4. In IIS Manager, create a new site (or application) pointing to the published folder. The included `web.config` configures in-process hosting automatically.

5. Set the connection string via IIS environment variables or update `appsettings.json` directly on the server.

---

## Project Architecture

```
Request → Controller → Repository Interface (Core) → Repository Implementation (Infrastructure) → SQL Server
                                                    ↑
                                             DapperContext (singleton)
```

- **UserAccessManager.Core** — Pure domain layer: DTOs, repository interfaces, FluentValidation validators. No infrastructure dependencies.
- **UserAccessManager.Infrastructure** — Dapper-based repository implementations; `DependencyInjection.cs` wires everything up.
- **UserAccessManager.API** — ASP.NET controllers, middleware, Serilog, Swagger, CORS configuration.

---

## Logging

Serilog writes structured logs to:
- **Console** — for development and container environments
- **Rolling file** — `Logs/log-{date}.txt` (30 days retention)

Log levels are configurable in `appsettings.json` under the `Serilog` section.

