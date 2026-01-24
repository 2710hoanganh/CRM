# HCRM - Hệ thống Quản lý Quan hệ Khách hàng (Customer Relationship Management)

## Tổng quan

HCRM là một hệ thống CRM được xây dựng theo kiến trúc Clean Architecture, sử dụng .NET 8.0 và các công nghệ hiện đại để quản lý khách hàng, khoản vay và thông tin tham chiếu người dùng.

## Kiến trúc dự án

Dự án được tổ chức theo mô hình **Clean Architecture** với các lớp độc lập:

```
HCRM/
├── Presentation/          # Lớp Presentation - API Controllers, Middleware
├── Application/           # Lớp Application - Business Logic, CQRS
├── Infrastructure/        # Lớp Infrastructure - External Services, Mapping
├── Domain/                # Lớp Domain - Entities, DTOs, Constants
└── Persistence/           # Lớp Persistence - Database, Repositories
```

### Cấu trúc các lớp

#### 1. **Presentation Layer**

- **Chức năng**: API Controllers, Middleware, Filters, Extensions
- **Các controller**:
  - `AccountController` - Quản lý tài khoản
  - `AuthController` - Xác thực người dùng
  - `LoanController` - Quản lý khoản vay
  - `UserReferenceController` - Quản lý thông tin tham chiếu người dùng
- **Công nghệ**: ASP.NET Core Web API, Swagger, JWT Authentication

#### 2. **Application Layer**

- **Chức năng**: Business Logic, CQRS Pattern với MediatR
- **Cấu trúc Features**:
  - `User/` - Đăng ký, đăng nhập, lấy thông tin người dùng
  - `Loan/` - Tạo khoản vay, duyệt khoản vay, lấy thông tin khoản vay, danh sách khoản vay (admin/user), lịch trả nợ
  - `UserReference/` - Tạo và lấy danh sách tham chiếu người dùng
- **Công nghệ**: MediatR (CQRS)

#### 3. **Infrastructure Layer**

- **Chức năng**: External Services, AutoMapper, Token Services
- **Các service**:
  - `HashingService` - Băm mật khẩu (BCrypt)
  - `TokenService` - Tạo và xác thực JWT
  - `MappingService` - Object Mapping với AutoMapper
  - `LoanInterestRateService` - Tính toán lãi suất
  - `DateTimeService` - Xử lý ngày giờ (IDateTimeService)
- **Công nghệ**: AutoMapper, BCrypt, JWT, Redis

#### 4. **Domain Layer**

- **Chức năng**: Entities, DTOs, Constants, Enums
- **Entities**:
  - `User` - Thông tin người dùng
  - `Loan` - Thông tin khoản vay
  - `UserReference` - Thông tin tham chiếu người dùng
  - `UserRepayment` - Lịch trả nợ theo khoản vay
- **Models**: DTOs cho các operations

#### 5. **Persistence Layer**

- **Chức năng**: Database Context, Repositories, Migrations
- **Công nghệ**: Entity Framework Core 8.0, SQL Server
- **Repositories**: Generic Repository Pattern, `UserRepaymentRepository`

## Công nghệ sử dụng

### Framework & Runtime

- **.NET 8.0** - Framework chính

### Database

- **Entity Framework Core 8.0** - ORM
- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server provider
- **Migrations** - Code-First database migrations

### Authentication & Authorization

- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT Authentication
- **System.IdentityModel.Tokens.Jwt** - JWT Token handling
- **BCrypt.Net-Next** - Password hashing

### API & Documentation

- **Swashbuckle.AspNetCore (Swagger)** - API documentation
- **Asp.Versioning** - API versioning support
- **Microsoft.AspNetCore.Mvc.Versioning** - Versioning helpers

### Architecture Patterns

- **MediatR 13.0** - CQRS Pattern implementation
- **AutoMapper 12.0** - Object-to-object mapping
- **Repository Pattern** - Data access abstraction

### Caching

- **StackExchange.Redis** - Redis client (nếu cần caching)

### DevOps

