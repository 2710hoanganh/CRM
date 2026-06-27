# Kế hoạch Triển khai: Danh sách người tham chiếu (Get User References)

## 1. Tổng quan
Tính năng cho phép người dùng hoặc hệ thống xem danh sách các người tham chiếu của người dùng hiện tại dưới dạng phân trang.

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/user-reference/get-all`
- **Authentication**: Yêu cầu xác thực JWT (User Role)
- **Request Parameters**:
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
- **Response Body**: `Response<Paged<List<GetUserReferenceResponse>>>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "items": [
          {
            "id": 1,
            "fullName": "Nguyễn Văn B",
            "phoneNumber": "0987654321",
            "relationship": 1
          }
        ],
        "pageNumber": 1,
        "pageSize": 10,
        "totalCount": 1,
        "totalPages": 1,
        "message": "Success"
      },
      "message": "Success"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **UserReference**: Lấy thông tin các người tham chiếu lọc theo `UserId`.
  - `Id`, `UserId`, `FullName`, `PhoneNumber`, `Relationship`.

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Người dùng gửi yêu cầu lấy danh sách người tham chiếu qua `/api/v1/user-reference/get-all`.
2. Controller trích xuất `UserId` từ token và gán vào `Id` của `query` DTO, sau đó gửi `GetUserReferenceQuery` qua MediatR.
3. Trong Handler (`GetUserReferenceHandler`):
   - Truy vấn thông tin phân trang người tham chiếu qua `IUserReferenceRepository.GetPagination` với điều kiện lọc `x => x.UserId == request.Id`.
   - Lấy danh sách kết quả từ `paged.Data` và map sang `GetUserReferenceResponse` DTO qua `IAutoMapper`.
   - Trả về đối tượng phân trang `Paged<List<GetUserReferenceResponse>>` chứa dữ liệu đã được map.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [UserReferenceController.cs](file:///D:/CRM/Presentation/Controllers/UserReferenceController.cs)
- **Application Layer**:
  - [GetUserReference.cs](file:///D:/CRM/Application/Features/UserReference/Query/GetUserReference.cs)
  - `GetUserReferenceResponse` trong [Domain/Models/DTO/UserReference](file:///D:/CRM/Domain/Models/DTO/UserReference)
  - Interface: `IUserReferenceRepository`
- **Persistence Layer**:
  - [UserReferenceRepository.cs](file:///D:/CRM/Persistence/Repositories/UserReferenceRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy danh sách thành công** trả về đúng danh sách những người tham chiếu của người dùng hiện tại đang đăng nhập.
- **TC2: Trả về danh sách trống** (thành công) nếu người dùng chưa khai báo người tham chiếu nào.
- **TC3: Kiểm tra phân quyền** (yêu cầu Token hợp lệ).
