# Waterlily Product Management

An ASP.NET Core MVC application for managing products and generating category-based inventory reports.

## Features

- View all products.
- Create, edit, and delete products.
- Validate product information.
- Generate average product price reports by category.
- Identify the category with the highest total inventory value.
- Use SQL Server stored procedures for report calculations.
- Apply service-layer caching.
- Use delegate-based callback functionality.
- Include automated service-layer and repository tests.

## Technology Stack

| Technology            | Purpose                        |
| --------------------- | ------------------------------ |
| C# / .NET 10          | Application development        |
| ASP.NET Core MVC      | Web application framework      |
| Entity Framework Core | Database access and Code First |
| Microsoft SQL Server  | Relational database            |
| SQL Stored Procedures | Report calculations            |
| Bootstrap             | Responsive UI                  |
| CSS                   | Custom styling                 |
| jQuery                | Client-side validation         |
| MSTest                | Automated testing              |
| Moq                   | Mocking dependencies           |

## Project Structure

```text
waterlily-product-management/
│
├── docs/
│   └── ProjectPlan.md
│
├── WaterlilyProductManagement/
│   ├── Controllers/
│   ├── Data/
│   ├── Database/
│   ├── Interfaces/
│   ├── Models/
│   ├── Repositories/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   ├── wwwroot/
│   ├── appsettings.json
│   └── Program.cs
│
├── WaterlilyProductManagement.Tests/
│   ├── Repositories/
│   └── Services/
│
└── README.md
```

## Architecture

The application uses a layered architecture:

```text
MVC Controllers
      |
      v
Service Layer
      |
      v
Repository Layer
      |
      v
Entity Framework Core
      |
      v
SQL Server
```

Controllers handle HTTP requests and responses. Services contain application logic and caching. Repositories handle database operations.

## Prerequisites

- .NET 10 SDK
- Microsoft SQL Server or SQL Server Express
- Visual Studio 2026 or another .NET-compatible IDE
- SQL Server Management Studio (recommended)

## Setup Instructions

### 1. Clone the repository

```bash
git clone <repository-url>
cd waterlily-product-management
```

### 2. Configure the database connection

Open:

`WaterlilyProductManagement/appsettings.json`

Update the `DefaultConnection` value to match your SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=WaterlilyProductManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

Use your own SQL Server instance name. Do not commit personal credentials or production connection strings.

### 3. Install Entity Framework Core tools

If the EF Core CLI tool is not already installed:

```bash
dotnet tool install --global dotnet-ef --version 10.0.14
```

### 4. Apply database migrations

From the repository root:

```bash
dotnet ef database update --project WaterlilyProductManagement
```

This creates the database schema using the existing Code First migrations.

### 5. Create the stored procedures

Open SQL Server Management Studio and connect to your SQL Server instance.

Select the `WaterlilyProductManagementDb` database and execute:

`WaterlilyProductManagement/Database/StoredProcedures.sql`

This creates or updates the stored procedures required by the Reports page.

### 6. Run the application

From the repository root:

```bash
dotnet run --project WaterlilyProductManagement
```

Open the local URL displayed in the terminal.

The application opens the Products page by default.

## Running Tests

Run the automated tests from the repository root:

```bash
dotnet test WaterlilyProductManagement.Tests
```

The test project includes service-layer unit tests using MSTest and Moq, as well as an integration test that executes the SQL Server stored procedures.

**Important:** The integration test requires the configured database and stored procedures to exist. It temporarily deletes product rows inside a transaction and rolls back the transaction afterward. Run it only against a dedicated local development/test database, not a shared or production database.

## Reports

The application provides two reports:

1. **Average Price by Category** – calculates the average unit price of products in each category.
2. **Highest Stock Value Category** – identifies the category with the highest total inventory value.

Total inventory value is calculated as:

`SUM(Price * Stock)`

## Caching

The service layer implements two generic caching methods:

- `CachedLong<T>` – caches results until explicitly invalidated.
- `Cached<T>` – caches results for up to five minutes.

Product modifications invalidate relevant product and report cache entries.

## Documentation

See [`docs/ProjectPlan.md`](docs/ProjectPlan.md) for the requirement analysis, proposed architecture, database design, caching strategy, testing approach, potential challenges, and implementation pseudocode.