- **Docker** - Containerization (compose.yaml)
- **Dockerfile** - Container configuration

## Các tính năng chính

### User Management

- Đăng ký tài khoản (`RegisterCommand`)
- Đăng nhập (`LoginQuery`)
- Lấy thông tin người dùng (`GetUserInfoQuery`)

### Loan Management

- Tạo khoản vay (`CreateLoanCommand`)
- Duyệt khoản vay (`ReviewLoanCommand`)
- Lấy thông tin khoản vay (`GetLoanInfoQuery`)
- Danh sách khoản vay admin (`GetAllLoanQuery`)
- Danh sách khoản vay user (`GetAllUserLoanQuery`)
- Lịch trả nợ theo khoản vay (`GetLoanRepaymentDateQuery`)

### User Reference Management

- Tạo thông tin tham chiếu (`CreateUserReferenceCommand`)
- Danh sách tham chiếu người dùng (`GetUserReferenceQuery`)

## API Controllers & Endpoints (v1)
- `AuthController`
  - POST `/api/v1/auth/register` – Đăng ký
  - POST `/api/v1/auth/login` – Đăng nhập
- `AccountController`
  - GET `/api/v1/account/info` – Lấy thông tin người dùng (Authorize)
- `LoanController`
  - GET `/api/v1/loan/all-admin` – Danh sách khoản vay (Admin)
  - GET `/api/v1/loan/all-user` – Khoản vay của người dùng
  - GET `/api/v1/loan/info` – Chi tiết khoản vay
  - GET `/api/v1/loan/repayment` – Lịch trả nợ theo khoản vay (query: Id)
  - POST `/api/v1/loan/create` – Tạo khoản vay
  - POST `/api/v1/loan/review` – Duyệt khoản vay (Admin)
- `UserReferenceController`
  - POST `/api/v1/user-reference/create` – Tạo người tham chiếu
  - GET `/api/v1/user-reference/get-all` – Danh sách người tham chiếu

## Cấu hình

### API Versioning

- Default version: v1.0
- Version format: URL Segment (e.g., `/api/v1/controller`)
- Swagger hỗ trợ multiple versions

### JWT Configuration

- JWT được cấu hình trong `appsettings.json`
- Bearer token authentication

### Database

- SQL Server database
- Entity Framework Migrations được sử dụng để quản lý schema

## Development Setup

### Yêu cầu

- .NET 8.0 SDK
- SQL Server
- Visual Studio / JetBrains Rider / VS Code

### Chạy dự án

1. Restore packages: `dotnet restore`
2. Cập nhật database connection string trong `appsettings.json`
3. Chạy migrations: `dotnet ef database update --project Persistence --startup-project Presentation`
4. Chạy ứng dụng: `dotnet run --project Presentation`

### Swagger

- Swagger UI chỉ hoạt động trong môi trường Development
- Truy cập: `https://localhost:{port}/swagger`

## Cấu trúc thư mục chi tiết

```
HCRM/
├── Application/
│   ├── Features/              # CQRS Commands & Queries
│   │   ├── Loan/
│   │   ├── User/
│   │   └── UserReference/
│   ├── Repositories/          # Repository Interfaces
│   └── Services/              # Application Services
│
├── Domain/
│   ├── Entities/              # Domain Entities
│   ├── Models/                # DTOs, Common Models
│   └── Constants/             # App Constants, Enums
│
├── Infrastructure/
│   ├── Services/              # External Services
│   ├── Extensions/            # Extension Methods
│   └── ScheduledJobs/         # Background Jobs
│
├── Persistence/
│   ├── Contexts/              # DbContext & Configurations
│   ├── Repositories/          # Repository Implementations
│   └── Migrations/            # EF Core Migrations
│
└── Presentation/
    ├── Controllers/           # API Controllers
    ├── Extensions/            # Startup Extensions
    ├── Filters/               # Action Filters
    └── Middlewares/           # Custom Middlewares
```

## License

Xem file [LICENSE](LICENSE) để biết thêm chi tiết.
