# Kế hoạch Tổng thể & Chi tiết Phát triển HCRM (Loan Management System)

Để xây dựng một hệ thống Quản lý Khoản vay (Core Lending/Loan Management System) hoàn chỉnh, có khả năng mở rộng và đáp ứng được các nghiệp vụ thực tế phức tạp, chúng ta cần phát triển hệ thống vượt ra khỏi các thao tác CRUD cơ bản.

Dưới đây là **Master Plan** phân rã chi tiết từng module, các Use Case, và kế hoạch implement kỹ thuật (Technical Plan) càng chi tiết càng tốt.

---

## PHẦN 1: PHÂN RÃ CHI TIẾT CÁC MODULE CẦN PHÁT TRIỂN

### 1. Loan Origination (Khởi tạo & Xét duyệt khoản vay)
Hiện tại chúng ta đã có tạo và duyệt cơ bản. Cần bổ sung:
- **eKYC & Tích hợp Credit Score**: Lưu trữ điểm tín dụng (CIC hoặc bên thứ 3) trước khi duyệt.
- **Rules Engine (Chấm điểm tự động)**: Tự động Reject nếu user có nợ xấu (Dựa vào `UserReference` hoặc lịch sử vay trước đó trong hệ thống). Tự động duyệt nếu khoản vay nhỏ và lịch sử tốt.
- **Contract/Document Generation**: Tự động sinh file PDF Hợp đồng vay (Loan Agreement) khi khoản vay được duyệt. Yêu cầu user ký điện tử (e-Signature) hoặc xác nhận OTP.

### 2. Loan Disbursement (Giải ngân)
- **Status Transition**: Từ `Approved` -> `Disbursing` -> `Disbursed`.
- **Payment Gateway/Bank Integration**: Tích hợp API của ngân hàng (vd: Napas, Vietinbank eFAST) để chuyển khoản tự động vào tài khoản của khách hàng.
- **Disbursement Receipt**: Lưu lại mã giao dịch (TxnId) của ngân hàng vào database để đối soát (Reconciliation).

### 3. Repayment Engine (Hệ thống Thanh toán & Thu nợ)
Đây là trái tim của hệ thống Lending. Cần bao phủ các kịch bản:
- **Thanh toán đúng hạn (On-time Payment)**: Trừ vào nợ gốc (Principal) và lãi (Interest).
- **Thanh toán một phần (Partial Payment)**: Khách hàng chỉ trả 1 phần của kỳ. Phần còn lại bị tính lãi phạt.
- **Tất toán trước hạn (Early Settlement/Pre-payment)**: Khách trả toàn bộ nợ trước hạn. Phải tính toán **Phí phạt trả nợ trước hạn** (Pre-payment fee) dựa trên % dư nợ gốc còn lại.
- **Thanh toán dư (Overpayment)**: Tiền dư tự động chuyển vào ví tạm giữ (Suspense Account) để trừ cho kỳ sau.
- **Lịch sử Giao dịch (Transaction Ledger)**: Cần có bảng `LoanTransaction` lưu mọi giao dịch: Giải ngân, Thanh toán gốc, Thanh toán lãi, Đóng phí phạt.

### 4. Overdue & Debt Collection (Xử lý Quá hạn & Đòi nợ)
- **Cấu trúc Nợ (Aging Buckets)**: Phân loại nợ theo tiêu chuẩn ngân hàng:
  - Nợ đủ tiêu chuẩn (Nhóm 1): Quá hạn < 10 ngày.
  - Nợ cần chú ý (Nhóm 2): Quá hạn 10 - 90 ngày.
  - Nợ dưới tiêu chuẩn (Nhóm 3): Quá hạn 91 - 180 ngày.
  - Nợ nghi ngờ (Nhóm 4): 181 - 360 ngày.
  - Nợ có khả năng mất vốn (Nhóm 5): > 360 ngày.
- **Penalty Calculation (Tính lãi phạt)**: 
  - Lãi suất quá hạn trên nợ gốc (thường = 150% lãi suất trong hạn).
  - Lãi chậm trả trên tiền lãi (thường 10%/năm).
