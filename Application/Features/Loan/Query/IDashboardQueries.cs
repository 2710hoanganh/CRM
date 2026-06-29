namespace Application.Features.Loan.Query
{
    public interface IDashboardQueries
    {
        Task<DashboardSummaryDto> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken);
    }
}
