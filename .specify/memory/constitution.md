# HCRM Constitution

> Hệ thống Quản lý Quan hệ Khách hàng & Khoản vay — .NET 8.0 Clean Architecture

## I. Kiến trúc & Tổ chức Dự án

### 1. Clean Architecture — 5 Layer (BẮT BUỘC)

Dự án tuân thủ nghiêm ngặt mô hình Clean Architecture với 5 project riêng biệt trong solution `HCRM.sln`:

| Layer | Project | Phụ thuộc vào | Vai trò |
|-------|---------|---------------|---------|
| **Domain** | `Domain/` | Không phụ thuộc | Entities, Enums, Constants, DTOs, Config models |
| **Application** | `Application/` | Domain | Business logic (CQRS), Repository interfaces, Service interfaces |
| **Infrastructure** | `Infrastructure/` | Application, Domain | Service implementations, AutoMapper, External integrations (Redis, RabbitMQ, Hangfire, VNPay) |
| **Persistence** | `Persistence/` | Application, Domain | DbContext, Repository implementations, EF Core Migrations, Raw queries |
| **Presentation** | `Presentation/` | Application, Infrastructure, Persistence | API Controllers, DTOs riêng API, Program.cs, Extensions (JWT, Swagger) |

**Quy tắc chiều phụ thuộc:**
- Domain KHÔNG được tham chiếu bất kỳ project nào khác.
- Application chỉ tham chiếu Domain.
- Infrastructure và Persistence tham chiếu Application + Domain.
- Presentation tham chiếu tất cả để thiết lập DI container.

### 2. CQRS Pattern với MediatR (BẮT BUỘC)

- Mọi business logic phải nằm trong `Application/Features/{FeatureName}/Command/` hoặc `Application/Features/{FeatureName}/Query/`.
- Mỗi file chứa cả class Request (`IRequest<T>`) và Handler (`IRequestHandler<TRequest, TResult>`) tương ứng.
- **Controllers chỉ được phép**: nhận request, trích xuất thông tin từ JWT Claims, gọi `_mediator.Send(...)`, và bọc kết quả vào `Response<T>`.
- KHÔNG đặt logic nghiệp vụ trong Controllers, Repositories hoặc Services infrastructure.

### 3. Repository Pattern & Unit of Work (BẮT BUỘC)

- Interface được khai báo tại `Application/Repositories/` (kế thừa `IBaseRepository<T>` generic).
- Implementation nằm tại `Persistence/Repositories/`.
- Sử dụng `IUnitOfWork` để quản lý transaction:
  - `BeginTransactionAsync()` / `CommitTransactionAsync()` / `RollbackTransactionAsync()` cho explicit transaction.
  - `ExecuteInTransactionAsync<T>()` cho execution strategy (retry on failure với SQL Server).
  - `SaveChangesAsync()` cho các thao tác đơn giản.

---

## II. Coding Conventions & Patterns

### 1. Entity Conventions

- Tất cả entities kế thừa `BaseEntity` (chứa `Id`, `CreatedAt`, `UpdatedAt`, `DeletedAt`).
- Các trường Enum (Status, Role, Gender, LoanStatus...) được lưu dưới dạng `int` trong entity, cast từ enum khi sử dụng.
- Navigation properties sử dụng `= null!` cho required references, `= new List<T>()` cho collections.
- Entities nằm tại `Domain/Entities/`, entity base tại `Domain/Entities/Base/`.

### 2. Enum Registry (Tập trung)

Tất cả enums được định nghĩa tập trung trong `Domain/Constants/AppEnum.cs` (namespace `Domain.Constants.AppEnum`):

