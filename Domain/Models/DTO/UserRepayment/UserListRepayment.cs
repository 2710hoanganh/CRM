namespace Domain.Models.DTO.UserRepayment
{
    public class UserListRepayment
    {
        public decimal Amount { get; set; }
        public List<UserRepaymentDateResponse> RepaymentDates { get; set; } = new List<UserRepaymentDateResponse>();
    }
}