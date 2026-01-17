using Domain.Entities.Base;

namespace Domain.Entities
{
    public class Loan : BaseEntity
    {
        public int UserId { get; set; }
        // money loan
        public decimal Amount { get; set; }
        // Money pay per month
        public decimal PaybackAmount { get; set; }
        // Total money pay after calculate interest rate
        public decimal Total { get; set; }
        public int Term { get; set; }
        // Loan rate base on account level
        public int Rate { get; set; }
        // Interest rate base on loan term and loan rate
        public decimal InterestRate { get; set; }
        public DateTime EndDate { get; set; }

        public int Status { get; set; }

        public User User { get; set; } = null!;
    }
}