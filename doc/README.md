# HCRM - Hệ thống Quản lý Quan hệ Khách hàng (Customer Relationship Management)

## Tổng quan

HCRM là một hệ thống CRM được xây dựng theo kiến trúc Clean Architecture, sử dụng .NET 8.0 và các công nghệ hiện đại để quản lý khách hàng, khoản vay và thông tin tham chiếu người dùng.

## Kiến trúc dự án

Dự án được tổ chức theo mô hình **Clean Architecture** với các lớp độc lập:

```
HCRM/
├── Presentation/          # Lớp Presentation - API Controllers, Extensions
├── Application/           # Lớp Application - Business Logic, CQRS
├── Infrastructure/        # Lớp Infrastructure - External Services, Mapping
├── Domain/                # Lớp Domain - Entities, DTOs, Constants
└── Persistence/           # Lớp Persistence - Database, Repositories
```

### Cấu trúc các lớp

#### 1. **Presentation Layer**

- **Chức năng**: API Controllers, Extensions, DTOs
- **Các controller**:
  - `AccountController` - Quản lý tài khoản người dùng
  - `AuthController` - Xác thực người dùng (đăng ký, đăng nhập)
  - `LoanController` - Quản lý khoản vay
  - `UserReferenceController` - Quản lý thông tin tham chiếu người dùng
- **Extensions**:
  - `JWTExtension` - Cấu hình JWT Authentication
  - `SwaggerExtension` - Cấu hình Swagger/OpenAPI
- **Công nghệ**: ASP.NET Core Web API, Swagger, JWT Authentication

#### 2. **Application Layer**

- **Chức năng**: Business Logic, CQRS Pattern với MediatR
- **Cấu trúc Features**:
  - `User/` - Đăng ký, đăng nhập, lấy thông tin người dùng
  - `Loan/` - Tạo khoản vay, duyệt khoản vay, lấy thông tin khoản vay, danh sách khoản vay (admin/user), lịch trả nợ
  - `UserReference/` - Tạo và lấy danh sách tham chiếu người dùng
- **Services Interfaces**:
  - `IRabbitMqService` - Interface cho message queue
  - `IRedisService` - Interface cho caching
  - `IHangFireService` - Interface cho background/recurring jobs (Enqueue, Schedule, Recurring)
  - `IRecurringJobRegistrar` - Đăng ký recurring jobs khi startup (Clean Architecture)
- **Công nghệ**: MediatR (CQRS)

#### 3. **Infrastructure Layer**

- **Chức năng**: External Services, AutoMapper, Token Services, Message Queue, Caching
- **Các service**:
  - `HashingService` - Băm mật khẩu (BCrypt)
  - `TokenService` - Tạo và xác thực JWT
  - `MappingService` - Object Mapping với AutoMapper
  - `LoanInterestRateService` - Tính toán lãi suất
  - `DateTimeService` - Xử lý ngày giờ
- **Extensions**:
  - `RabbitMQ/` - RabbitMQ Connection, Consumer, Service
  - `Redis/` - Redis caching service
  - `HangFire/` - Hangfire Service, RecurringJobRegistrar, HangfireDatabaseEnsurer (tự tạo DB Hangfire nếu chưa có)
- **Mappings**: AutoMapper profiles (Loan, User, UserReference, UserRepayment)
- **Công nghệ**: AutoMapper, BCrypt, JWT, Redis, RabbitMQ, Hangfire (SQL Server storage)

#### 4. **Domain Layer**

- **Chức năng**: Entities, DTOs, Constants, Enums
- **Entities**:
  - `User` - Thông tin người dùng
  - `Loan` - Thông tin khoản vay
  - `UserReference` - Thông tin tham chiếu người dùng
  - `UserRepayment` - Lịch trả nợ theo khoản vay
- **Models**:
  - `Common/` - Response, Paging, Config models
  - `DTO/` - Data Transfer Objects cho từng feature

#### 5. **Persistence Layer**

- **Chức năng**: Database Context, Repositories, Migrations
- **Công nghệ**: Entity Framework Core 8.0, SQL Server
- **Repositories**: Generic Repository Pattern, UnitOfWork Pattern

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

### Architecture Patterns

- **MediatR** - CQRS Pattern implementation
- **AutoMapper** - Object-to-object mapping
- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management

### Message Queue

- **RabbitMQ** - Message broker cho async communication
- **RabbitMQ.Client** - .NET client cho RabbitMQ

### Caching

- **StackExchange.Redis** - Redis client cho distributed caching

### Background Jobs

- **Hangfire** - Background jobs (Enqueue, Schedule, Recurring) với SQL Server storage
- **Hangfire.SqlServer** - Persistence cho job queue
- Tự tạo database Hangfire khi chưa có (kèm retry khi chạy Docker Compose)

### DevOps

- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration

