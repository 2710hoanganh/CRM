# Kế hoạch Kiểm thử: Tác vụ chạy ngầm và Nhắc nợ (Background Jobs Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và hướng dẫn xác minh các tác vụ ngầm (Background/Recurring Jobs) sử dụng Hangfire trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **Dashboard URL**: `http://localhost:5000/hangfire`
- **Database**: SQL Server (Database `Hangfire` và `HCRM`)
- **Điều kiện**:
  - Ứng dụng phải được khởi chạy với môi trường `Development` để bật Hangfire Dashboard.
  - Hangfire Server phải hoạt động (hiển thị trong phần "Servers" trên Dashboard).

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Đăng ký thành công các Tác vụ định kỳ (Recurring Jobs) khi khởi động
- **Mục tiêu**: Đảm bảo toàn bộ 4 Tác vụ định kỳ (Recurring Jobs) được đăng ký thành công vào cơ sở dữ liệu và hiển thị trên Hangfire Dashboard sau khi ứng dụng khởi chạy.
- **Các bước thực hiện**:
  1. Khởi động ứng dụng (chạy dự án `Presentation`).
  2. Truy cập trình duyệt tại: `http://localhost:5000/hangfire/recurring`.
  3. Xác minh danh sách hiển thị đúng 4 tác vụ sau với cấu hình Cron tương ứng:
     - `test-hourly` -> Cron: `0 * * * *`
     - `reminder-loan-repayment-3-days` -> Cron: `0 0 * * *`
     - `reminder-loan-repayment-1-day` -> Cron: `0 0 * * *`
     - `reminder-loan-repayment-late-hour` -> Cron: `0 13 * * *`
  4. Đảm bảo cột "Next Execution" hiển thị thời gian chạy dự kiến tiếp theo của các job.

### TC2: Xác minh chạy thủ công (Trigger Job)
- **Mục tiêu**: Đảm bảo các tác vụ ngầm có thể thực thi thành công mà không gây lỗi luồng hoặc ngoại lệ.
- **Các bước thực hiện**:
  1. Tại Hangfire Dashboard (`/hangfire/recurring`), tick chọn tất cả 4 job và nhấn nút **"Trigger now"**.
  2. Truy cập tab **"Jobs"** -> **"Succeeded"** để kiểm tra lịch sử thực thi.
  3. Kiểm tra log Console/Terminal của ứng dụng, đảm bảo in ra các dòng log placeholder tương ứng:
     - `[Recurring] Hourly job at ...`
     - `[Recurring] Reminder 3 days placeholder`
     - `[Recurring] Reminder 1 day placeholder`
     - `[Recurring] Reminder late 1 hour placeholder`

### TC3: Xác minh tự động tạo Database Hangfire khi khởi chạy (Hangfire Database Ensuring)
- **Mục tiêu**: Đảm bảo ứng dụng tự tạo cơ sở dữ liệu `Hangfire` nếu cơ sở dữ liệu này chưa tồn tại trong SQL Server.
- **Các bước thực hiện**:
  1. Mở SSMS (SQL Server Management Studio) hoặc Docker, xóa database `Hangfire` (nếu đang có sẵn).
  2. Khởi động lại ứng dụng.
  3. Xác minh log Console không báo lỗi kết nối SQL Server và tự động tạo lại DB `Hangfire`.
  4. Kiểm tra lại trong SQL Server xem DB `Hangfire` và các bảng dữ liệu bên trong (`dbo.Hash`, `dbo.Job`, v.v.) đã được khởi tạo tự động chưa.

---

## 3. Cách thức kiểm thử nhanh (cURL & Dashboard)
Không có API HTTP trực tiếp cho các tác vụ định kỳ của Hangfire vì chúng chạy ngầm tự động theo lịch Cron. 
Việc kiểm thử được thực hiện hoàn toàn trực quan qua giao diện Hangfire Dashboard:
- Truy cập Dashboard: `http://localhost:5000/hangfire`
- Để theo dõi log thời gian thực của các tác vụ: Quan sát console output trong terminal chạy dự án `Presentation`.
