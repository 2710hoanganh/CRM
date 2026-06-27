# Kế hoạch Triển khai: Tích hợp Cổng thanh toán (Payment Gateway VNPAY/MoMo)

## 1. Tổng quan
Tích hợp hệ thống thanh toán với bên thứ 3 (như VNPAY) để khách hàng có thể thanh toán khoản vay dễ dàng và hệ thống nhận phản hồi tức thời.

## 2. Infrastructure Layer
- **IVNPayService**: 
  - Triển khai logic tạo chữ ký HMAC SHA512.
  - Build URL thanh toán chuyển hướng sang cổng VNPAY.

## 3. Các API Controllers
- **`POST /api/v1/payment/create-url`**: 
  - Nhận tham số `UserRepaymentId` từ người dùng và trả về URL thanh toán VNPAY.
- **`GET /api/v1/payment/vnpay-return`**: 
  - Xử lý request khi người dùng hoàn thành thanh toán trên VNPAY và bị redirect trở về giao diện web.
- **`POST /api/v1/payment/vnpay-ipn`**: 
  - Endpoint Server-to-Server dùng để nhận kết quả giao dịch thực tế từ VNPAY.
  - **Bảo mật & Tính vẹn toàn**: Chặn request giả mạo bằng cách validate signature. Đảm bảo tính Idempotent bằng cách kiểm tra mã giao dịch (`ReferenceNumber`).
  - **Logic**: Nếu hợp lệ, tự động dispatch `PayLoanCommand` để xử lý trừ nợ.
  - **Concurrency**: Chặn race condition bằng Redis Lock (`IRedisService` - dùng Redlock hoặc cờ `LockKey:LoanId`).

## 4. Kịch bản Kiểm thử (Test Cases)
- **TC1: Tạo URL thanh toán thành công**: Gọi API create-url và kiểm tra URL trả về có đầy đủ tham số và chữ ký hợp lệ của VNPAY.
- **TC2: Xử lý IPN hợp lệ**: Gửi một request IPN giả lập với chữ ký đúng, kiểm tra hệ thống có tự động chạy `PayLoanCommand` và ghi nhận thanh toán thành công.
- **TC3: Xử lý IPN giả mạo/trùng lặp**: Gửi IPN sai chữ ký -> Bị reject. Gửi lại một IPN đã xử lý (cùng ReferenceNumber) -> Không cộng dồn/trừ nợ thêm.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [PaymentController.cs](file:///d:/CRM/Presentation/Controllers/PaymentController.cs)
- **Application Layer**:
  - [IVNPayService.cs](file:///d:/CRM/Application/Services/IVNPayService.cs)
- **Infrastructure Layer**:
  - [VNPayService.cs](file:///d:/CRM/Infrastructure/Services/VNPayService.cs)
