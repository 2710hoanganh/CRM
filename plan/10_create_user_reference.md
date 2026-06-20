# Kế hoạch Triển khai: Tạo thông tin tham chiếu (Create User Reference)

## 1. Tổng quan
Tính năng cho phép người dùng thêm thông tin người tham chiếu (Reference) như người thân, đồng nghiệp, bạn bè. Đây là điều kiện cần thiết trước khi tạo bất kỳ khoản vay nào trên hệ thống.

## 2. API Endpoint Specification
- **Method**: `POST`
- **URL**: `/api/v1/user-reference/create`
- **Authentication**: Yêu cầu xác thực JWT (User Role)
- **Request Body**: `CreateUserReferenceCommand` (chứa danh sách `CreateUserReferenceRequest`)
  ```json
  {
    "requests": [
      {
        "fullName": "Nguyễn Văn B",
        "phoneNumber": "0987654321",
        "relationship": 1
      },
      {
        "fullName": "Trần Thị C",
        "phoneNumber": "0912345678",
        "relationship": 2
      }
    ]
  }
  ```
  *(Relationship: 1 = Parent, 2 = Spouse, 3 = Friend, etc.)*
- **Response Body**: `Response<bool>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": true,
      "message": "User references created successfully"
    }
    ```
  - Thất bại:
    ```json
    {
      "result": 0,
      "data": false,
      "message": "Error details..."
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **UserReference**: Thực thể lưu thông tin liên hệ của người tham chiếu.
  - `Id` (Khóa chính)
  - `UserId` (Khóa ngoại liên kết tới người vay)
  - `FullName`
  - `PhoneNumber`
  - `Relationship`

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Người dùng gửi danh sách người tham chiếu qua `/api/v1/user-reference/create`.
2. Controller trích xuất `UserId` từ token và gán vào `command.Id`.
3. Gửi `CreateUserReferenceCommand` qua MediatR.
4. Trong Handler (`CreateUserReferenceCommandHandler`):
   - Lặp qua danh sách `requests` trong command.
   - Với mỗi người tham chiếu, khởi tạo một thực thể `UserReference` mới liên kết với `UserId` của người dùng hiện tại.
   - Thêm danh sách thực thể này vào DB qua `IUserReferenceRepository.AddRange`.
   - Gọi `IUnitOfWork.SaveChangesAsync` để lưu dữ liệu vào cơ sở dữ liệu.
   - Trả về kết quả thành công.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [UserReferenceController.cs](file:///D:/CRM/Presentation/Controllers/UserReferenceController.cs)
- **Application Layer**:
  - [CreateUserReference.cs](file:///D:/CRM/Application/Features/UserReference/Command/CreateUserReference.cs)
  - `CreateUserReferenceRequest` trong [Domain/Models/DTO/UserReference](file:///D:/CRM/Domain/Models/DTO/UserReference)
  - Interface: `IUserReferenceRepository`
- **Persistence Layer**:
  - [UserReferenceRepository.cs](file:///D:/CRM/Persistence/Repositories/UserReferenceRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Tạo thông tin tham chiếu thành công** khi truyền vào danh sách người tham chiếu hợp lệ (họ tên, số điện thoại, mối quan hệ).
- **TC2: Tạo thông tin tham chiếu thất bại** do thiếu dữ liệu bắt buộc (ví dụ: thiếu số điện thoại, tên).
- **TC3: Tạo thông tin tham chiếu thất bại** do không truyền token xác thực hợp lệ.