- **Escalation Workflow**:
  - Quá hạn 1-3 ngày: Gửi SMS/Zalo/Email tự động.
  - Quá hạn 4-15 ngày: Chuyển task cho bộ phận Call Center (Tạo thực thể `CollectionTask`).
  - Quá hạn > 90 ngày: Bàn giao cho đối tác thu hồi nợ thứ 3.

### 5. Accounting & Ledger (Sổ cái Kế toán)
Để đối soát tài chính với kế toán:
- **Chart of Accounts (Hệ thống tài khoản)**: Định nghĩa các tài khoản: Phải thu khách hàng (AR), Doanh thu tiền lãi, Doanh thu phí phạt.
- **Journal Entries (Bút toán)**: Khi có giao dịch trả nợ, tự động sinh bút toán Nợ/Có.

### 6. Notifications & Webhooks
- **Email/SMS Service**: Nhắc nợ trước 3 ngày, 1 ngày, ngay ngày đến hạn. Nhắc nợ khi quá hạn. (Hiện tại Hangfire đã có, nhưng cần bổ sung Template Engine).

---

## PHẦN 2: KẾ HOẠCH TRIỂN KHAI KỸ THUẬT CHI TIẾT (TECHNICAL ROADMAP)

Dưới đây là kế hoạch code chi tiết, chia thành các Sprint hoặc Task thực thi hàng ngày.

### Ngày 1: Nền tảng Repayment & Ledger (Core Payment Engine)
**1. Database Changes (Entity Framework)**
- Cập nhật Enum `LoanStatus`: `Pending, Approved, Disbursing, Active, Completed, BadDebt, Cancelled, Rejected`.
- Tạo Enum `TransactionType`: `Disbursement, Repayment, EarlySettlement, PenaltyFee`.
- Tạo entity `LoanTransaction`: Lưu log chi tiết mọi dòng tiền ra/vào.
  - `Id`, `LoanId`, `Amount`, `TransactionType`, `ReferenceNumber` (Mã GD ngân hàng), `CreatedAt`, `CreatedBy`.
- Cập nhật entity `UserRepayment`:
  - Thêm `PrincipalAmount` (Nợ gốc kỳ này).
  - Thêm `InterestAmount` (Lãi kỳ này).
  - Thêm `PenaltyAmount` (Lãi phạt).
  - Thêm `PaidAmount` (Số tiền thực tế đã trả cho kỳ này).

**2. API & Application Logic**
- Tạo `PayLoanCommand`: Xử lý logic chia tiền (Waterfall logic). Tiền khách trả sẽ trừ theo thứ tự: **Phí phạt -> Lãi -> Gốc**.
- Cập nhật trạng thái `UserRepayment` (đổi thành `Partial` hoặc `Paid`).
- Cập nhật tổng `Paid` trong bảng `Loan`.
- Ghi 1 record vào `LoanTransaction`.

### Ngày 2: Xử lý Tiền phạt, Quá hạn & Background Jobs (Overdue Processing)
**1. Formulas (Domain Services)**
- Viết `PenaltyCalculationService` dựa vào luật (Ví dụ: Số ngày quá hạn * (150% * Lãi suất vay) / 365).
**2. Hangfire Jobs**
- Viết `DailyOverdueProcessorJob`: 
  - Fetch tất cả `UserRepayment` có `RepaymentDate < Today` và `Status != Paid`.
  - Gọi `PenaltyCalculationService` tính tiền phạt.
  - Update `PenaltyAmount` vào DB.
  - Đổi trạng thái `Loan` thành `Overdue` nếu đây là khoản vay mới bị quá hạn lần đầu.
- Viết `NotificationTriggerJob`: Quét các khoản sắp đến hạn để đẩy message vào **RabbitMQ**. Message queue consumer sẽ lấy message và gọi Email/SMS API.