## Các tính năng chính

### User Management

- Đăng ký tài khoản (`RegisterCommand`)
- Đăng nhập (`LoginQuery`)
- Lấy thông tin người dùng (`GetUserInfoQuery`)

### Loan Management

- Tạo khoản vay (`CreateLoanCommand`)
- Duyệt khoản vay (`ReviewLoanCommand`)
- Lấy thông tin khoản vay (`GetLoanInfo`)
- Danh sách khoản vay admin (`GetAllLoan`)
- Danh sách khoản vay user (`GetAllUserLoan`)
- Lịch trả nợ theo khoản vay (`GetLoanRepaymentDate`)

### User Reference Management

- Tạo thông tin tham chiếu (`CreateUserReference`)
- Danh sách tham chiếu người dùng (`GetUserReference`)

### Caching & Message Queue

- **Redis Caching**: Get, Set, Remove, Exists operations với TTL support
- **RabbitMQ**: Publish/Subscribe pattern cho async messaging

### Background Jobs (Hangfire)

- **Recurring jobs**: Đăng ký khi startup qua `IRecurringJobRegistrar` (test-hourly, reminder trả nợ 3 ngày/1 ngày/trễ 1 giờ)
- **Dashboard**: `https://localhost:{port}/hangfire` (Development)
- Enqueue (chạy ngay), Schedule (chạy sau delay), Recurring (theo cron)

## API Controllers & Endpoints (v1)

### AuthController
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/v1/auth/register` | Đăng ký tài khoản | No |
| POST | `/api/v1/auth/login` | Đăng nhập | No |

### AccountController
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/v1/account/info` | Lấy thông tin người dùng | Yes |

### LoanController
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/v1/loan/all-admin` | Danh sách khoản vay (Admin) | Yes |
| GET | `/api/v1/loan/all-user` | Khoản vay của người dùng hiện tại | Yes |
| GET | `/api/v1/loan/info` | Chi tiết khoản vay (query: Id) | Yes |
| GET | `/api/v1/loan/repayment` | Lịch trả nợ theo khoản vay (query: Id) | Yes |
| POST | `/api/v1/loan/create` | Tạo khoản vay mới | Yes |
| POST | `/api/v1/loan/review` | Duyệt khoản vay (Admin) | Yes |

### UserReferenceController
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/v1/user-reference/create` | Tạo người tham chiếu | Yes |
| GET | `/api/v1/user-reference/get-all` | Danh sách người tham chiếu | Yes |

## Cấu hình

### API Versioning

- Default version: v1.0
- Version format: URL Segment (e.g., `/api/v1/controller`)
- Swagger hỗ trợ multiple versions

### JWT Configuration

Cấu hình trong `appsettings.Development.json` (section `Jwt`):

```json
{
  "Jwt": {
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001",
    "Key": "your-secret-key",
    "ExpiresIn": 30
  }
}
```

### Database

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=HCRM;User Id=sa;Password=...;TrustServerCertificate=True",
    "BackgroundConnection": "Server=...;Database=Hangfire;User Id=sa;Password=...;TrustServerCertificate=True"
  }
}
```

- **DefaultConnection**: Database ứng dụng (HCRM). EF Core Migrations tạo bảng.
- **BackgroundConnection**: Database Hangfire. Ứng dụng tự tạo database nếu chưa có (Hangfire tạo bảng).

### RabbitMQ Configuration

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Redis Configuration

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "Default",
    "KeyPrefix": "HCRM:"
  }
}
```

## Development Setup

### Yêu cầu

- .NET 8.0 SDK
- SQL Server
- Redis Server (optional)
- RabbitMQ Server (optional)
- Visual Studio / JetBrains Rider / VS Code

### Chạy dự án

