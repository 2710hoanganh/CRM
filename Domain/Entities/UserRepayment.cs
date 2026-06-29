using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserRepayment : BaseEntity
    {
        public int LoanId { get; set; }
        public DateTime RepaymentDate { get; set; }
        public int Status { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public Loan Loan { get; set; } = null!;
    }
}