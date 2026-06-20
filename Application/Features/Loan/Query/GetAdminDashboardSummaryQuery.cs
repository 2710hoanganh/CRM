using MediatR;

namespace Application.Features.Loan.Query
{
    public class DashboardSummaryDto
    {
        public decimal TotalDisbursedAmount { get; set; }
        public decimal TotalCollectionAmount { get; set; }
        public decimal NPLRatio { get; set; }
        public Dictionary<DateTime, decimal> DisbursedLast7Days { get; set; } = new();
        public Dictionary<DateTime, decimal> CollectedLast7Days { get; set; } = new();
    }

    public class GetAdminDashboardSummaryQuery : IRequest<DashboardSummaryDto>
    {
    }

    public class GetAdminDashboardSummaryQueryHandler : IRequestHandler<GetAdminDashboardSummaryQuery, DashboardSummaryDto>
    {
        private readonly IDashboardQueries _dashboardQueries;

        public GetAdminDashboardSummaryQueryHandler(IDashboardQueries dashboardQueries)
        {
            _dashboardQueries = dashboardQueries;
        }

        public async Task<DashboardSummaryDto> Handle(GetAdminDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            return await _dashboardQueries.GetAdminDashboardSummaryAsync(cancellationToken);
        }
    }
}
