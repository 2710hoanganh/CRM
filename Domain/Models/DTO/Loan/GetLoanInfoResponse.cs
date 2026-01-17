using Domain.Models.DTO.User;
namespace Domain.Models.DTO.Loan
{
    public class GetLoanInfoResponse
    {
        public required int Id { get; set; }
        public required decimal Amount { get; set; }
        public required int Term { get; set; }
        public required decimal Total { get; set; }
        public required decimal PaybackAmount { get; set; }
        public required string FeedBack { get; set; }
        public required DateTime EndDate { get; set; }
        public required int Status { get; set; }
    }
}