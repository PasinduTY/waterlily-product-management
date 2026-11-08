# Waterlily Product Management – Project Plan

## 1. Requirement Understanding

The objective is to develop an ASP.NET Core MVC application to manage product information and generate product-related reports.

The application should support:

- Displaying a list of products.
- Creating, updating, and deleting products.
- Storing product information in Microsoft SQL Server.
- Generating reports using SQL Server stored procedures.
- Implementing a service and repository architecture.
- Applying caching in the service layer.
- Using delegates for callback functionality.
- Providing a responsive user interface using Bootstrap, custom CSS, and jQuery.
- Writing unit tests for service-layer CRUD operations using MSTest and mocks.

### Product Information

Each product contains:

| Field      | Description               |
| ---------- | ------------------------- |
| Product ID | Unique product identifier |
| Name       | Product name              |
| Category   | Product category          |
| Price      | Product unit price        |
| Stock      | Available quantity        |

### Required Reports

1. Average product price grouped by category.
2. Category with the highest total inventory value.

For the second report, inventory value is calculated as:

`Total Inventory Value = SUM(Price × Stock)`

This interpretation considers both the unit price and available quantity.

## 2. Proposed Architecture

The application follows the MVC pattern together with the Service and Repository patterns.

### Architecture Flow

Controller → Service → Repository → Entity Framework Core → SQL Server

**Presentation Layer – MVC Controllers and Views**

- Handles incoming HTTP requests.
- Displays product information and reports.
- Performs input validation through view models.
- Delegates business operations to services.

**Service Layer**

- Contains application-level business logic.
- Implements caching.
- Invalidates relevant cache entries after successful product modifications.
- Provides delegate-based callback functionality.

**Repository Layer**

- Handles database access.
- Uses Entity Framework Core for product CRUD operations.
- Executes stored procedures for report data.

**Data Layer**

- Contains the ApplicationDbContext.
- Defines entity configurations.
- Uses Code First migrations to manage the database schema.

## 3. Database Design

The application uses Microsoft SQL Server.

### Products Table

| Column    | Data Type     | Description              |
| --------- | ------------- | ------------------------ |
| ProductId | INT           | Primary key, identity    |
| Name      | NVARCHAR(100) | Product name             |
| Category  | NVARCHAR(100) | Product category         |
| Price     | DECIMAL(18,2) | Product price            |
| Stock     | INT           | Available stock quantity |

Entity Framework Core Code First migrations are used to create and update the database schema.

### Stored Procedures

**1. Average Price by Category**

- Groups products by category.
- Calculates the average product price using `AVG(Price)`.
- Returns the category and average price.

**2. Highest Stock Value Category**

- Groups products by category.
- Calculates total inventory value using `SUM(Price * Stock)`.
- Returns the category with the highest total inventory value.
- Uses category name as a secondary ordering criterion to provide deterministic results when values are equal.

## 4. Caching Strategy

Caching is implemented in the service layer using `IMemoryCache`.

Two generic caching methods are used:

**CachedLong<T>**

- Stores a generic result without a configured expiration time.
- The cached result remains available until it is explicitly removed.
- Suitable for data that is manually invalidated after changes.

**Cached<T>**

- Stores a generic result for a maximum of five minutes.
- The result is automatically removed after the expiration period.

### Cache Invalidation

When a product is successfully created, updated, or deleted:

- The product-list cache is invalidated.
- The affected product cache is invalidated.
- Both report caches are invalidated.

This prevents the application from displaying outdated product and report information after modifications.

## 5. Delegate Implementation

A custom delegate named `ProductCallback` is used:

`public delegate void ProductCallback(Product product);`

The `ProductChanged` event uses this delegate to notify registered callbacks when a product is successfully created, updated, or deleted.

This demonstrates callback functionality without tightly coupling the service to specific callback implementations.

## 6. User Interface

The application uses:

- ASP.NET Core MVC Razor Views.
- Bootstrap for responsive layouts and components.
- Custom CSS for additional styling.
- jQuery for client-side validation.
- Data Annotations for server-side model validation.

The interface includes product listing, create, edit, delete confirmation, and report pages.

## 7. Testing Strategy

MSTest is used as the testing framework, and Moq is used to mock dependencies.

**Service-layer unit tests**

- Verify product CRUD operations.
- Verify behavior when products do not exist.
- Verify cache usage.
- Verify cache invalidation after product changes.
- Verify delegate event notifications.
- Verify report caching behavior.

**Repository integration test**

- Executes the actual SQL Server stored procedures.
- Verifies report calculations and result mapping.
- Uses a database transaction and rollback to restore test data.

The integration test requires a dedicated local development/test database.

## 8. Potential Challenges and Considerations

| Challenge                          | Approach                                                         |
| ---------------------------------- | ---------------------------------------------------------------- |
| Maintaining separation of concerns | Use controller, service, and repository layers                   |
| Stale cached data                  | Invalidate relevant cache entries after successful modifications |
| Database precision                 | Configure product prices as DECIMAL(18,2)                        |
| Stored procedure result mapping    | Use keyless Entity Framework Core result models                  |
| Invalid user input                 | Apply Data Annotations and client-side validation                |
| Reliable report testing            | Use integration tests with controlled test data                  |
| SQL injection risks                | Use fixed stored procedure calls without user-supplied SQL       |

## 9. Implementation Pseudocode

### Product Creation

1. Receive product details from the create form.
2. Validate the submitted model.
3. If validation fails, redisplay the form.
4. Call the product service.
5. Service calls the repository to save the product.
6. Repository saves the changes to SQL Server.
7. Service invalidates relevant caches.
8. Service raises the ProductChanged event.
9. Redirect to the product list.

### Product Update

1. Receive the product identifier and updated details.
2. Validate the submitted model.
3. Retrieve and update the existing product through the service and repository.
4. If the product does not exist, return an appropriate response.
5. Invalidate relevant caches.
6. Raise the ProductChanged event.
7. Redirect to the product list.

### Report Generation

1. User navigates to the Reports page.
2. Controller requests report data from the report service.
3. Service checks whether a cached result exists.
4. If cached data is unavailable, the repository executes the relevant stored procedure.
5. Service caches the result.
6. Controller passes the report data to the view.
7. View displays the calculated results.

## 10. Development Approach

The project is developed incrementally:

1. Create and configure the ASP.NET Core MVC project.
2. Configure Entity Framework Core and SQL Server.
3. Implement product models and database migrations.
4. Implement repository interfaces and implementations.
5. Implement service interfaces and business logic.
6. Implement generic caching and delegate functionality.
7. Implement report stored procedures and repository methods.
8. Develop product CRUD controllers and views.
9. Develop the reports page.
10. Implement unit and integration tests.
11. Review the application, validate functionality, and prepare documentation.

## 11. Expected Outcome

The expected outcome is a functional ASP.NET Core MVC product management application with database-backed CRUD operations, stored-procedure-based reports, service-layer caching, delegate-based callbacks, responsive views, and automated tests.