### Ngày 3: Tích hợp Cổng thanh toán (Payment Gateway VNPAY/MoMo)
**1. Infrastructure Layer**
- Viết `IVNPayService` implement logic tạo HMAC SHA512 signature, build URL chuyển hướng sang VNPAY.
**2. API Controllers**
- `POST /api/v1/payment/create-url`: User truyền vào `UserRepaymentId` -> Trả về URL VNPAY.
- `GET /api/v1/payment/vnpay-return`: Xử lý khi user thanh toán xong bị redirect về web.
- `POST /api/v1/payment/vnpay-ipn`: Endpoint cực kỳ quan trọng cho Server-to-Server. Chặn các request giả mạo (validate signature). Nếu hợp lệ, tự động dispatch `PayLoanCommand` (tái sử dụng logic Ngày 1). Chặn race condition bằng Redis Lock (`IRedisService` - dùng Redlock hoặc cờ `LockKey:LoanId`).

### Ngày 4: Giải ngân & Quản lý Hợp đồng (Disbursement & Documents)
**1. Disbursement Workflow**
- Admin gọi API `POST /api/v1/loan/{id}/disburse`.
- Tích hợp mock API chuyển khoản. Cập nhật `LoanStatus = Active`. Sinh ra các dòng lịch trả nợ (`UserRepayment`) tương ứng với Term (Ví dụ 12 tháng sinh ra 12 record). Trước đây nếu tạo lúc Create thì giờ dời sang lúc Disburse.
**2. Document Generator**
- Dùng thư viện `iText7` hoặc `DinkToPdf` hoặc Razor HTML to PDF.
- Inject dữ liệu user và loan vào HTML template.
- Lưu file PDF vào MinIO / AWS S3 hoặc ổ cứng local, lưu URL vào bảng `Loan`.

### Ngày 5: Đòi nợ & Dashboard Thống kê (Collection & Analytics)
**1. Collection Features**
- Tạo entity `CollectionTask`. Khi khoản vay quá hạn 5 ngày, sinh Task cho Role `CallCenterAgent`.
- API để Agent ghi log cuộc gọi: "Khách hẹn mùng 5 trả", "Không nghe máy", v.v.
**2. Dashboard Queries (CQRS)**
- Dùng Dapper hoặc EF Core GroupBy để lấy số liệu tốc độ cao.
- `GetAdminDashboardSummaryQuery`:
  - `TotalDisbursedAmount` (Tháng này, Năm này).
  - `TotalCollectionAmount` (Thu nợ thực tế).
  - `NPL (Non-performing loan) Ratio`: Tỷ lệ nợ xấu = Tổng dư nợ nhóm 3,4,5 / Tổng dư nợ hiện tại.
  - Biểu đồ line: Tiền giải ngân vs Tiền thu hồi theo 7 ngày gần nhất.

---

## KIẾN TRÚC & BEST PRACTICES CẦN ÁP DỤNG THÊM

- **Concurrency & Idempotency**: Khi xử lý thanh toán IPN từ VNPAY, Webhook có thể gọi lại nhiều lần. Phải đảm bảo tính **Idempotent** (Xử lý 1 lần duy nhất) bằng cách kiểm tra `ReferenceNumber` đã tồn tại trong `LoanTransaction` hay chưa.
- **Distributed Tracing**: Bổ sung Serilog hoặc OpenTelemetry để trace Request từ Controller -> MediatR -> DB -> External Gateway. Điều này bắt buộc đối với hệ thống tài chính để tìm ra lỗi khi tiền không khớp.
- **Audit Logging**: Mọi thao tác đổi trạng thái (Duyệt, Giải ngân) phải lưu vào bảng `AuditLog` (Who, When, OldValue, NewValue). Có thể dùng Entity Framework Core Interceptors để bắt tự động.
- **Caching Optimization**: Dùng `IRedisService` để cache lại Bảng Lãi suất, Config của hệ thống thay vì query DB liên tục.
- **Unit Testing**: Bắt buộc viết Unit Test cho `PayLoanCommand` (Waterfall logic chia tiền) và `PenaltyCalculationService` (Tính lãi phạt) vì đây là nơi dễ tính sai tiền nhất.

> Kế hoạch này được thiết kế để bạn có thể scale hệ thống từ một project nhỏ thành một Core System chuẩn ngân hàng/Fintech. Bạn có thể chọn pick nghiệm vụ ở **Ngày 1 và Ngày 2** làm trước mắt để hoàn thiện MVP (Minimum Viable Product).
