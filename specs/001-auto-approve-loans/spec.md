# Feature Specification: Auto Approve Loans for High Credit Score Users

**Feature Branch**: `001-auto-approve-loans`

**Created**: 2026-06-28

**Status**: Draft

**Input**: User description: "Auto approve loans for high credit score users"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Auto Approval for High Credit Score (Priority: P1)

A user with a high credit score ($\ge 700$) applies for a loan. Instead of waiting in a `Pending` state for manual review, the loan is automatically approved immediately upon registration.

**Why this priority**: Core requirement of the feature.

**Independent Test**: Can be tested by creating a user with a Credit Score of 750, submitting a loan request, and verifying that the loan status is immediately set to `Approved` in the database, with automatic feedback stating it was auto-approved.

**Acceptance Scenarios**:

1. **Given** a user has a Credit Score of 750, **When** they request a loan, **Then** the loan status is immediately `Approved` (1) and `FeedBack` contains "Auto-approved due to high credit score".
2. **Given** a user has a Credit Score of 700, **When** they request a loan, **Then** the loan status is immediately `Approved` (1).

---

### User Story 2 - Standard Pending State for Lower Credit Score (Priority: P2)

A user with a normal or low credit score ($< 700$) applies for a loan. The loan must go to the standard `Pending` state for manual review by an admin.

**Why this priority**: Essential to ensure safety and prevent unauthorized auto-approvals for risky users.

**Independent Test**: Can be tested by creating a user with a Credit Score of 699 or below (or default 0), submitting a loan request, and verifying that the loan status remains `Pending` (0).

**Acceptance Scenarios**:

1. **Given** a user has a Credit Score of 650, **When** they request a loan, **Then** the loan status is `Pending` (0) and `FeedBack` is empty.

---

### User Story 3 - Send Email upon Auto Approval (Priority: P3)

When a loan is auto-approved due to a high credit score, the system MUST send an email to the user notifying them of the successful approval.

**Why this priority**: Enhances user notification flow for instant approvals.

**Independent Test**: Verify that when a user with CreditScore >= 700 applies for a loan, an email sending task is triggered and logged with the user's email, a subject of "Loan Approved", and appropriate message details.

**Acceptance Scenarios**:

1. **Given** a user has a Credit Score of 750, **When** they request a loan, **Then** an email notification is sent to the user's email address.

---

### Edge Cases

- **User has no Credit Score (default/null)**: The system should treat this as a Credit Score of `0` and place the loan in a `Pending` state.
- **Credit Score is boundary (700)**: Auto-approval triggers correctly since the threshold is inclusive ($\ge 700$).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST store a user's credit score (`CreditScore`) in the database.
- **FR-002**: The threshold for a high credit score MUST be configurable/defined as `HighCreditScoreThreshold = 700` in the application constants.
- **FR-003**: During the loan creation workflow, if the user's `CreditScore` is $\ge$ `HighCreditScoreThreshold`, the system MUST set the loan's status directly to `Approved` (1) instead of `Pending` (0).
- **FR-004**: If auto-approved, the system MUST record a feedback message: `"Auto-approved due to high credit score (Score: X)"` in the loan record.
- **FR-005**: If the user's credit score is less than the threshold, the system MUST default the loan status to `Pending` (0) and leave the feedback field blank.
- **FR-006**: The system MUST trigger an email notification to the user's registered email when their loan is auto-approved.

### Key Entities *(include if feature involves data)*

- **User**: Repesent a system user, extended to include:
  - `CreditScore` (`int`): The credit score of the user.
- **Loan**: Represents the loan request, containing:
  - `Status` (`int`): Mapped to `LoanStatus` enum.
  - `FeedBack` (`string`): Notes or feedback, populated during auto-approval.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of loan requests from users with `CreditScore >= 700` are auto-approved.
- **SC-002**: 100% of loan requests from users with `CreditScore < 700` are placed in `Pending` state.
- **SC-003**: Zero database schema issues or compiler warnings added to the build.
- **SC-004**: Auto-approved users receive an email notification indicating approval.

## Assumptions

- We assume credit scores are updated either during registration/KYC or mocked, and the DB migration will initialize existing users to a default score of `0` or appropriate default value.
