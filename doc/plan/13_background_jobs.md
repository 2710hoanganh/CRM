# Kế hoạch Triển khai: Tác vụ chạy ngầm và Nhắc nợ (Background Jobs - Hangfire)

## 1. Tổng quan
Tính năng sử dụng **Hangfire** làm công cụ quản lý và chạy các tác vụ định kỳ (Recurring jobs) để nhắc nợ người dùng trước 3 ngày, trước 1 ngày, và thông báo trễ hạn nợ (sau 1 giờ tính từ lúc đến hạn thanh toán).

## 2. Các Tác vụ định kỳ (Recurring Jobs Definition)
- **Tác vụ test hàng giờ (`test-hourly`)**:
  - **Cron**: `0 * * * *` (Chạy ở phút thứ 0 của mỗi giờ)
  - **Chức năng**: Ghi log kiểm tra trạng thái hoạt động của Hangfire.
- **Nhắc nợ trước 3 ngày (`reminder-loan-repayment-3-days`)**:
  - **Cron**: `0 0 * * *` (Chạy vào lúc 00:00 hàng ngày)
  - **Chức năng**: Gửi thông báo nhắc nhở thanh toán khi kỳ hạn thanh toán còn 3 ngày.
- **Nhắc nợ trước 1 ngày (`reminder-loan-repayment-1-day`)**:
  - **Cron**: `0 0 * * *` (Chạy vào lúc 00:00 hàng ngày)
  - **Chức năng**: Gửi thông báo nhắc nhở thanh toán gấp khi kỳ hạn thanh toán còn 1 ngày.
- **Nhắc nợ trễ hạn (`reminder-loan-repayment-late-hour`)**:
  - **Cron**: `0 13 * * *` (Chạy vào lúc 13:00 hàng ngày)
  - **Chức năng**: Gửi thông báo cảnh báo trễ hạn khi người dùng quá hạn thanh toán kỳ đó.

## 3. Các thực thể Database liên quan (Database Entities)
- **UserRepayment**: Lấy danh sách các kỳ thanh toán sắp đến hạn (so sánh `RepaymentDate` với ngày hiện tại).
- **Notification**: Lưu thông báo nhắc nợ mới vào DB cho người dùng sau khi job xử lý thành công.
- **User**: Lấy thông tin email / thông tin liên hệ của khách hàng để gửi thông báo/mail.
- **Hangfire Database**: Dùng để Hangfire lưu trữ hàng đợi và trạng thái của các Job (`BackgroundConnection`).

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Ứng dụng khởi động, `IRecurringJobRegistrar.RegisterRecurringJobs` được gọi để đăng ký các tác vụ này vào Hangfire Server.
2. Khi đến thời gian kích hoạt theo biểu thức Cron:
   - Hangfire Worker kích hoạt tác vụ cụ thể.
   - Truy cập DB để lấy thông tin các kỳ trả nợ sắp tới của người dùng có trạng thái là Chưa thanh toán (`Pending`).
   - Kiểm tra xem thời gian giữa ngày hiện tại và `RepaymentDate` có thỏa mãn điều kiện nhắc nợ không (còn đúng 3 ngày, còn đúng 1 ngày, hoặc đã trễ hạn).
   - Tạo thực thể `Notification` mới, ghi nội dung nhắc nợ tương ứng và gửi thông báo cho người dùng.
   - *(Cải tiến đề xuất: Gửi email thực tế qua một `EmailService` hoặc gửi notification Realtime qua SignalR/RabbitMQ).*

## 5. Cấu trúc mã nguồn chi tiết
- **Application Layer (Interfaces)**:
  - [IHangFireService.cs](file:///D:/CRM/Application/Services/IHangFireService.cs)
  - [IRecurringJobRegistrar.cs](file:///D:/CRM/Application/Services/IRecurringJobRegistrar.cs)
- **Infrastructure Layer (Implementations)**:
  - [HangFireService.cs](file:///D:/CRM/Infrastructure/Extensions/HangFire/HangFireService.cs)
  - [RecurringJobRegistrar.cs](file:///D:/CRM/Infrastructure/Extensions/HangFire/RecurringJobRegistrar.cs)
  - [HangfireDatabaseEnsurer.cs](file:///D:/CRM/Infrastructure/Extensions/HangFire/HangfireDatabaseEnsurer.cs) (Tự động khởi tạo database Hangfire nếu chưa tồn tại ở SQL Server)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Đăng ký thành công các Job khi startup**: Kiểm tra Hangfire Dashboard tại `/hangfire` để xác minh danh sách 4 Recurring Jobs đã hiển thị đúng cấu hình Cron.
- **TC2: Kiểm tra chạy thủ công**: Nhấp vào "Trigger" trên Hangfire Dashboard cho từng job và xác minh log Console / database Notification xem thông báo nhắc nợ có được sinh ra chính xác không.
- **TC3: Kiểm tra lọc ngày**: Mô phỏng các bản ghi `UserRepayment` có ngày đến hạn là $N+3$, $N+1$, $N-1$ ngày và kiểm tra các job nhắc nhở tương ứng có gửi thông báo đúng đối tượng khách hàng hay không.
