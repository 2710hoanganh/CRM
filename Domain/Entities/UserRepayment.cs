using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserRepayment : BaseEntity
    {
        public int LoanId { get; set; }
        public DateTime RepaymentDate { get; set; }
        public int Status { get; set; }
        public Loan Loan { get; set; } = null!;
    }
}