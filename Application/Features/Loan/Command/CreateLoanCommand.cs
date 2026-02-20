using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.Loan;
using Application.Repositories.Base;
using Application.Repositories;
using Domain.Constants.AppEnum;
using Domain.Entities;
using Application.Services.Base;

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
            private readonly IDateTimeService _dateTimeService;
            public CreateLoanCommandHandler(IUnitOfWork unitOfWork, ILoanRepository loanRepository, ILoanInterestRate loanInterestRate, IUserRepaymentRepository userRepaymentRepository, IDateTimeService dateTimeService)
            {
                _unitOfWork = unitOfWork;
                _loanRepository = loanRepository;
                _loanInterestRate = loanInterestRate;
                _userRepaymentRepository = userRepaymentRepository;
                _dateTimeService = dateTimeService;
            }

            public async Task<Response<bool>> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
            {
                try
                {
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
                            Status = (int)LoanStatus.Pending,
                            Rate = (int)LoanRate.BaseRate,
                            InterestRate = interestRate,
                            EndDate = DateTime.Now.AddMonths(request.Request.LoanTerm),
                            Total = total,
                            PaybackAmount = Math.Round(total / request.Request.LoanTerm, 2),
                        };
                        await _loanRepository.Add(loan, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        // create repatment plan base on term 
                        _ = Task.Run(async () =>
                        {
                            List<UserRepayment> userRepayments = new List<UserRepayment>();
                            for (int i = 0; i < request.Request.LoanTerm; i++)
                            {
                                var item = new UserRepayment
                                {
                                    LoanId = loan.Id,
                                    RepaymentDate = await _dateTimeService.GetRepaymentDate(DateTime.Now, i + 1, cancellationToken),
                                    Status = (int)UserRepatmentStatus.Pending,
                                };
                                userRepayments.Add(item);
                            }
                            await _userRepaymentRepository.AddRange(userRepayments, cancellationToken);
                            await _unitOfWork.SaveChangesAsync(cancellationToken);
                        });

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync(transactionId: transaction, cancellationToken);
                    }
                    catch (System.Exception)
                    {
                        await _unitOfWork.RollbackTransactionAsync(transactionId: transaction, cancellationToken);
                        throw;
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