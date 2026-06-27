# Kế hoạch Triển khai: Lấy thông tin người dùng (Get User Info)

## 1. Tổng quan
Tính năng cho phép người dùng hiện tại (đã đăng nhập) lấy thông tin chi tiết của tài khoản mình từ Token xác thực.

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/account/info`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token)
- **Request Body**: Không có
- **Response Body**: `Response<UserInfo>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "id": 1,
        "email": "user@example.com",
        "fullName": "Nguyễn Văn A",
        "role": 2
      },
      "message": "Get user info successful"
    }
    ```
  - Thất bại (Không được xác thực):
    - HTTP Status: `401 Unauthorized`

## 3. Các thực thể Database liên quan (Database Entities)
- **User**: Truy vấn thông tin người dùng dựa trên `Id` trích xuất từ Token.
  - `Id` (Khóa chính)
  - `Email`
  - `FullName`
  - `Role`

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Người dùng gửi request kèm JWT token trong header `Authorization: Bearer <token>`.
2. `AccountController.GetUserInfo` nhận request, kiểm tra xác thực.
3. Trích xuất `UserId` từ `Claims` (trường `ClaimTypes.NameIdentifier`) trong Token.
4. Gửi `GetUserInfoQuery` thông qua MediatR với `Id` của người dùng.
5. Trong Handler (`GetUserInfoQueryHandler`):
   - Truy vấn thông tin user qua `IUserRepository.GetOne`.
   - Nếu không tìm thấy user, trả về lỗi: `Get user info failed`.
   - Map thực thể sang `UserInfo` DTO qua `IAutoMapper`.
   - Trả về response thành công.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [AccountController.cs](file:///D:/CRM/Presentation/Controllers/AccountController.cs)
- **Application Layer**:
  - [GetUserInfoQuery.cs](file:///D:/CRM/Application/Features/User/Query/GetUserInfoQuery.cs)
  - `UserInfo` trong [Domain/Models/DTO/User](file:///D:/CRM/Domain/Models/DTO/User)
- **Persistence Layer**:
  - [UserRepository.cs](file:///D:/CRM/Persistence/Repositories/UserRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy thông tin thành công** khi cung cấp JWT token hợp lệ.
- **TC2: Lấy thông tin thất bại** với mã lỗi `401 Unauthorized` do không truyền hoặc truyền sai định dạng Token.
- **TC3: Lấy thông tin thất bại** khi ID người dùng trong Token không tồn tại trong DB.
