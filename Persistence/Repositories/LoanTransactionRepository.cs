using Application.Repositories;
using Domain.Entities;
using Persistence.Contexts;
using Persistence.Repositories.Base;

namespace Persistence.Repositories
{
    public class LoanTransactionRepository : Repository<LoanTransaction>, ILoanTransactionRepository
    {
        public LoanTransactionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
