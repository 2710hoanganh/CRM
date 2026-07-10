using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.Loan;
using Application.Repositories.Base;
using Application.Repositories;
using Domain.Constants.AppEnum;
using Application.Services.Base;
using Application.Services;

namespace Application.Features.Loan.Command
{
    public class CreateLoanCommand : BaseFields, IRequest<Response<bool>>
    {
        public required CreateLoanRequest Request { get; set; }

        public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, Response<bool>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILoanRepository _loanRepository;
            private readonly ILoanInterestRate _loanInterestRate;
            private readonly IUserRepaymentRepository _userRepaymentRepository;
            private readonly IUserReferenceRepository _userReferenceRepository;
            private readonly IDateTimeService _dateTimeService;
            private readonly IUserRepository _userRepository;
            private readonly IEmailService _emailService;
            public CreateLoanCommandHandler(IUnitOfWork unitOfWork, ILoanRepository loanRepository, ILoanInterestRate loanInterestRate, IUserRepaymentRepository userRepaymentRepository, IDateTimeService dateTimeService, IUserReferenceRepository userReferenceRepository, IUserRepository userRepository, IEmailService emailService)
            {
                _unitOfWork = unitOfWork;
                _loanRepository = loanRepository;
                _loanInterestRate = loanInterestRate;
                _userRepaymentRepository = userRepaymentRepository;
                _dateTimeService = dateTimeService;
                _userReferenceRepository = userReferenceRepository;
                _userRepository = userRepository;
                _emailService = emailService;
            }

            public async Task<Response<bool>> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    // check user ref berfor create loan
                    var userRef = await _userReferenceRepository.Find(x => x.UserId == request.Id, include: null, asNoTracking: true, cancellationToken: cancellationToken);
                    if (!userRef)
                    {
                        return new Response<bool>(ResponseResult.ERROR, Domain.Constants.Error.ReferenceRequired, false, null);
                    }

                    // check user credit score for auto-approval
                    var user = await _userRepository.GetById(request.Id, cancellationToken);
                    var isAutoApproved = user != null && user.CreditScore >= Domain.Constants.AppConstants.AppConstants.HighCreditScoreThreshold;

                    var interestRate = await _loanInterestRate.CalculateInterestRate(request.Request.LoanTerm, (int)LoanRate.BaseRate, cancellationToken);
                    var total = await _loanInterestRate.CalculateTotal(request.Request.LoanAmount, request.Request.LoanTerm, interestRate, cancellationToken);

                    var transaction = await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, Guid.NewGuid(), cancellationToken);
                    try
                    {
                        // Create loan
                        var loan = new Domain.Entities.Loan
                        {
                            Amount = request.Request.LoanAmount,
                            Term = request.Request.LoanTerm,
                            UserId = request.Id,
                            Status = isAutoApproved ? (int)LoanStatus.Approved : (int)LoanStatus.Pending,
                            FeedBack = isAutoApproved ? $"Auto-approved due to high credit score (Score: {user?.CreditScore})." : null,
                            Rate = (int)LoanRate.BaseRate,
                            InterestRate = interestRate,
                            EndDate = DateTime.Now.AddMonths(request.Request.LoanTerm),
                            Total = total,
                            PaybackAmount = Math.Round(total / request.Request.LoanTerm, 2),
                        };
                        await _loanRepository.Add(loan, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync(transactionId: transaction, cancellationToken);
                    }
                    catch (System.Exception)
                    {
                        await _unitOfWork.RollbackTransactionAsync(transactionId: transaction, cancellationToken);
                        throw;
                    }

                    if (isAutoApproved && user != null)
                    {
                        await _emailService.SendEmailAsync(user.Email, "Loan Approved", $"Dear {user.FullName}, your loan request of {request.Request.LoanAmount} has been automatically approved based on your high credit score of {user.CreditScore}.");
                    }

                    return new Response<bool>(ResponseResult.SUCCESS, "Loan created successfully", true, null);
                }
                catch (Exception ex)
                {
                    return new Response<bool>(ResponseResult.ERROR, ex.Message, false, null);
                }
            }
        }
    }
}