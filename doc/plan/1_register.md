# Kế hoạch Triển khai: Đăng ký tài khoản (User Registration)

## 1. Tổng quan
Tính năng cho phép người dùng đăng ký tài khoản mới trong hệ thống CRM (HCRM) với thông tin cơ bản: Họ tên, Email, Mật khẩu.

## 2. API Endpoint Specification
- **Method**: `POST`
- **URL**: `/api/v1/auth/register`
- **Authentication**: Không yêu cầu (Public)
- **Request Body**: `RegisterModelRequest`
  ```json
  {
    "email": "user@example.com",
    "password": "StrongPassword123",
    "firstName": "Nguyễn",
    "lastName": "Văn A"
  }
  ```
- **Response Body**: `Response<RegisterModelResponse>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "id": 1,
        "email": "user@example.com",
        "fullName": "Nguyễn Văn A"
      },
      "message": "User registered successfully"
    }
    ```
  - Thất bại (Ví dụ: Email đã tồn tại):
    ```json
    {
      "result": 0,
      "data": null,
      "message": "Email already exists"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **User**: Lưu thông tin người dùng được đăng ký.
  - `Id` (Khóa chính)
  - `Email`
  - `PasswordHash`
  - `FirstName`, `LastName`, `FullName`
  - `Role` (Mặc định: `Role.User` = 2)
  - `CreatedDate`, `UpdatedDate`

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Nhận yêu cầu đăng ký qua `AuthController.Register`.
2. Gửi `RegisterCommand` thông qua MediatR.
3. Trong Handler (`RegisterCommandHandler`):
   - Kiểm tra xem Email đã tồn tại trong DB chưa qua `IUserRepository.Find`.
   - Nếu đã tồn tại, trả về kết quả lỗi: `Email already exists`.
   - Băm mật khẩu (Hashing) sử dụng `IHashPassword` (BCrypt).
   - Tạo thực thể `User` mới với `Role.User`.
   - Lưu vào DB qua `IUserRepository.Add` và gọi `IUnitOfWork.SaveChangesAsync`.
   - Map thực thể sang `RegisterModelResponse` và trả về kết quả thành công.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [AuthController.cs](file:///d:/CRM/Presentation/Controllers/AuthController.cs)
- **Application Layer**:
  - [RegisterCommand.cs](file:///d:/CRM/Application/Features/User/Command/RegisterCommand.cs)
  - `RegisterModelRequest` / `RegisterModelResponse` trong [Domain/Models/DTO/User](file:///d:/CRM/Domain/Models/DTO/User)
- **Infrastructure Layer**:
  - [HashingService.cs](file:///d:/CRM/Infrastructure/Services/HashingService.cs)
- **Persistence Layer**:
  - [UserRepository.cs](file:///d:/CRM/Persistence/Repositories/UserRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Đăng ký thành công** với dữ liệu hợp lệ (Email chưa tồn tại).
- **TC2: Đăng ký thất bại** do Email đã tồn tại trong hệ thống.
- **TC3: Đăng ký thất bại** do dữ liệu đầu vào không hợp lệ (Format email sai, thiếu mật khẩu).
