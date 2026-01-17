namespace Domain.Models.DTO.Loan
{
    public class ListLoanResponse
    {
        public required int Id { get; set; }
        public required int UserId { get; set; }
        public required string UserName { get; set; }
        public required decimal Amount { get; set; }
        public required int Term { get; set; }
        public required decimal Total { get; set; }
        public required decimal PaybackAmount { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required int Status { get; set; }
    }
}