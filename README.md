# Order Management System API

A layered ASP.NET Core Web API for managing customers, products, and orders with authentication, role-based authorization, invoicing, and inventory validation.

This project demonstrates clean architecture principles, service orchestration, and testable business logic suitable for real-world backend systems.

---

## Features

- JWT Authentication & Authorization
- Role-based access (Admin / Customer)
- Order creation workflow
- Inventory validation & stock updates
- Discount calculation
- Payment processing simulation
- Invoice generation
- Global exception handling middleware
- Entity Framework Core (Code First)
- Unit testing (xUnit)

---

## Tech Stack

- ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- SQL Server / InMemory (tests)
- JWT Bearer Authentication
- Swagger / OpenAPI
- xUnit + FluentAssertions

---

## Project Structure

```
OrderManagementSystem
│
├── Auth                → JWT + Password hashing
├── Controllers         → API endpoints
├── Data                → DbContext
├── DTOs                → Request models
├── Middleware          → Global exception handler
├── Models              → Entities
├── Repositories        → Data access layer
├── Services            → Business logic
└── Program.cs          → App configuration
```

Tests project:

```
OrderManagementSystem.Tests
└── Services Tests
```

---

## Roles

| Role     | Permissions |
|----------|-------------|
| Admin    | Manage products, invoices, orders |
| Customer | Create orders, view own orders |

---

## Running Tests

```bash
dotnet test
```

Tests cover:

- Order creation workflow
- Discount logic
- Inventory validation
- Payment processing

---

## Design Notes

- Service layer handles orchestration
- Repositories abstract EF Core
- DTOs protect domain models
- Middleware centralizes error handling
- JWT contains role & user claims

---

## Author

Ahmed Mostafa Matar
