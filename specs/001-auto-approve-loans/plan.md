# Implementation Plan: Credit Score Auto Approval

**Branch**: `001-auto-approve-loans` | **Date**: 2026-06-28 | **Spec**: [spec.md](file:///d:/CRM/specs/001-auto-approve-loans/spec.md)

**Input**: Feature specification from `specs/001-auto-approve-loans/spec.md`

## Summary

We will add a `CreditScore` property to the `User` entity, define `HighCreditScoreThreshold` in constants, and update the `CreateLoanCommand` handler to automatically set the loan status to `Approved` with a feedback log if the user's credit score is high ($\ge 700$). Additionally, if a loan is auto-approved, an email notification will be sent to the user using an asynchronous email notification service (`IEmailService`).

## Technical Context

**Language/Version**: C# / .NET 8.0

**Primary Dependencies**: EF Core 8.0, MediatR, Microsoft.Extensions.Logging

**Storage**: SQL Server

**Testing**: Manual test requests

**Project Type**: Web API (ASP.NET Core)

## Proposed Changes

### Domain

#### [MODIFY] [User.cs](file:///d:/CRM/Domain/Entities/User.cs)
- Add `public int CreditScore { get; set; }` to the `User` class.

#### [MODIFY] [AppConstants.cs](file:///d:/CRM/Domain/Constants/AppConstants.cs)
- Add `public const int HighCreditScoreThreshold = 700;` to `AppConstants`.

### Application

#### [NEW] [IEmailService.cs](file:///d:/CRM/Application/Services/IEmailService.cs)
- Create interface defining `Task SendEmailAsync(string receiverEmail, string subject, string body);`.

#### [MODIFY] [CreateLoanCommand.cs](file:///d:/CRM/Application/Features/Loan/Command/CreateLoanCommand.cs)
- Inject `IUserRepository` and `IEmailService` into `CreateLoanCommandHandler`.
- Fetch the user using `_userRepository.GetById(request.Id, cancellationToken)`.
- If the user's `CreditScore` is $\ge$ `AppConstants.HighCreditScoreThreshold`, set the loan `Status` to `(int)LoanStatus.Approved` and `FeedBack` to `"Auto-approved due to high credit score (Score: X)"`.
- If auto-approved, trigger `_emailService.SendEmailAsync(user.Email, "Loan Approved", "Your loan request has been automatically approved based on your high credit score.")`.

### Infrastructure

#### [NEW] [EmailService.cs](file:///d:/CRM/Infrastructure/Services/EmailService.cs)
- Implement `IEmailService` using `ILogger<EmailService>` to log details for simulation.

#### [MODIFY] [DependencyInjection.cs](file:///d:/CRM/Infrastructure/DependencyInjection.cs)
- Register `IEmailService` as scoped: `services.AddScoped<IEmailService, EmailService>();`.

## Verification Plan

### Automated Tests
- Run `dotnet build` to verify the codebase compiles successfully.
- Run Entity Framework migrations to verify DB update.

### Manual Verification
- Verify that when a user with CreditScore >= 700 requests a loan:
  - Loan is auto-approved.
  - The email dispatch log output is printed in the server logs.
