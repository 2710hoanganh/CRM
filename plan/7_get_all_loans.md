# Kế hoạch Triển khai: Danh sách khoản vay (Admin - Get All Loans)

## 1. Tổng quan
Tính năng cho phép Quản trị viên (Admin) xem và tìm kiếm danh sách toàn bộ các khoản vay của tất cả khách hàng trong hệ thống CRM dưới dạng phân trang.

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/loan/all-admin`
- **Authentication**: Yêu cầu xác thực JWT (Admin/Staff Role)
- **Request Parameters**:
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
- **Response Body**: `Response<Paged<List<ListLoanResponse>>>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "items": [
          {
            "id": 1,
            "amount": 10000000,
            "term": 12,
            "interestRate": 10,
            "status": 0,
            "total": 11000000,
            "userName": "Nguyễn Văn A"
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
- **Loan**: Thực thể chính cần lấy thông tin.
- **User**: Thực thể quan hệ để lấy tên người vay (`UserName` hoặc `FullName`).

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Admin gửi yêu cầu lấy danh sách toàn bộ khoản vay kèm các tham số phân trang qua `/api/v1/loan/all-admin`.
2. Controller gửi `GetAllLoanQuery` qua MediatR.
3. Trong Handler (`GetAllLoanQueryHandler`):
   - Truy vấn danh sách khoản vay phân trang kèm thông tin người dùng liên kết sử dụng `ILoanRepository.GetPaginationWithUser`.
   - Sử dụng `IAutoMapper` để map trực tiếp danh sách `Loan` sang `ListLoanResponse`.
   - Trả về đối tượng `Paged<List<ListLoanResponse>>` chứa danh sách dữ liệu, vị trí trang hiện tại, kích thước trang và tổng số bản ghi.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [LoanController.cs](file:///D:/CRM/Presentation/Controllers/LoanController.cs)
- **Application Layer**:
  - [GetAllLoan.cs](file:///D:/CRM/Application/Features/Loan/Query/GetAllLoan.cs)
  - `ListLoanResponse` trong [Domain/Models/DTO/Loan](file:///D:/CRM/Domain/Models/DTO/Loan)
  - Interface: `ILoanRepository`
- **Persistence Layer**:
  - [LoanRepository.cs](file:///D:/CRM/Persistence/Repositories/LoanRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy danh sách thành công** khi tài khoản gửi request có quyền Admin/Staff.
- **TC2: Kiểm tra phân trang hoạt động đúng** (ví dụ: yêu cầu pageNumber=2, pageSize=5 trả về đúng bản ghi của trang 2).
- **TC3: Lấy danh sách thất bại** nếu tài khoản thực hiện không có quyền Admin (trả về lỗi phân quyền).
