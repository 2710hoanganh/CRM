using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.Loan;
using Application.Repositories.Base;
using Application.Repositories;
using Domain.Constants.AppEnum;

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
            public CreateLoanCommandHandler(IUnitOfWork unitOfWork, ILoanRepository loanRepository, ILoanInterestRate loanInterestRate)
            {
                _unitOfWork = unitOfWork;
                _loanRepository = loanRepository;
                _loanInterestRate = loanInterestRate;
            }

            public async Task<Response<bool>> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var interestRate = await _loanInterestRate.CalculateInterestRate(request.Request.LoanTerm, (int)request.Request.LoanRate, cancellationToken);
                    var total = await _loanInterestRate.CalculateTotal(request.Request.LoanAmount, request.Request.LoanTerm, interestRate, cancellationToken);
                    //loan term in months
                    var loan = new Domain.Entities.Loan
                    {
                        Amount = request.Request.LoanAmount,
                        Term = request.Request.LoanTerm,
                        UserId = request.Id,
                        Status = (int)LoanStatus.Pending,
                        Rate = (int)request.Request.LoanRate,
                        InterestRate = interestRate,
                        Total = total,
                        PaybackAmount = Math.Round(total / request.Request.LoanTerm, 2),
                    };
                    await _loanRepository.Add(loan, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    
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