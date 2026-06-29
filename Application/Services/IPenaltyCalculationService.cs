namespace Application.Services
{
    public interface IPenaltyCalculationService
    {
        decimal CalculatePenalty(decimal principal, decimal interestRate, int overdueDays);
        Task ProcessOverdueRepaymentsAsync(CancellationToken cancellationToken = default);
    }
}
