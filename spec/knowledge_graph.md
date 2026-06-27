# System Knowledge Graph (HCRM)

Bản đồ tri thức (Knowledge Graph) dưới đây thể hiện sự liên kết giữa các Thành phần Đặc tả (Spec), Lớp Giao tiếp (API), Lớp Xử lý (Application CQRS), và Lớp Dữ liệu (Entities). 

> **Mục đích:** Giúp lập trình viên và AI hiểu rõ hệ sinh thái của một tính năng (Traceability) khi cần thay đổi hoặc nâng cấp, ngăn chặn hiện tượng sửa chỗ này hỏng chỗ khác (regression bugs).

## Tổng quan Kiến trúc bằng Mermaid

```mermaid
graph TD
    %% 1. Khối Đặc Tả (Specs / Use Cases)
    subgraph Specs [Spec-Driven Layer (doc/plan/)]
        S_Auth[1_register.md / 2_login.md]
        S_Loan[4_create_loan.md / 5_review_loan.md]
        S_Repay[9_get_loan_repayment_date.md]
        S_Pay[Payment Webhooks / IPN]
    end

    %% 2. Khối Trình diễn (Presentation Layer)
    subgraph Controllers [Presentation Layer (Controllers)]
        C_Auth[AuthController]
        C_Loan[LoanController]
        C_Pay[PaymentController]
    end

    %% 3. Khối Nghiệp vụ (Application Layer - MediatR)
    subgraph AppLayer [Application Layer (CQRS)]
        Cmd_Reg[RegisterCommand / LoginQuery]
        Cmd_Loan[CreateLoanCommand / ReviewLoanCommand]
        Q_Repay[GetLoanRepaymentDateQuery]
        Cmd_Pay[PayLoanCommand]
    end

    %% 4. Khối Giao tiếp Ngoại vi (Infrastructure)
    subgraph Infra [Infrastructure Layer]
        Svc_Hash[HashingService BCrypt]
        Svc_VNPay[VNPayService]
        Svc_Token[TokenService JWT]
        Svc_Hangfire[DailyOverdueProcessorJob]
    end

    %% 5. Khối Lưu trữ (Domain & Persistence)
    subgraph DB [Domain Entities / DB Tables]
        E_User[(Users)]
        E_Loan[(Loans)]
        E_Repay[(UserRepayments)]
        E_Txn[(LoanTransactions)]
    end

    %% Liên kết luồng Auth
    S_Auth -.-> C_Auth
    C_Auth --> Cmd_Reg
    Cmd_Reg --> Svc_Hash
    Cmd_Reg --> Svc_Token
    Cmd_Reg --> E_User

    %% Liên kết luồng Loan
    S_Loan -.-> C_Loan
    C_Loan --> Cmd_Loan
    Cmd_Loan --> E_Loan
    Cmd_Loan --> E_Repay

    %% Liên kết luồng Lịch Trả Nợ
    S_Repay -.-> C_Loan
    C_Loan --> Q_Repay
    Q_Repay --> E_Repay

    %% Liên kết luồng Thanh toán (VNPay IPN)
    S_Pay -.-> C_Pay
    C_Pay --> Svc_VNPay
    C_Pay --> Cmd_Pay
    Cmd_Pay --> E_Txn
    Cmd_Pay --> E_Repay
    Cmd_Pay --> E_Loan
    
    %% Background Jobs Update Overdue
    Svc_Hangfire --> E_Loan
    Svc_Hangfire --> E_Repay
```

### Hướng dẫn Đọc Biểu Đồ
1. **[Spec-Driven Layer]:** Điểm bắt đầu của mọi thay đổi (Source of Truth).
2. **[Presentation Layer]:** Các điểm tiếp nhận HTTP Request. Nếu thay đổi Response, OpenAPI (Swagger) cũng sẽ thay đổi.
3. **[Application Layer]:** Chứa logic tính toán tiền, tạo dữ liệu. Nơi chứa business rule (ví dụ: cấm tạo loan khi đang có nợ xấu).
4. **[Domain/DB]:** Dữ liệu gốc lưu trữ trên SQL Server. 
