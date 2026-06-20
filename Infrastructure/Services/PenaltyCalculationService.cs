using Application.Services;
using Application.Repositories;
using Application.Repositories.Base;
using Domain.Constants.AppEnum;

namespace Infrastructure.Services
{
    public class PenaltyCalculationService : IPenaltyCalculationService
    {
        private readonly IUserRepaymentRepository _userRepaymentRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PenaltyCalculationService(
            IUserRepaymentRepository userRepaymentRepository,
            ILoanRepository loanRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepaymentRepository = userRepaymentRepository;
            _loanRepository = loanRepository;
            _unitOfWork = unitOfWork;
        }

        public decimal CalculatePenalty(decimal principal, decimal interestRate, int overdueDays)
        {
            // Số ngày quá hạn * (150% * Lãi suất vay) / 365
            decimal penaltyRate = interestRate * 1.5m / 100m;
            decimal penalty = overdueDays * penaltyRate * principal / 365m;
            return Math.Round(penalty, 2);
        }

        public async Task ProcessOverdueRepaymentsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            
            var overdueRepayments = await _userRepaymentRepository.GetOverdueRepaymentsWithLoanAsync(today, cancellationToken);

            foreach (var repayment in overdueRepayments)
            {
                var overdueDays = (today - repayment.RepaymentDate.Date).Days;
                if (overdueDays > 0)
                {
                    decimal interestRate = repayment.Loan.InterestRate; 
                    repayment.PenaltyAmount = CalculatePenalty(repayment.PrincipalAmount, interestRate, overdueDays);
                    repayment.Status = (int)UserRepatmentStatus.Overdue;
                    
                    if (repayment.Loan.Status == (int)LoanStatus.Active)
                    {
                        repayment.Loan.Status = (int)LoanStatus.Overdue;
                        await _loanRepository.Update(repayment.Loan, cancellationToken);
                    }
                    
                    await _userRepaymentRepository.Update(repayment, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
