using Domain.Constants.AppEnum;
namespace Domain.Models.DTO.Loan
{
    public class CreateLoanRequest
    {
        public required decimal LoanAmount { get; set; }
        public required int LoanTerm { get; set; }
        // public LoanRate LoanRate { get; set; } = LoanRate.BaseRate;
    }
}