using Domain.Entities;
using Persistence.Contexts;
using Persistence.Repositories.Base;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Constants.AppEnum;

namespace Persistence.Repositories
{
    public class UserRepaymentRepository : Repository<UserRepayment>, IUserRepaymentRepository
    {
        public UserRepaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<UserRepayment>> GetOverdueRepaymentsWithLoanAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
        {
            return await _context.UserRepayments
                .Include(ur => ur.Loan)
                .Where(ur => ur.RepaymentDate < beforeDate && ur.Status != (int)UserRepatmentStatus.Paid)
                .ToListAsync(cancellationToken);
        }
    }
}