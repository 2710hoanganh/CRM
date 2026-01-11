namespace Application.Repositories.Base
{
    public interface ILoanInterestRate
    {
        Task<decimal> CalculateInterestRate(int months, int loanRate, CancellationToken cancellationToken = default);
        Task<decimal> CalculateTotal(decimal amount, int months, decimal interestRate, CancellationToken cancellationToken = default);
    }
}