| Enum | Giá trị | Mục đích |
|------|---------|----------|
| `Role` | Admin=0, Manager=1, User=2 | Phân quyền người dùng |
| `LoanStatus` | Pending=0, Approved=1, Rejected=2, Disbursing=3, Active=4, Completed=5, BadDebt=6, Cancelled=7, Overdue=8 | Vòng đời khoản vay |
| `TransactionType` | Disbursement=0, Repayment=1, EarlySettlement=2, PenaltyFee=3 | Loại giao dịch tài chính |
| `UserRepatmentStatus` | Pending=0, Due=1, Overdue=2, Paid=3, Partial=4 | Trạng thái kỳ trả nợ |
| `LoanTerm` | 1, 3, 6, 12, 24, 36, 48 tháng | Kỳ hạn vay cho phép |
| `LoanRate` | BaseRate=0, SpecialRate=1, PremiumRate=2 | Hạng lãi suất theo cấp tài khoản |
| `NotificationType` | Loan=0, Payment=1, Reminder=2, Other=3 | Phân loại thông báo |
| `Queue` | CreateNotification=0, ConsumeNotification=1 | RabbitMQ queue names |

### 3. Error Messages (Tập trung)

- Tất cả error messages phải được khai báo dưới dạng `const string` trong class `Domain.Constants.Error`.
- KHÔNG hardcode chuỗi lỗi trực tiếp trong Controllers hay Handlers (trừ exception message nội bộ).

### 4. Constants (Tập trung)

- Hằng số nghiệp vụ nằm trong `Domain.Constants.AppConstants.AppConstants`:
  - Lãi suất cơ bản: `BaseLoanRate = 0.02M`, `SpecialLoanRate = 0.015M`, `PremiumLoanRate = 0.01M`.
  - Hệ số rủi ro: `RiskStep = 0.12m`.
  - Queue mapping dictionary.

### 5. Response Format (BẮT BUỘC)

Mọi API trả về đều sử dụng wrapper `Response<T>` (namespace `Domain.Models.Common`):
```json
{
  "result": 1,       // ResponseResult enum: SUCCESS=1, ERROR=0
  "data": { },       // T? nullable
  "message": "...",  // string? nullable
  "errors": null     // IDictionary<string, string[]>? cho validation errors
}
```

Danh sách phân trang sử dụng `Paged<T>`:
```json
{
  "result": 1,
  "data": {
    "pageNumber": 0,
    "pageSize": 10,
    "totalRecords": 100,
    "totalPages": 10,
    "nextPage": true,
    "previousPage": false,
    "data": [ ... ]
  }
}
```

**Lưu ý**: `PageNumber` bắt đầu từ `0` (zero-indexed), kiểu `ulong`.

### 6. Dependency Injection (Pattern chuẩn)

Mỗi layer có class `DependencyInjection` riêng với extension method:
- `Application/DependencyInjection.cs` → `services.AddApplication()` (MediatR)
- `Infrastructure/DependencyInjection.cs` → `services.AddInfrastructure(configuration)` (Services, AutoMapper, Hangfire, Redis, RabbitMQ, VNPay)
- `Persistence/DependencyInjection.cs` → `services.AddPersistence(configuration)` (DbContext, Repositories, UnitOfWork)

Thứ tự đăng ký trong `Program.cs`:
1. `AddApplication()` → 2. `AddInfrastructure(config)` → 3. `AddPersistence(config)`

### 7. AutoMapper Profiles

- Profiles nằm tại `Infrastructure/Extensions/Mappings/`.
- Đăng ký tự động qua `Assembly.GetExecutingAssembly()` trong Infrastructure DI.
- Sử dụng thông qua interface `IAutoMapper` (không inject trực tiếp `IMapper`).

---

## III. Quy tắc Nghiệp vụ Tài chính (KHÔNG ĐƯỢC VI PHẠM)

### 1. Waterfall Repayment Allocation

Tiền khách thanh toán cho kỳ hạn PHẢI được phân bổ theo thứ tự nghiêm ngặt:

**Tiền phạt (PenaltyAmount) → Tiền lãi (InterestAmount) → Tiền gốc (PrincipalAmount)**

- Trạng thái kỳ hạn cập nhật thành `Paid` khi `PaidAmount >= PenaltyAmount + InterestAmount + PrincipalAmount`.
- Nếu chưa đủ: `Partial`.
- Đồng thời cập nhật `Loan.Paid` tổng.

### 2. Interest Rate Calculation

