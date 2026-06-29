using Domain.Entities.Base;

namespace Domain.Entities
{
    public class CollectionTask : BaseEntity
    {
        public int LoanId { get; set; }
        public int AgentId { get; set; } 
        public string? Note { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        
        public Loan Loan { get; set; } = null!;
    }
}
