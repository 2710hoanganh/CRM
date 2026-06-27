# Kế hoạch Triển khai: Danh sách người tham chiếu (Get User References)

## 1. Tổng quan
Tính năng cho phép người dùng hoặc hệ thống xem danh sách các người tham chiếu của người dùng hiện tại dưới dạng phân trang.

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/user-reference/get-all`
- **Authentication**: Yêu cầu xác thực JWT (User Role)
- **Request Parameters**:
  - `pageNumber` (ulong, default: 0)
  - `pageSize` (ulong, default: 20)
- **Response Body**: `Response<Paged<List<GetUserReferenceResponse>>>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "pageNumber": 0,
        "pageSize": 20,
        "firstPage": 0,
        "lastPage": 0,
        "totalPages": 1,
        "totalRecords": 1,
        "nextPage": false,
        "previousPage": false,
        "data": [
          {
            "fullName": "Nguyễn Văn B",
            "phoneNumber": "0987654321",
            "relationship": 1
          }
        ],
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
  - [UserReferenceController.cs](file:///d:/CRM/Presentation/Controllers/UserReferenceController.cs)
- **Application Layer**:
  - [GetUserReference.cs](file:///d:/CRM/Application/Features/UserReference/Query/GetUserReference.cs)
  - `GetUserReferenceResponse` trong [Domain/Models/DTO/UserReference](file:///d:/CRM/Domain/Models/DTO/UserReference)
  - Interface: `IUserReferenceRepository`
- **Persistence Layer**:
  - [UserReferenceRepository.cs](file:///d:/CRM/Persistence/Repositories/UserReferenceRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy danh sách thành công** trả về đúng danh sách những người tham chiếu của người dùng hiện tại đang đăng nhập.
- **TC2: Trả về danh sách trống** (thành công) nếu người dùng chưa khai báo người tham chiếu nào.
- **TC3: Kiểm tra phân quyền** (yêu cầu Token hợp lệ).
