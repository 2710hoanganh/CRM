using MediatR;
using Domain.Constants.AppEnum;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Application.Repositories;
using Application.Repositories.Base;

namespace Application.Features.Loan.Command
{
    public class PayLoanCommand : IRequest<bool>
    {
        [Required]
        public int UserRepaymentId { get; set; }
        
        [Required]
        public decimal AmountToPay { get; set; }
        
        public string? ReferenceNumber { get; set; }
    }

    public class PayLoanCommandHandler : IRequestHandler<PayLoanCommand, bool>
    {
        private readonly IUserRepaymentRepository _userRepaymentRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanTransactionRepository _loanTransactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PayLoanCommandHandler(
            IUserRepaymentRepository userRepaymentRepository,
            ILoanRepository loanRepository,
            ILoanTransactionRepository loanTransactionRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepaymentRepository = userRepaymentRepository;
            _loanRepository = loanRepository;
            _loanTransactionRepository = loanTransactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(PayLoanCommand request, CancellationToken cancellationToken)
        {
            var userRepayment = await _userRepaymentRepository.GetById(request.UserRepaymentId, cancellationToken);

            if (userRepayment == null)
            {
                throw new Exception("UserRepayment not found.");
            }

            var loan = await _loanRepository.GetById(userRepayment.LoanId, cancellationToken);
            
            if (userRepayment.Status == (int)UserRepatmentStatus.Paid)
            {
                throw new Exception("This repayment is already paid.");
            }

            decimal remainingAmountToPay = request.AmountToPay;

            decimal penaltyRemaining = Math.Max(0, userRepayment.PenaltyAmount - userRepayment.PaidAmount);
            decimal amountForPenalty = Math.Min(remainingAmountToPay, penaltyRemaining);
            remainingAmountToPay -= amountForPenalty;

            decimal interestRemaining = Math.Max(0, userRepayment.InterestAmount - Math.Max(0, userRepayment.PaidAmount - userRepayment.PenaltyAmount));
            decimal amountForInterest = Math.Min(remainingAmountToPay, interestRemaining);
            remainingAmountToPay -= amountForInterest;

            decimal principalRemaining = Math.Max(0, userRepayment.PrincipalAmount - Math.Max(0, userRepayment.PaidAmount - userRepayment.PenaltyAmount - userRepayment.InterestAmount));
            decimal amountForPrincipal = Math.Min(remainingAmountToPay, principalRemaining);
            remainingAmountToPay -= amountForPrincipal;

            decimal totalRequired = userRepayment.PenaltyAmount + userRepayment.InterestAmount + userRepayment.PrincipalAmount;
            
            userRepayment.PaidAmount += (request.AmountToPay - remainingAmountToPay);
            loan.Paid += (request.AmountToPay - remainingAmountToPay);

            if (userRepayment.PaidAmount >= totalRequired)
            {
                userRepayment.Status = (int)UserRepatmentStatus.Paid;
            }
            else
            {
                userRepayment.Status = (int)UserRepatmentStatus.Partial;
            }

            var transaction = new LoanTransaction
            {
                LoanId = loan.Id,
                Amount = request.AmountToPay - remainingAmountToPay,
                TransactionType = (int)TransactionType.Repayment,
                ReferenceNumber = request.ReferenceNumber
            };

            await _loanTransactionRepository.Add(transaction, cancellationToken);
            await _userRepaymentRepository.Update(userRepayment, cancellationToken);
            await _loanRepository.Update(loan, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
