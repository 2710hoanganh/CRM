# Kế hoạch Triển khai: Duyệt khoản vay (Review Loan)

## 1. Tổng quan
Tính năng cho phép Quản trị viên (Admin) xem xét và phê duyệt hoặc từ chối một khoản vay đang ở trạng thái Chờ duyệt (`Pending`).

## 2. API Endpoint Specification
- **Method**: `POST`
- **URL**: `/api/v1/loan/review`
- **Authentication**: Yêu cầu xác thực JWT (Admin/Staff Role)
- **Request Body**: `ReviewLoanCommand`
  ```json
  {
    "id": 1,
    "feedBack": "Đủ điều kiện phê duyệt",
    "status": 1
  }
  ```
  *(Status: 1 = Approved, 2 = Rejected/Cancelled)*
- **Response Body**: `Response<bool>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": true,
      "message": "Loan reviewed successfully"
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
- **Loan**: Thực thể chính cần cập nhật thông tin phê duyệt.
  - `Id`
  - `Status` (Cập nhật sang Approved/Rejected)
  - `FeedBack` (Lưu thông tin phản hồi từ Admin)

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Admin gửi yêu cầu phê duyệt thông qua `/api/v1/loan/review`.
2. Gửi `ReviewLoanCommand` qua MediatR.
3. Trong Handler (`ReviewLoanCommandHandler`):
   - Sử dụng `ILoanRepository.ExcuteUpdate` để cập nhật trực tiếp trường `Status` và `FeedBack` của khoản vay khớp với `Id` trong yêu cầu.
   - Lưu các thay đổi vào DB qua `IUnitOfWork.SaveChangesAsync`.
   - Trả về kết quả thành công.
   - *(Cải tiến đề xuất trong tương lai: Tự động gửi thông báo qua RabbitMQ hoặc Mail khi trạng thái khoản vay được cập nhật).*

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [LoanController.cs](file:///d:/CRM/Presentation/Controllers/LoanController.cs)
- **Application Layer**:
  - [ReviewLoanCommand.cs](file:///d:/CRM/Application/Features/Loan/Command/ReviewLoanCommand.cs)
  - Interfaces: `ILoanRepository`, `IUnitOfWork`
- **Persistence Layer**:
  - [LoanRepository.cs](file:///d:/CRM/Persistence/Repositories/LoanRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Phê duyệt thành công** một khoản vay hợp lệ (Cập nhật trạng thái thành `Approved` và ghi nhận Feedback).
- **TC2: Từ chối thành công** khoản vay (Cập nhật trạng thái thành `Rejected` kèm lý do từ chối).
- **TC3: Phê duyệt thất bại** do người thực hiện không phải Admin (Kiểm tra phân quyền Bearer Token).
- **TC4: Phê duyệt thất bại** khi ID khoản vay không tồn tại trong hệ thống.
