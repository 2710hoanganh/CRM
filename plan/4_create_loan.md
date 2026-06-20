# Kế hoạch Triển khai: Tạo khoản vay (Create Loan)

## 1. Tổng quan
Tính năng cho phép người dùng đăng ký một khoản vay mới với số tiền và thời hạn vay mong muốn. 

## 2. API Endpoint Specification
- **Method**: `POST`
- **URL**: `/api/v1/loan/create`
- **Authentication**: Yêu cầu xác thực JWT (User Role)
- **Request Body**: `CreateLoanCommand` (chứa `CreateLoanRequest`)
  ```json
  {
    "request": {
      "loanAmount": 10000000,
      "loanTerm": 12
    }
  }
  ```
- **Response Body**: `Response<bool>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": true,
      "message": "Loan created successfully"
    }
    ```
  - Thất bại (Ví dụ: Chưa đủ thông tin tham chiếu):
    ```json
    {
      "result": 0,
      "data": false,
      "message": "User have to add at least two references"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **Loan**: Lưu trữ thông tin khoản vay mới (Số tiền, Kỳ hạn, Lãi suất, Tổng tiền phải trả, Số tiền trả mỗi kỳ, Trạng thái mặc định: `Pending`).
- **UserReference**: Dùng để kiểm tra xem người dùng đã cập nhật đủ thông tin người tham chiếu chưa.
- **UserRepayment**: Lưu trữ kế hoạch/lịch trả nợ cho từng kỳ của khoản vay (sinh ra tự động dựa trên kỳ hạn vay).

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Người dùng gửi yêu cầu qua `/api/v1/loan/create`.
2. Controller trích xuất `UserId` từ Token và gán vào `command.Id`.
3. Gửi `CreateLoanCommand` qua MediatR.
4. Trong Handler (`CreateLoanCommandHandler`):
   - Kiểm tra thông tin người tham chiếu trong DB qua `IUserReferenceRepository.Find`. Nếu người dùng chưa thêm người tham chiếu, trả về lỗi: `"User have to add at least two references"`. (Ghi chú: Lẽ ra cần đếm số lượng $\ge 2$, kiểm tra thực tế trong code hiện tại chỉ check sự tồn tại của người tham chiếu `!userRef`).
   - Tính toán lãi suất dựa trên kỳ hạn vay qua `ILoanInterestRate.CalculateInterestRate` (sử dụng base rate).
   - Tính toán tổng tiền phải trả qua `ILoanInterestRate.CalculateTotal` (gồm gốc và lãi).
   - Bắt đầu một database transaction qua `IUnitOfWork.BeginTransactionAsync` để đảm bảo tính toàn vẹn dữ liệu.
   - Tạo thực thể `Loan` với trạng thái ban đầu là `Pending` (Chờ duyệt).
   - Lưu `Loan` vào DB để có `LoanId`.
   - Sinh lịch trả nợ `UserRepayment` cho từng kỳ (mỗi kỳ cách nhau 1 tháng tính từ thời điểm hiện tại). Tác vụ này chạy bất đồng bộ trong background task (`Task.Run`).
   - Commit transaction và trả về kết quả thành công.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [LoanController.cs](file:///D:/CRM/Presentation/Controllers/LoanController.cs)
- **Application Layer**:
  - [CreateLoanCommand.cs](file:///D:/CRM/Application/Features/Loan/Command/CreateLoanCommand.cs)
  - `CreateLoanRequest` trong [Domain/Models/DTO/Loan](file:///D:/CRM/Domain/Models/DTO/Loan)
  - Interfaces: `ILoanRepository`, `ILoanInterestRate`, `IUserRepaymentRepository`, `IUserReferenceRepository`
- **Infrastructure Layer**:
  - [LoanInterestRateService.cs](file:///D:/CRM/Infrastructure/Services/LoanInterestRateService.cs) (Tính toán lãi suất và tổng tiền)
  - [DateTimeService.cs](file:///D:/CRM/Infrastructure/Services/DateTimeService.cs) (Xác định ngày đến hạn trả nợ)
- **Persistence Layer**:
  - [Repositories](file:///D:/CRM/Persistence/Repositories)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Tạo khoản vay thành công** khi người dùng có đầy đủ thông tin tham chiếu. Kiểm tra xem các bản ghi lịch trả nợ (`UserRepayment`) có được sinh ra chính xác và khớp với số tháng của kỳ hạn vay không.
- **TC2: Tạo khoản vay thất bại** do người dùng chưa khai báo thông tin tham chiếu.
- **TC3: Tạo khoản vay thất bại** do số tiền hoặc thời hạn vay không hợp lệ (Ví dụ: Số tiền âm, kỳ hạn vay là 0 hoặc âm).
