# Kế hoạch Triển khai: Chi tiết khoản vay (Get Loan Info)

## 1. Tổng quan
Tính năng cho phép người dùng hoặc Admin lấy thông tin chi tiết của một khoản vay cụ thể qua ID của nó.

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/loan/info`
- **Query Parameter**: `id` (ID của khoản vay)
- **Authentication**: Yêu cầu xác thực JWT
- **Request Parameters**:
  - `id` (int, required)
- **Response Body**: `Response<GetLoanInfoResponse>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": {
        "id": 1,
        "amount": 10000000,
        "term": 12,
        "rate": 0,
        "interestRate": 10,
        "status": 0,
        "total": 11000000,
        "paybackAmount": 916666.67,
        "feedBack": "",
        "createdDate": "2026-06-20T19:12:00"
      },
      "message": "Loan info found"
    }
    ```
  - Thất bại:
    ```json
    {
      "result": 0,
      "data": null,
      "message": "Loan not found"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **Loan**: Truy vấn thông tin chi tiết của khoản vay.
  - `Id`, `Amount`, `Term`, `Rate`, `InterestRate`, `Status`, `Total`, `PaybackAmount`, `FeedBack`, `CreatedDate`.

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Người dùng gửi request kèm Query Parameter `id` qua `/api/v1/loan/info`.
2. Controller nhận tham số và gửi `GetLoanInfoQuery` qua MediatR.
3. Trong Handler (`GetLoanInfoQueryHandler`):
   - Truy vấn DB qua `ILoanRepository.GetOne` theo `Id` của khoản vay.
   - Nếu không tồn tại, trả về kết quả lỗi: `Loan not found`.
   - Map thực thể sang `GetLoanInfoResponse` DTO qua `IAutoMapper`.
   - Trả về response thành công.
   - *(Cải tiến bảo mật đề xuất: Cần kiểm tra xem người dùng hiện tại có phải chủ sở hữu của khoản vay đó không, tránh trường hợp rò rỉ thông tin của người khác - trừ khi là Admin).*

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [LoanController.cs](file:///D:/CRM/Presentation/Controllers/LoanController.cs)
- **Application Layer**:
  - [GetLoanInfo.cs](file:///D:/CRM/Application/Features/Loan/Query/GetLoanInfo.cs)
  - `GetLoanInfoResponse` trong [Domain/Models/DTO/Loan](file:///D:/CRM/Domain/Models/DTO/Loan)
- **Persistence Layer**:
  - [LoanRepository.cs](file:///D:/CRM/Persistence/Repositories/LoanRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy chi tiết thành công** khi cung cấp ID khoản vay hợp lệ đang có sẵn trong DB.
- **TC2: Lấy chi tiết thất bại** khi ID khoản vay không tồn tại trong hệ thống.
- **TC3: Lấy chi tiết thất bại** do lỗi phân quyền hoặc không có Token xác thực.