- Lãi suất = `BaseRate * RiskFactor` (theo cấp tài khoản `LoanRate`).
- `RiskFactor = 1 + 0.12 * (months - 1)` — lãi suất tăng theo kỳ hạn.
- Tổng tiền vay = `Amount + (Amount * InterestRate * Months)`, làm tròn 0 chữ số thập phân.

### 3. Penalty Calculation

- Công thức: `OverdueDays * (InterestRate * 1.5 / 100) * PrincipalAmount / 365`.
- Job `daily-overdue-processor` chạy lúc 00:00 UTC hàng ngày quét tất cả `UserRepayment` quá hạn chưa thanh toán.
- Khoản vay Active bị quá hạn lần đầu sẽ chuyển sang `LoanStatus.Overdue`.

### 4. Transaction Ledger (Sổ cái giao dịch)

- Mọi dòng tiền (giải ngân, thanh toán, phí phạt) PHẢI ghi nhận vào bảng `LoanTransaction`.
- Mỗi record gồm: `LoanId`, `Amount`, `TransactionType` (enum), `ReferenceNumber` (mã GD ngân hàng/cổng thanh toán).

### 5. Idempotency cho thanh toán

- Khi xử lý IPN (VNPay/MoMo), phải kiểm tra `ReferenceNumber` đã tồn tại trong `LoanTransaction` hay chưa trước khi dispatch `PayLoanCommand`.
- PHẢI validate chữ ký HMAC SHA512 (`IVNPayService.ValidateSignature`) trước khi xử lý.
- **Lưu ý**: Race condition handling bằng Redis Lock chưa triển khai, cần bổ sung.

---

## IV. Infrastructure & External Services

### 1. SQL Server (Database chính)

- **ORM**: Entity Framework Core 8.0 + SQL Server provider.
- **Migrations**: Code-First, assembly nằm tại `Persistence`. Command: `dotnet ef database update --project Persistence --startup-project Presentation`.
- **Retry on failure**: Bật `EnableRetryOnFailure` (maxRetryCount=10, maxRetryDelay=5s) cho SQL transient errors.
- **Startup migration**: `Program.cs` tự chạy `Database.Migrate()` với retry 15 lần × 3s delay (phục vụ Docker Compose khi SQL container chưa sẵn sàng).

### 2. Hangfire (Background Jobs)

- **Storage**: SQL Server riêng (connection string `BackgroundConnection`, database `Hangfire`).
- **Auto-create DB**: `HangfireDatabaseEnsurer` tự tạo database Hangfire nếu chưa có.
- **Dashboard**: `/hangfire` (chỉ bật trong Development).
- **Recurring jobs** (đăng ký qua `IRecurringJobRegistrar` — Clean Architecture):
  - `test-hourly`: Chạy mỗi giờ.
  - `ReminderLoanRepayment3DaysJob`: Nhắc trước 3 ngày.
  - `ReminderLoanRepayment1DayJob`: Nhắc trước 1 ngày.
  - `ReminderLoanRepaymentLateHourJob`: Nhắc khi trễ hạn.
  - `daily-overdue-processor`: Tính phạt + cập nhật trạng thái quá hạn lúc 00:00 UTC.
- Kiểu job hỗ trợ: `Enqueue` (chạy ngay), `Schedule` (chạy sau delay), `Recurring` (theo cron).

### 3. RabbitMQ (Message Queue)

- Connection quản lý qua `RabbitMqConnection` (singleton, async init).
- Consumer: `RabbitMqConsumer` (HostedService, chạy nền).
- Service: `IRabbitMqService` → `RabbitMqService` (scoped).
- Queue names mapping tại `AppConstants.Queues`.
- Config: Section `RabbitMQ` trong appsettings — `Host`, `Port`, `Username`, `Password`.
- Có flag `Config.EnableQueue` để bật/tắt queue processing.

### 4. Redis (Distributed Cache)

- Client: `StackExchange.Redis` (`IConnectionMultiplexer` singleton).
- Service: `IRedisService` → `RedisService` (scoped).
- Config: Section `Redis` — `ConnectionString`, `InstanceName`, `KeyPrefix` ("HCRM:"), `MessageExpirationMinutes`, `MaxMessageCount`, `EnableCaching`.

