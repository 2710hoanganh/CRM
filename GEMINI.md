## ⚡ READ FIRST — Knowledge Base Routing
Use the KB at graphify-out/ before grepping source.

| Question type           | First action                |
|-------------------------|-----------------------------|
| Architecture / flow     | /graphify query "..."       |
| Trace one chain         | /graphify query "..." --dfs |
| Two concepts → path     | /graphify path "A" "B"      |
| Explain one component   | /graphify explain "Node"    |
| Locate file / symbol    | Grep / Glob (skip KB)       |

**Forbidden:** opening 10+ source files to "build a
mental model" — if tempted, you skipped the KB.

# HCRM Development Guidelines

## Project Overview
HCRM (Customer Relationship Management) is a system designed to manage customers, loan applications, and user reference information, utilizing an admin interface for loan reviews/approvals and a customer interface for loan applications. Built using .NET 8.0 and SQL Server following Clean Architecture principles, it integrates Redis for distributed caching, RabbitMQ for asynchronous message queuing, and Hangfire for background/recurring job scheduling.

## Build & Development Commands
- **Run Application Locally**:
  ```powershell
  dotnet run --project Presentation
  ```
- **Apply Database Migrations (EF Core)**:
  ```powershell
  dotnet ef database update --project Persistence --startup-project Presentation
  ```
- **Docker Compose Orchestration**:
  ```powershell
  docker compose -f compose.yaml up --build
  ```
  *(Note: A `.env` file containing `SA_PASSWORD`, `DB_CONNECTION`, and `HF_CONNECTION` must exist in the root folder).*

## Architecture
HCRM is built using Clean Architecture with strict separation of concerns between its layers:
```
+-------------------------------------------------------------+
|                       Presentation                          |
|   (API Controllers, Routing, JWT Auth, Swagger, v1 APIs)    |
+------------------------------+------------------------------+
                               |
                               v  [MediatR Request/Response]
+-------------------------------------------------------------+
|                        Application                          |
|   (Business Logic, CQRS Commands/Queries, Feature Folders)  |
|   Interfaces: IUnitOfWork, IBaseRepository, IRedisService   |
+------------------------------+------------------------------+
            /                     |                     \
           /                      v                      \
          v                       |                       v
   +--------------+      +----------------+      +----------------+
   |    Domain    | <--- |  Persistence   | <--- | Infrastructure |
   | (Entities,   |      | (DbContext, EF |      | (Redis, JWT,   |
   |  DTOs, Enums |      |  Repositories, |      |  RabbitMQ,     |
   |  Constants)  |      |  Migrations)   |      |  Hangfire)     |
   +--------------+      +----------------+      +----------------+
```

### Dependency Injection (DI) Registration
- Repository registrations: [Persistence/DependencyInjection.cs](file:///d:/CRM/Persistence/DependencyInjection.cs)
- Service registrations: [Infrastructure/DependencyInjection.cs](file:///d:/CRM/Infrastructure/DependencyInjection.cs)
- MediatR registrations: [Application/DependencyInjection.cs](file:///d:/CRM/Application/DependencyInjection.cs)

### Routing
API routing is managed in the `Presentation` layer via URL segment-based API versioning (e.g., `/api/v1/[controller]`).

## Critical Patterns (MUST follow)
- **CQRS Pattern via MediatR**: Implement all application logic as distinct Commands and Queries placed inside `Application/Features/` (e.g., [CreateLoanCommand.cs](file:///d:/CRM/Application/Features/Loan/Command/CreateLoanCommand.cs)).
- **Standard API Response Formats**: All controller responses must strictly match the following wrappers:
  - **Standard Response**:
    ```json
    {
      "result": 1, // SUCCESS = 1, ERROR = 0
      "data": {},
      "message": "Result description"
    }
    ```
  - **Pagination Response**:
    ```json
    {
      "result": 1,
      "data": {
        "items": [],
        "pageNumber": 1,
        "pageSize": 10,
        "totalCount": 100,
        "totalPages": 10
      },
      "message": "Data retrieved successfully"
    }
    ```
- **Repository & Unit of Work Isolation**: Interact with databases only through Generic Repository patterns. Never inject `DbContext` directly into Services or Handlers. Use `IUnitOfWork` to commit transactions.
- **Asynchronous Services Integration**: Utilize high-level interface abstractions for external integrations: `IRedisService` for caching, `IRabbitMqService` for messaging, and `IHangFireService` for background tasks.

## Verification Gates (before marking work done)
1. **Build**: Run `dotnet build` from the workspace root and verify that the application compiles successfully with zero warnings or errors.
2. **Tests**: Ensure database schema changes are migrated cleanly by running `dotnet ef database update --project Persistence --startup-project Presentation`. Verify unit tests pass if applicable.
3. **Runs**: Start the API project (`dotnet run --project Presentation`) or containers (`docker compose up`) and verify that connection retries and database instantiation (including automatic Hangfire database creation) complete successfully.
4. **Smoke check**: Navigate to the Swagger UI (`https://localhost:5000/swagger`) and the Hangfire Dashboard (`https://localhost:5000/hangfire`). Manually call test endpoints and verify that responses adhere to the standard envelope format and that jobs trigger properly.

## Code Conventions
- **Naming**: Use `PascalCase` for classes, interfaces (must start with `I`), methods, and public properties. Use `camelCase` for local variables/parameters, and prefix private fields with an underscore (e.g., `_redisService`).
- **File size limits**: Keep classes focused and files strictly under 500 lines.
- **Types**: Use strong types and defined enums (such as `LoanStatus`). Map database entities to DTOs in the Application layer before returning them; do not return entities directly from Controllers.
- **Imports**: Group imports at the top of files starting with `System` and `Microsoft` packages, followed by external dependencies, and finally local project namespaces, sorted alphabetically.

