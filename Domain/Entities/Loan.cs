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
        // Total money pay
        public decimal Total { get; set; }
        public int Term { get; set; }
        public int Rate { get; set; }
        public int Status { get; set; }

        public User User { get; set; } = null!;
    }
}