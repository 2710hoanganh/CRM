# Kế hoạch Triển khai: Nền tảng Repayment & Ledger (Core Payment Engine)

## 1. Tổng quan
Tính năng quản lý thanh toán nợ và sổ cái (Core Payment Engine) là trái tim của hệ thống Lending, giúp theo dõi, xử lý và lưu trữ chi tiết các dòng tiền ra/vào.

## 2. Thay đổi Database (Entity Framework)
- **Cập nhật Enum `LoanStatus`**: Thêm các trạng thái `Pending, Approved, Disbursing, Active, Completed, BadDebt, Cancelled, Rejected`.
- **Tạo Enum `TransactionType`**: Bao gồm `Disbursement, Repayment, EarlySettlement, PenaltyFee`.
- **Tạo entity mới `LoanTransaction`**: Lưu log chi tiết giao dịch.
  - Các trường: `Id`, `LoanId`, `Amount`, `TransactionType`, `ReferenceNumber` (Mã GD ngân hàng), `CreatedAt`, `CreatedBy`.
- **Cập nhật entity `UserRepayment`**:
  - `PrincipalAmount` (Nợ gốc kỳ này).
  - `InterestAmount` (Lãi kỳ này).
  - `PenaltyAmount` (Lãi phạt).
  - `PaidAmount` (Số tiền thực tế đã trả cho kỳ này).

## 3. API & Quy trình xử lý (Business Logic)
- **PayLoanCommand**: 
  - Xử lý logic chia tiền (Waterfall logic). 
  - Tiền khách trả sẽ trừ theo thứ tự ưu tiên: **Phí phạt -> Lãi -> Gốc**.
  - Cập nhật trạng thái `UserRepayment` thành `Partial` hoặc `Paid`.
  - Cập nhật tổng số tiền `Paid` trong bảng `Loan`.
  - Ghi 1 bản ghi vào bảng `LoanTransaction`.

## 4. Kịch bản Kiểm thử (Test Cases)
- **TC1: Thanh toán đầy đủ kỳ hạn**: Gửi số tiền bằng tổng (Gốc + Lãi + Phạt), kiểm tra trạng thái cập nhật thành `Paid` và ghi log đúng.
- **TC2: Thanh toán một phần (Partial)**: Gửi số tiền nhỏ hơn tổng cần trả, kiểm tra Waterfall logic trừ đúng thứ tự Phạt -> Lãi -> Gốc, và cập nhật trạng thái `Partial`.
- **TC3: Kiểm tra lưu vết**: Xác nhận mỗi giao dịch đều sinh ra đúng 1 record trong `LoanTransaction`.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [PaymentController.cs](file:///d:/CRM/Presentation/Controllers/PaymentController.cs)
- **Application Layer**:
  - [PayLoanCommand.cs](file:///d:/CRM/Application/Features/Loan/Command/PayLoanCommand.cs)
  - Interfaces: `IUserRepaymentRepository`, `ILoanRepository`, `ILoanTransactionRepository`, `IUnitOfWork`
- **Domain Layer**:
  - [LoanTransaction.cs](file:///d:/CRM/Domain/Entities/LoanTransaction.cs)
  - [UserRepayment.cs](file:///d:/CRM/Domain/Entities/UserRepayment.cs)
  - [Loan.cs](file:///d:/CRM/Domain/Entities/Loan.cs)
- **Persistence Layer**:
  - [LoanTransactionRepository.cs](file:///d:/CRM/Persistence/Repositories/LoanTransactionRepository.cs)
