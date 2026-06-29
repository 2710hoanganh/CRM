using Application.Repositories.Base;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepaymentRepository : IBaseRepository<UserRepayment> 
    { 
        Task<List<UserRepayment>> GetOverdueRepaymentsWithLoanAsync(DateTime beforeDate, CancellationToken cancellationToken = default);
    }
}