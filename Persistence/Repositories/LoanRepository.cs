using Persistence.Contexts;
using Persistence.Repositories.Base;
using Domain.Entities;
using Application.Repositories;

namespace Persistence.Repositories
{
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        public LoanRepository(AppDbContext context) : base(context)
        {
        }
    }
}