# Kế hoạch Triển khai: Đăng nhập (User Login)

## 1. Tổng quan
Tính năng cho phép người dùng đăng nhập vào hệ thống bằng Email và Mật khẩu. Sau khi đăng nhập thành công, hệ thống trả về JWT Access Token (để xác thực các API sau) và Refresh Token (để làm mới phiên đăng nhập).

## 2. API Endpoint Specification
- **Method**: `POST`
- **URL**: `/api/v1/auth/login`
- **Authentication**: Không yêu cầu (Public)
- **Request Body**: `LoginRequest`
  ```json
  {
    "email": "user@example.com",
    "password": "StrongPassword123"
  }
  ```
- **Response Body**: `Response<LoginResponse>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "accessToken": "ey...",
        "refreshToken": "rf...",
        "tokenType": "Bearer",
        "expiresIn": 30,
        "userInfo": {
          "id": 1,
          "email": "user@example.com",
          "firstName": "Nguyễn",
          "lastName": "Văn A",
          "fullName": "Nguyễn Văn A",
          "role": 2
        }
      },
      "message": "Login successful"
    }
    ```
  - Thất bại (Sai mật khẩu hoặc email không tồn tại):
    ```json
    {
      "result": 0,
      "data": null,
      "message": "Email or password is incorrect"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **User**: Truy vấn thông tin người dùng và cập nhật Refresh Token đã hash.
  - `Email` (dùng để tìm kiếm)
  - `PasswordHash` (dùng để so sánh mật khẩu)
  - `RefreshTokenHash` (lưu trữ hash của Refresh Token mới nhất)

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Nhận yêu cầu đăng nhập qua `AuthController.Login`.
2. Gửi `LoginQuery` thông qua MediatR.
3. Trong Handler (`LoginQueryHandler`):
   - Tìm người dùng bằng Email qua `IUserRepository.GetOne`.
   - Nếu không tìm thấy, trả về lỗi: `Email or password is incorrect`.
   - Kiểm tra mật khẩu bằng `IHashPassword.VerifyPassword`. Nếu không đúng, trả về lỗi.
   - Tạo Access Token và Refresh Token sử dụng `ITokenService.GenerateAccessToken` và `ITokenService.GenerateRefreshToken`.
   - Băm Refresh Token và lưu phiên bản băm vào trường `RefreshTokenHash` của User để bảo mật.
   - Lưu thay đổi qua `IUserRepository.ExcuteUpdate` và `IUnitOfWork.SaveChangesAsync`.
   - Trả về thông tin JWT và User Info.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [AuthController.cs](file:///d:/CRM/Presentation/Controllers/AuthController.cs)
- **Application Layer**:
  - [LoginQuery.cs](file:///d:/CRM/Application/Features/User/Query/LoginQuery.cs)
  - `LoginRequest` / `LoginResponse` trong [Domain/Models/DTO/User](file:///d:/CRM/Domain/Models/DTO/User)
- **Infrastructure Layer**:
  - [HashingService.cs](file:///d:/CRM/Infrastructure/Services/HashingService.cs)
  - [TokenService.cs](file:///d:/CRM/Infrastructure/Services/TokenService.cs)
- **Persistence Layer**:
  - [UserRepository.cs](file:///d:/CRM/Persistence/Repositories/UserRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Đăng nhập thành công** với thông tin chính xác, kiểm tra xem Access Token có chứa đúng Claim của User không.
- **TC2: Đăng nhập thất bại** do nhập sai Email.
- **TC3: Đăng nhập thất bại** do nhập sai Mật khẩu.
- **TC4: Đăng nhập thất bại** do truyền thiếu Email hoặc Mật khẩu.
