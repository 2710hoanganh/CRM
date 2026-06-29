namespace Domain.Models.DTO.UserRepayment
{
    public class UserRepaymentDateResponse
    {
        public required DateTime RepaymentDate { get; set; }
        public required int Status { get; set; }
    }
}