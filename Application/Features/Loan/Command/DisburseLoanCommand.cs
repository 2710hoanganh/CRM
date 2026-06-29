using MediatR;
using Domain.Constants.AppEnum;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Application.Services.Base;
using Application.Repositories;
using Application.Repositories.Base;

namespace Application.Features.Loan.Command
{
    public class DisburseLoanCommand : IRequest<bool>
    {
        [Required]
        public int LoanId { get; set; }
    }

    public class DisburseLoanCommandHandler : IRequestHandler<DisburseLoanCommand, bool>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepaymentRepository _userRepaymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;

        public DisburseLoanCommandHandler(
            ILoanRepository loanRepository,
            IUserRepaymentRepository userRepaymentRepository,
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService)
        {
            _loanRepository = loanRepository;
            _userRepaymentRepository = userRepaymentRepository;
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
        }

        public async Task<bool> Handle(DisburseLoanCommand request, CancellationToken cancellationToken)
        {
            var loan = await _loanRepository.GetById(request.LoanId, cancellationToken);

            if (loan == null)
            {
                throw new Exception("Loan not found");
            }

            if (loan.Status != (int)LoanStatus.Approved)
            {
                throw new Exception("Loan must be approved before disbursement");
            }

            loan.Status = (int)LoanStatus.Active;

            decimal monthlyPrincipal = loan.Amount / loan.Term;
            decimal monthlyInterest = (loan.Total - loan.Amount) / loan.Term;

            List<UserRepayment> userRepayments = new List<UserRepayment>();
            for (int i = 0; i < loan.Term; i++)
            {
                var item = new UserRepayment
                {
                    LoanId = loan.Id,
                    RepaymentDate = await _dateTimeService.GetRepaymentDate(DateTime.Now, i + 1, cancellationToken),
                    Status = (int)UserRepatmentStatus.Pending,
                    PrincipalAmount = monthlyPrincipal,
                    InterestAmount = monthlyInterest,
                    PenaltyAmount = 0,
                    PaidAmount = 0
                };
                userRepayments.Add(item);
            }
            
            await _userRepaymentRepository.AddRange(userRepayments, cancellationToken);
            await _loanRepository.Update(loan, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
