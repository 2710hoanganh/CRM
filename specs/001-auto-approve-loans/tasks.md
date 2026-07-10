# Tasks: Credit Score Auto Approval

**Input**: Design documents from `/specs/001-auto-approve-loans/`

**Prerequisites**: plan.md (required), spec.md (required for user stories)

## Phase 1: Foundational (Blocking Prerequisites)

**Purpose**: Core model and database schema updates

- [x] T001 Add CreditScore to User entity in `Domain/Entities/User.cs`
- [x] T002 Add HighCreditScoreThreshold constant to AppConstants in `Domain/Constants/AppConstants.cs`
- [x] T003 Generate and apply EF Core database migrations

---

## Phase 2: User Story 1 & 3 - Auto Approval & Email Notification (Priority: P1, P3)

**Goal**: Auto-approve loans for users with CreditScore >= 700 and send email notification.

- [x] T004 Create `IEmailService.cs` under `Application/Services/`
- [x] T005 Create `EmailService.cs` under `Infrastructure/Services/`
- [x] T006 Register `IEmailService` in `Infrastructure/DependencyInjection.cs`
- [x] T007 Update `CreateLoanCommand.cs` to inject `IUserRepository` & `IEmailService`, query user's CreditScore, auto-approve, and send email if high.
- [x] T008 Verify auto-approval logic and email notification simulation logs.