### 5. VNPay (Payment Gateway)

- Service: `IVNPayService` → `VNPayService`.
- Config: Section `VNPay` — `TmnCode`, `HashSecret`, `Url`, `ReturnUrl`.
- API Endpoints:
  - `POST /api/v1/payment/create-url` — Tạo URL thanh toán.
  - `GET /api/v1/payment/vnpay-return` — User redirect sau thanh toán.
  - `POST /api/v1/payment/vnpay-ipn` — Server-to-server callback (validate signature → dispatch `PayLoanCommand`).

---

## V. API Design Rules

### 1. Versioning

- Format: `/api/v{version}/[controller-route]` (URL Segment).
- Default version: `v1.0`.
- Swagger hỗ trợ multiple versions qua `IApiVersionDescriptionProvider`.

### 2. Authentication

- **JWT Bearer** — config section `Jwt` (`Issuer`, `Audience`, `Key`, `ExpiresIn` phút).
- Password hashing: **BCrypt** qua `IHashPassword`.
- Refresh token hash lưu trên entity `User.RefreshTokenHash`.
- User ID trích xuất từ `ClaimTypes.NameIdentifier`.

### 3. Controller Conventions

- Attribute: `[ApiController]`, `[Authorize]`, `[ApiVersion("1.0")]`, `[Route("api/v{version:apiVersion}/[route]")]`.
- Pattern xử lý response:
  ```csharp
  var result = await _mediator.Send(command, cancellationToken);
  return Ok(new Response<T>(result.Result) { Data = result.Data, Message = ... });
  ```
- Pagination input dùng `[FromQuery] BasePaginationQueryDto`.

### 4. Danh sách Controllers & Routes hiện tại

| Controller | Route Prefix | Auth |
|-----------|-------------|------|
| `AuthController` | `/api/v1/auth` | No |
| `AccountController` | `/api/v1/account` | Yes |
| `LoanController` | `/api/v1/loan` | Yes |
| `NotifcationController` | `/api/v1/notification` | Yes |
| `UserReferenceController` | `/api/v1/user-reference` | Yes |
| `PaymentController` | `/api/v1/payment` | Mixed |

---

## VI. DevOps & Deployment

### 1. Docker Compose

- File: `compose.yaml` (version 3.8).
- Services: `db` (SQL Server 2022), `redis`, `rabbitmq` (3-management), `app`.
- Biến môi trường qua file `.env`: `SA_PASSWORD`, `DB_CONNECTION`, `HF_CONNECTION`.
- App tự retry kết nối SQL, tự tạo DB Hangfire, tự chạy EF Migrate.

### 2. Ports

| Service | Port | Ghi chú |
|---------|------|---------|
| App | 5000 | HTTP API |
| SQL Server | 1433 | Database |
| Redis | 6379 | Cache |
| RabbitMQ | 5672 / 15672 | AMQP / Management UI |

### 3. Environment-specific

- Swagger chỉ bật trong `Development`.
- Hangfire Dashboard tại `/hangfire`.

---

## VII. Testing Standards

### 1. Test Plans

- Mọi feature đều có test plan chi tiết tại `doc/test/`.
- Naming: `{feature_name}_test_plan.md`.

### 2. Unit Tests bắt buộc cho

- `LoanInterestRateService` — tính lãi suất, risk factor, tổng tiền vay.
- `PenaltyCalculationService` — tính phạt quá hạn.
- `PayLoanCommand` Handler — waterfall allocation logic (full payment, partial, early settlement).

---

## VIII. Governance

- Hiến pháp này là văn bản tối cao quy định chuẩn mực thiết kế và phát triển hệ thống HCRM.
- Mọi thay đổi kiến trúc, thêm layer, thay đổi flow phụ thuộc, hoặc sửa đổi quy tắc nghiệp vụ tài chính PHẢI được cập nhật vào hiến pháp trước khi viết code.
- Khi thêm feature mới: tạo plan tại `doc/plan/`, tạo test plan tại `doc/test/`, tuân thủ CQRS pattern.

**Version**: 2.0.0 | **Ratified**: 2026-06-28 | **Last Amended**: 2026-06-28
