# Kế hoạch Triển khai: Danh sách khoản vay của người dùng (Get User Loans)

## 1. Tổng quan
Tính năng cho phép khách hàng hiện tại (đã đăng nhập) xem danh sách các khoản vay cá nhân mà mình đã đăng ký trên hệ thống.

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/loan/all-user`
- **Authentication**: Yêu cầu xác thực JWT (User Role)
- **Request Parameters**:
  - `pageNumber` (ulong, default: 0)
  - `pageSize` (ulong, default: 20)
- **Response Body**: `Response<Paged<List<ListLoanResponse>>>`
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
            "id": 1,
            "userId": 1,
            "userName": "Nguyễn Văn A",
            "amount": 10000000,
            "term": 12,
            "total": 11000000,
            "paybackAmount": 916666.67,
            "createdAt": "2026-06-20T19:12:00",
            "status": 0
          }
        ],
        "message": "Success"
      },
      "message": "Success"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **Loan**: Truy vấn các bản ghi khoản vay có `UserId` khớp với ID người dùng hiện tại.
- **User**: Thực thể quan hệ để lấy tên người vay.

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Khách hàng gửi yêu cầu lấy danh sách khoản vay cá nhân qua `/api/v1/loan/all-user`.
2. Controller trích xuất `UserId` từ Token, gán vào `Id` của `query` và gửi `GetAllUserLoanQuery` qua MediatR.
3. Trong Handler (`GetAllUserLoanQueryHandler`):
   - Gọi `ILoanRepository.GetPaginationWithUser` với điều kiện lọc `x => x.UserId == request.Id`.
   - Sử dụng selector `x => _autoMapper.Map<ListLoanResponse>(x)` để map tự động kết quả.
   - Trả về đối tượng `Paged<List<ListLoanResponse>>` chứa danh sách các khoản vay của riêng người dùng đó.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [LoanController.cs](file:///d:/CRM/Presentation/Controllers/LoanController.cs)
- **Application Layer**:
  - [GetAllUserLoan.cs](file:///d:/CRM/Application/Features/Loan/Query/GetAllUserLoan.cs)
  - `ListLoanResponse` DTO trong [Domain/Models/DTO/Loan](file:///d:/CRM/Domain/Models/DTO/Loan)
  - Interface: `ILoanRepository`
- **Persistence Layer**:
  - [LoanRepository.cs](file:///d:/CRM/Persistence/Repositories/LoanRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy danh sách thành công** trả về chính xác danh sách các khoản vay thuộc sở hữu của người dùng hiện tại (không hiển thị khoản vay của người khác).
- **TC2: Kiểm tra phân trang và lọc** hoạt động đúng đối với tập dữ liệu của riêng người dùng đó.
- **TC3: Trả về danh sách trống** (nhưng thành công) nếu người dùng chưa từng tạo bất kỳ khoản vay nào.
