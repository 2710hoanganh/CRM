using Application.Features.Loan.Query;
using Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Domain.Constants.AppEnum;

namespace Persistence.Queries
{
    public class DashboardQueries : IDashboardQueries
    {
        private readonly AppDbContext _context;

        public DashboardQueries(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            
            var totalDisbursed = await _context.LoanTransactions
                .Where(t => t.TransactionType == (int)TransactionType.Disbursement && t.CreatedAt >= startOfMonth)
                .SumAsync(t => t.Amount, cancellationToken);

            var totalCollected = await _context.LoanTransactions
                .Where(t => t.TransactionType == (int)TransactionType.Repayment && t.CreatedAt >= startOfMonth)
                .SumAsync(t => t.Amount, cancellationToken);

            var totalDebt = await _context.Loans
                .Where(l => l.Status == (int)LoanStatus.Active || l.Status == (int)LoanStatus.BadDebt || l.Status == (int)LoanStatus.Overdue)
                .SumAsync(l => Math.Max(0, l.Total - l.Paid), cancellationToken);

            var badDebt = await _context.Loans
                .Where(l => l.Status == (int)LoanStatus.BadDebt || l.Status == (int)LoanStatus.Overdue)
                .SumAsync(l => Math.Max(0, l.Total - l.Paid), cancellationToken);

            decimal nplRatio = totalDebt > 0 ? (badDebt / totalDebt) * 100m : 0m;

            var sevenDaysAgo = now.Date.AddDays(-7);

            var recentDisbursements = await _context.LoanTransactions
                .Where(t => t.TransactionType == (int)TransactionType.Disbursement && t.CreatedAt >= sevenDaysAgo)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(t => t.Amount) })
                .ToListAsync(cancellationToken);

            var recentCollections = await _context.LoanTransactions
                .Where(t => t.TransactionType == (int)TransactionType.Repayment && t.CreatedAt >= sevenDaysAgo)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(t => t.Amount) })
                .ToListAsync(cancellationToken);

            var dto = new DashboardSummaryDto
            {
                TotalDisbursedAmount = totalDisbursed,
                TotalCollectionAmount = totalCollected,
                NPLRatio = Math.Round(nplRatio, 2)
            };

            for (int i = 0; i < 7; i++)
            {
                var d = now.Date.AddDays(-i);
                dto.DisbursedLast7Days[d] = recentDisbursements.FirstOrDefault(x => x.Date == d)?.Total ?? 0m;
                dto.CollectedLast7Days[d] = recentCollections.FirstOrDefault(x => x.Date == d)?.Total ?? 0m;
            }

            return dto;
        }
    }
}
