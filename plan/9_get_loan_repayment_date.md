# Kế hoạch Triển khai: Lịch trả nợ theo khoản vay (Get Loan Repayment Dates)

## 1. Tổng quan
Tính năng cho phép người dùng xem lịch trình trả nợ chi tiết của một khoản vay cụ thể (gồm các ngày phải trả nợ và số tiền tương ứng cho từng kỳ).

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/loan/repayment`
- **Authentication**: Yêu cầu xác thực JWT (User Role)
- **Query Parameter**:
  - `id` (int, required) - ID của khoản vay cần xem lịch trả nợ.
- **Response Body**: `Response<UserListRepayment>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "amount": 10000000,
        "repaymentDates": [
          {
            "id": 1,
            "repaymentDate": "2026-07-20T19:12:00",
            "status": 0
          },
          {
            "id": 2,
            "repaymentDate": "2026-08-20T19:12:00",
            "status": 0
          }
        ]
      },
      "message": "Loan repayment dates retrieved successfully"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **Loan**: Dùng để lấy thông tin số tiền gốc của khoản vay.
- **UserRepayment**: Chứa danh sách các kỳ trả nợ cụ thể gắn với khoản vay (`RepaymentDate`, `Status`).

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Khách hàng gửi yêu cầu kèm theo `id` khoản vay qua `/api/v1/loan/repayment`.
2. Controller gửi `GetLoanRepaymentDateQuery` qua MediatR.
3. Trong Handler (`GetLoanRepaymentDateQueryHandler`):
   - Kiểm tra xem khoản vay có tồn tại trong hệ thống không qua `ILoanRepository.GetOne`. Nếu không, trả về lỗi: `Loan not found`.
   - Lấy danh sách lịch trả nợ từ `IUserRepaymentRepository.Get` lọc theo `LoanId == request.Id`, sắp xếp giảm dần theo ngày trả nợ (`RepaymentDate`).
   - Sử dụng `IAutoMapper` để map danh sách các bản ghi `UserRepayment` sang `UserRepaymentDateResponse` DTO.
   - Trả về đối tượng `UserListRepayment` chứa số tiền gốc khoản vay và danh sách chi tiết các kỳ hạn thanh toán.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [LoanController.cs](file:///D:/CRM/Presentation/Controllers/LoanController.cs)
- **Application Layer**:
  - [GetLoanRepaymentDate.cs](file:///D:/CRM/Application/Features/Loan/Query/GetLoanRepaymentDate.cs)
  - DTOs: `UserListRepayment` và `UserRepaymentDateResponse` trong [Domain/Models/DTO/UserRepayment](file:///D:/CRM/Domain/Models/DTO/UserRepayment)
- **Persistence Layer**:
  - [UserRepaymentRepository.cs](file:///D:/CRM/Persistence/Repositories/UserRepaymentRepository.cs)
  - [LoanRepository.cs](file:///D:/CRM/Persistence/Repositories/LoanRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy lịch trả nợ thành công** khi khoản vay có tồn tại trong hệ thống. Kiểm tra xem thứ tự hiển thị và số lượng kỳ trả nợ có khớp với kỳ hạn của khoản vay ban đầu không.
- **TC2: Lấy lịch trả nợ thất bại** do khoản vay không tồn tại trong DB.
- **TC3: Kiểm tra tính bảo mật** (Chỉ cho phép chính khách hàng sở hữu khoản vay hoặc Admin truy cập lịch trả nợ này).
