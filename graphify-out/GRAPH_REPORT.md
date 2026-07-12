# Graph Report - .  (2026-07-10)

## Corpus Check
- 1 files · ~51,720 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 85 nodes · 65 edges · 24 communities (18 shown, 6 thin omitted)
- Extraction: 95% EXTRACTED · 5% INFERRED · 0% AMBIGUOUS · INFERRED: 3 edges (avg confidence: 0.95)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- Authentication
- Persistence Setup
- Notifications
- Background Jobs
- User References
- Payment Processing
- Payment Gateway
- Disbursement
- User Info
- Loan Creation
- Loan Review
- User Loans
- Overdue Processing
- Collection Analytics
- Loan Info
- All Loans Admin
- Loan Repayment Date
- Clean Architecture Setup
- Docker Services
- Application Layer Setup
- Repayment Engine Planning
- Domain Layer Setup
- Credit Score Specs

## God Nodes (most connected - your core abstractions)
1. `Background Jobs` - 4 edges
2. `GetUserReference` - 3 edges
3. `AuthController` - 3 edges
4. `AuthController` - 3 edges
5. `GET /api/v1/user-reference/get-all` - 2 edges
6. `UserReferenceController` - 2 edges
7. `GET /api/v1/notification/list` - 2 edges
8. `NotifcationController` - 2 edges
9. `ListNoitifiaction` - 2 edges
10. `PaymentController` - 2 edges

## Surprising Connections (you probably didn't know these)
- `Clean Architecture` --semantically_similar_to--> `Clean Architecture`  [INFERRED] [semantically similar]
  .specify/memory/constitution.md → doc/README.md
- `AuthController` --semantically_similar_to--> `AuthController`  [INFERRED] [semantically similar]
  doc/plan/1_register.md → doc/plan/2_login.md
- `Application Layer (CQRS)` --semantically_similar_to--> `Application Layer`  [INFERRED] [semantically similar]
  spec/knowledge_graph.md → spec/constitution.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Loan Controllers Group** — doc_plan_4_create_loan_loancontroller, doc_plan_5_review_loan_loancontroller, doc_plan_6_get_loan_info_loancontroller, doc_plan_7_get_all_loans_loancontroller, doc_plan_8_get_user_loans_loancontroller, doc_plan_9_get_loan_repayment_date_loancontroller, doc_plan_17_disbursement_documents_loancontroller [INFERRED 0.95]

## Communities (24 total, 6 thin omitted)

### Community 0 - "Authentication"
Cohesion: 0.25
Nodes (8): POST /api/v1/auth/register, AuthController, RegisterCommand, User Registration, POST /api/v1/auth/login, AuthController, LoginQuery, User Login

### Community 1 - "Persistence Setup"
Cohesion: 0.33
Nodes (4): Persistence.DependencyInjection, IConfiguration, IServiceCollection, DependencyInjection

### Community 2 - "Notifications"
Cohesion: 0.40
Nodes (5): GET /api/v1/notification/list, INotificationRepository, List Notifications, ListNoitifiaction, NotifcationController

### Community 3 - "Background Jobs"
Cohesion: 0.40
Nodes (5): Background Jobs, IHangFireService, IRecurringJobRegistrar, reminder-loan-repayment-3-days, test-hourly

### Community 5 - "User References"
Cohesion: 0.67
Nodes (4): GET /api/v1/user-reference/get-all, GetUserReference, IUserReferenceRepository, UserReferenceController

### Community 6 - "Payment Processing"
Cohesion: 0.50
Nodes (4): Core Payment Engine, LoanTransaction, PayLoanCommand, PaymentController

### Community 7 - "Payment Gateway"
Cohesion: 0.50
Nodes (4): POST /api/v1/payment/create-url, IVNPayService, Payment Gateway, PaymentController

### Community 8 - "Disbursement"
Cohesion: 0.50
Nodes (4): POST /api/v1/loan/{id}/disburse, DisburseLoanCommand, Disbursement & Documents, LoanController

### Community 9 - "User Info"
Cohesion: 0.50
Nodes (4): AccountController, GET /api/v1/account/info, Get User Info, GetUserInfoQuery

### Community 10 - "Loan Creation"
Cohesion: 0.50
Nodes (4): POST /api/v1/loan/create, Create Loan, CreateLoanCommand, LoanController

### Community 11 - "Loan Review"
Cohesion: 0.50
Nodes (4): POST /api/v1/loan/review, LoanController, Review Loan, ReviewLoanCommand

### Community 12 - "User Loans"
Cohesion: 0.50
Nodes (4): GET /api/v1/loan/all-user, Get User Loans, GetAllUserLoan, LoanController

### Community 13 - "Overdue Processing"
Cohesion: 0.67
Nodes (3): DailyOverdueProcessorJob, IPenaltyCalculationService, Overdue Processing

### Community 14 - "Collection Analytics"
Cohesion: 0.67
Nodes (3): Collection & Analytics, CollectionTask, GetAdminDashboardSummaryQuery

### Community 15 - "Loan Info"
Cohesion: 1.00
Nodes (3): GET /api/v1/loan/info, GetLoanInfo, LoanController

### Community 16 - "All Loans Admin"
Cohesion: 1.00
Nodes (3): GET /api/v1/loan/all-admin, GetAllLoan, LoanController

### Community 17 - "Loan Repayment Date"
Cohesion: 1.00
Nodes (3): GET /api/v1/loan/repayment, GetLoanRepaymentDate, LoanController

## Knowledge Gaps
- **39 isolated node(s):** `Clean Architecture`, `Clean Architecture`, `clean-app Service`, `sqlserver Service`, `Repayment Engine` (+34 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **6 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **What connects `Clean Architecture`, `Clean Architecture`, `clean-app Service` to the rest of the system?**
  _39 weakly-connected nodes found - possible documentation gaps or missing edges._