using Domain.Entities.Base;

namespace Domain.Entities
{
    public class LoanTransaction : BaseEntity
    {
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public int TransactionType { get; set; }
        public string? ReferenceNumber { get; set; }
        
        public Loan Loan { get; set; } = null!;
    }
}
