# Kế hoạch Triển khai: Giải ngân & Quản lý Hợp đồng (Disbursement & Documents)

## 1. Tổng quan
Phát triển luồng quy trình duyệt và giải ngân khoản vay, đồng thời tự động hóa việc khởi tạo hợp đồng (PDF document) cho khách hàng.

## 2. Quy trình Giải ngân (Disbursement Workflow)
- **API `POST /api/v1/loan/{id}/disburse`**: Admin sẽ gọi API này để giải ngân.
- **Logic thực thi**:
  - Tích hợp (mock) API chuyển khoản ngân hàng.
  - Chuyển `LoanStatus` thành `Active`.
  - Sinh ra các dòng lịch trả nợ (`UserRepayment`) tương ứng với kỳ hạn (Term). Ví dụ: kỳ hạn 12 tháng thì sinh ra 12 records (Thay vì tạo ở lúc Create).

## 3. Khởi tạo & Quản lý Hợp đồng (Document Generator)
- **Công cụ**: Sử dụng thư viện `iText7`, `DinkToPdf` hoặc Razor HTML to PDF.
- **Logic thực thi**:
  - Bơm dữ liệu User và Loan vào HTML template.
  - Tự động sinh file PDF Hợp đồng vay (Loan Agreement).
  - Lưu file PDF vào MinIO / AWS S3 (hoặc local drive).
  - Lưu đường dẫn (URL) vào bảng `Loan`.

## 4. Kịch bản Kiểm thử (Test Cases)
- **TC1: Giải ngân khoản vay**: Gọi API giải ngân, kiểm tra `LoanStatus`, xác nhận các records `UserRepayment` được sinh ra đúng số lượng, đúng số tiền mỗi kỳ.
- **TC2: Tạo PDF Hợp đồng**: Kiểm tra file PDF có được sinh ra sau khi duyệt khoản vay, dữ liệu trong PDF có khớp với thông tin khách hàng và khoản vay không.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [LoanController.cs](file:///d:/CRM/Presentation/Controllers/LoanController.cs) (Planned endpoints: giải ngân và hợp đồng)
- **Application Layer**:
  - [DisburseLoanCommand.cs](file:///d:/CRM/Application/Features/Loan/Command/DisburseLoanCommand.cs)
  - [IDateTimeService.cs](file:///d:/CRM/Application/Services/Base/IDateTimeService.cs)
  - Interface: `IUserRepaymentRepository`
- **Infrastructure Layer**:
  - [DateTimeService.cs](file:///d:/CRM/Infrastructure/Services/DateTimeService.cs)
