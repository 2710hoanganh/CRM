# Kế hoạch Triển khai: Đòi nợ & Dashboard Thống kê (Collection & Analytics)

## 1. Tổng quan
Xây dựng công cụ hỗ trợ nhân viên Call Center đòi nợ và Dashboard cung cấp số liệu thống kê tình trạng tài chính/nợ xấu cho quản lý.

## 2. Tính năng Thu hồi nợ (Collection Features)
- **Thực thể `CollectionTask`**:
  - Khi khoản vay quá hạn 5 ngày, hệ thống sẽ tự động sinh Task cho Role `CallCenterAgent`.
- **API cho Call Center Agent**: 
  - Ghi log cuộc gọi, tình trạng khách hàng, lời hứa trả nợ (ví dụ: "Khách hẹn mùng 5 trả", "Không nghe máy").

## 3. Thống kê & Dashboard (CQRS)
- Sử dụng Dapper hoặc EF Core GroupBy để tối ưu truy vấn dữ liệu lớn.
- **Query `GetAdminDashboardSummaryQuery`**:
  - Lấy thông tin tổng tiền giải ngân (`TotalDisbursedAmount`) theo tháng/năm.
  - Lấy thông tin tổng tiền thu nợ thực tế (`TotalCollectionAmount`).
  - Lấy thông tin Tỷ lệ nợ xấu (`NPL Ratio`): Tổng dư nợ nhóm 3,4,5 / Tổng dư nợ hiện tại.
  - Dữ liệu biểu đồ (Line chart): Tiền giải ngân vs Tiền thu hồi trong 7 ngày gần nhất.

## 4. Kịch bản Kiểm thử (Test Cases)
- **TC1: Tạo Collection Task tự động**: Kiểm tra điều kiện khoản vay quá hạn 5 ngày để xem Task đòi nợ có được gán cho Agent không.
- **TC2: Lấy số liệu Dashboard**: Mock dữ liệu giải ngân, thu nợ, nợ xấu và gọi API dashboard để so sánh tính chính xác của dữ liệu trả về và performance.
