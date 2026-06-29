# HCRM Project Constitution

Đây là tài liệu gốc (Persistent Centralized Intent Layer) xác định các nguyên tắc, tiêu chuẩn, và ràng buộc kỹ thuật của dự án HCRM. Tất cả các lập trình viên và AI Agents bắt buộc phải tuân thủ tài liệu này khi đóng góp code.

## 1. Tech Stack & Frameworks
- **Backend:** C# / .NET 8.0
- **Web API:** ASP.NET Core Web API
- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server
- **Authentication:** JWT Bearer Token, mật khẩu hash bằng BCrypt
- **Background Jobs:** Hangfire
- **Caching:** Redis (StackExchange.Redis)
- **Message Broker:** RabbitMQ
- **Object Mapping:** AutoMapper

## 2. Architectural Constraints (Clean Architecture)
Dự án được cấu trúc theo 5 lớp tách biệt nghiêm ngặt:
1. **Domain:** Chứa các Entities, DTOs, Enums, Constants. Không phụ thuộc vào bất kỳ thư viện ngoài nào (trừ MediatR Contracts nếu cần).
2. **Application:** Nơi xử lý Business Logic. Sử dụng **CQRS Pattern** qua thư viện `MediatR`. Mọi use case đều phải được đóng gói thành Command hoặc Query riêng biệt.
3. **Infrastructure:** Triển khai các external services như Email, SMS, Payment Gateway (VNPay), RabbitMQ, Redis, Hangfire, TokenService, HashingService.
4. **Persistence:** Triển khai Data Access (AppDbContext) bằng Entity Framework Core. Bắt buộc dùng **Repository Pattern** và **Unit of Work Pattern**.
5. **Presentation:** Chứa các Web API Controllers, Extensions, Middleware. API Controllers chỉ nhận Request, gọi MediatR và trả về Response chuẩn. Tuyệt đối không chứa business logic trong Controller.

## 3. Coding Standards & Conventions
- **Response Format:** Mọi API đều phải trả về theo cấu trúc `Response<T>` chuẩn:
  ```json
  {
    "result": 1, 
    "data": { ... },
    "message": "Success",
    "errors": []
  }
  ```
  Trong đó `result` là `1` (SUCCESS) hoặc `0` (ERROR).
- **Naming Conventions:**
  - Class/Method: PascalCase (e.g., `UserService`, `CreateLoanAsync`).
  - Variables/Parameters: camelCase.
  - Interfaces: Prefix `I` (e.g., `IRepository`).
- **Dependency Injection (DI):** Tất cả services phải được inject qua constructor.
- **Asynchronous Programming:** Luôn sử dụng `async/await` cho các tác vụ I/O. Hậu tố phương thức kết thúc bằng `Async` (tùy chọn nhưng khuyến khích). Luôn truyền `CancellationToken` vào các phương thức EF Core hoặc MediatR.
- **Pagination:** Các API dạng danh sách phải có phân trang, sử dụng response `Paged<List<T>>`.

## 4. Spec-Driven Workflow Rules
- Không tiến hành code khi chưa có file `.md` mô tả đặc tả (Spec) trong thư mục `spec/use_cases/` hoặc `doc/plan/`.
- **API Drift Prevention:** Mọi thay đổi về Request/Response Model, hoặc bổ sung Route đều phải cập nhật lại file `spec/openapi.json`.
- **Knowledge Graph Alignment:** Khi phát triển tính năng mới, cần đảm bảo cập nhật `spec/knowledge_graph.md` để mapping giữa Spec -> Controller -> MediatR -> Repository.