1. Clone repository:
   ```bash
   git clone <repository-url>
   cd HCRM
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Cập nhật cấu hình trong `Presentation/appsettings.Development.json`:
   - `ConnectionStrings:DefaultConnection`, `ConnectionStrings:BackgroundConnection` (Hangfire)
   - `Jwt`: Issuer, Audience, Key, ExpiresIn
   - `Redis`, `RabbitMQ` (nếu sử dụng)

4. (Tùy chọn) Chạy migrations thủ công; hoặc để app tự chạy khi start:
   ```bash
   dotnet ef database update --project Persistence --startup-project Presentation
   ```

5. Chạy ứng dụng:
   ```bash
   dotnet run --project Presentation
   ```

### Chạy với Docker Compose

1. Tạo file `.env` ở thư mục gốc (cùng cấp `compose.yaml`):

   ```env
   SA_PASSWORD=YourStrongPassword
   DB_CONNECTION=Server=db,1433;Database=HCRM;User Id=sa;Password=YourStrongPassword;TrustServerCertificate=True
   HF_CONNECTION=Server=db,1433;Database=Hangfire;User Id=sa;Password=YourStrongPassword;TrustServerCertificate=True
   ```

2. Chạy:

   ```bash
   docker compose -f compose.yaml up --build
   ```

3. Ứng dụng tự:
   - **Retry** kết nối SQL (tối đa 15 lần, mỗi 3 giây) vì SQL Server trong container cần ~10–30s mới sẵn sàng.
   - Tạo database **Hangfire** nếu chưa có (Hangfire sau đó tự tạo bảng).
   - Chạy **EF Core Migrate** cho database **HCRM** (tạo DB + bảng nếu chưa có).

**Ports:**

| Service   | Port  | Ghi chú           |
|----------|-------|-------------------|
| app      | 5000  | API               |
| SQL Server | 1433 | DB                |
| Redis    | 6379  | Cache             |
| RabbitMQ | 5672 (AMQP), 15672 (UI) | Message queue |

### Swagger & Hangfire Dashboard

- **Swagger**: Chỉ bật trong Development — `https://localhost:5000/swagger`
- **Hangfire Dashboard**: `https://localhost:5000/hangfire` (xem recurring/scheduled jobs)

## Cấu trúc thư mục chi tiết

```
HCRM/
├── Application/
│   ├── Features/                    # CQRS Commands & Queries
│   │   ├── Loan/
│   │   │   ├── Command/             # CreateLoanCommand, ReviewLoanCommand
│   │   │   └── Query/               # GetAllLoan, GetLoanInfo, GetLoanRepaymentDate
│   │   ├── User/
│   │   │   ├── Command/             # RegisterCommand
│   │   │   └── Query/               # GetUserInfoQuery, LoginQuery
│   │   └── UserReference/
│   │       ├── Command/             # CreateUserReference
│   │       └── Query/               # GetUserReference
│   ├── Repositories/                # Repository Interfaces
│   │   ├── Base/                    # IBaseRepository, IUnitOfWork, etc.
│   │   ├── ILoanRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── IUserReferenceRepository.cs
│   │   └── IUserRepaymentRepository.cs
│   └── Services/                    # Service Interfaces
│       ├── Base/                    # IDateTimeService, ...
│       ├── IRabbitMqService.cs
│       ├── IRedisService.cs
│       ├── IHangFireService.cs
│       └── IRecurringJobRegistrar.cs
│
├── Domain/
│   ├── Constants/                   # AppConstants, AppEnum
│   ├── Entities/                    # User, Loan, UserReference, UserRepayment
│   │   └── Base/                    # BaseEntity
│   └── Models/
│       ├── Common/                  # Response, Paging, Config models
│       └── DTO/                     # Feature-specific DTOs
│           ├── Loan/
│           ├── User/
│           ├── UserReference/
│           └── UserRepayment/
│
├── Infrastructure/
│   ├── Services/                    # Service Implementations
│   │   ├── DateTimeService.cs
│   │   ├── HashingService.cs
│   │   ├── LoanInterestRateService.cs
│   │   ├── MappingService.cs
│   │   └── TokenService.cs
│   ├── Extensions/
│   │   ├── Mappings/                # AutoMapper Profiles
│   │   ├── RabbitMQ/                # RabbitMQ Connection, Consumer, Service
│   │   ├── Redis/                   # Redis Service
│   │   └── HangFire/                # HangFireService, RecurringJobRegistrar, HangfireDatabaseEnsurer
│   ├── ApplicationBuilderExtensions.cs   # UseHangfireDashboard (Clean Arch)
│   └── DependencyInjection.cs
│
├── Persistence/
│   ├── Contexts/                    # DbContext & Configurations
│   ├── Repositories/                # Repository Implementations
│   └── Migrations/                  # EF Core Migrations
│
├── Presentation/
│   ├── Controllers/                 # API Controllers
│   ├── DTOs/                        # API-specific DTOs
│   ├── Extensions/                  # JWT, Swagger Extensions
│   ├── Program.cs                   # Application Entry Point
│   └── appsettings.json             # Configuration
│
├── compose.yaml                     # Docker Compose configuration
├── HCRM.sln                         # Solution file
└── README.md
```

## Response Format

API sử dụng format response chuẩn:

```json
{
  "result": 1,
  "data": { },
  "message": "Success message"
}
```

| Field | Type | Mô tả |
|-------|------|-------|
| result | int | Mã kết quả (SUCCESS = 1, ERROR = 0) |
| data | object | Dữ liệu trả về |
| message | string | Thông báo kết quả |

## Pagination Response

Các API danh sách sử dụng format phân trang:

```json
{
  "result": 1,
  "data": {
    "items": [],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10,
    "message": "Data retrieved successfully"
  },
  "message": "Success"
}
```

## License

Xem file [LICENSE](LICENSE) để biết thêm chi tiết.
