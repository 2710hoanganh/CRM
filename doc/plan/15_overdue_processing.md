# Kế hoạch Triển khai: Xử lý Tiền phạt, Quá hạn & Background Jobs (Overdue Processing)

## 1. Tổng quan
Hệ thống tự động tính toán tiền phạt cho các khoản vay quá hạn và cập nhật trạng thái khoản vay tương ứng thông qua các tác vụ chạy ngầm.

## 2. Xây dựng Công thức & Dịch vụ (Domain Services)
- **PenaltyCalculationService**: 
  - Thực thi logic tính toán tiền phạt dựa theo quy định.
  - Công thức mẫu: `Số ngày quá hạn * (150% * Lãi suất vay) / 365`.

## 3. Các Tác vụ định kỳ (Hangfire Jobs)
- **DailyOverdueProcessorJob**:
  - **Chức năng**: Xử lý khoản vay quá hạn hàng ngày.
  - **Quy trình**: 
    - Lấy tất cả `UserRepayment` có `RepaymentDate < Today` và `Status != Paid`.
    - Gọi `PenaltyCalculationService` để tính tiền phạt.
    - Cập nhật `PenaltyAmount` vào cơ sở dữ liệu.
    - Thay đổi trạng thái `Loan` thành `Overdue` nếu khoản vay này bị quá hạn lần đầu tiên.
- **NotificationTriggerJob**:
  - **Chức năng**: Quét các khoản sắp đến hạn để đẩy tin nhắn thông báo vào RabbitMQ. Message queue consumer sẽ lấy message và thực thi gửi Email/SMS thông qua API.

## 4. Kịch bản Kiểm thử (Test Cases)
- **TC1: Tính lãi phạt chính xác**: Cung cấp đầu vào là số ngày quá hạn, nợ gốc và lãi suất, kiểm tra output của `PenaltyCalculationService`.
- **TC2: Job tính phạt hàng ngày**: Giả lập dữ liệu `UserRepayment` trễ hạn, chạy trigger thủ công job `DailyOverdueProcessorJob` và kiểm tra database xem `PenaltyAmount` và `Status` của `Loan` có cập nhật đúng không.

## 5. Cấu trúc mã nguồn chi tiết
- **Application Layer**:
  - [IPenaltyCalculationService.cs](file:///d:/CRM/Application/Services/IPenaltyCalculationService.cs)
- **Infrastructure Layer**:
  - [PenaltyCalculationService.cs](file:///d:/CRM/Infrastructure/Services/PenaltyCalculationService.cs)
  - [RecurringJobRegistrar.cs](file:///d:/CRM/Infrastructure/Extensions/HangFire/RecurringJobRegistrar.cs)
