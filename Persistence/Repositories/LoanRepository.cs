using Persistence.Contexts;
using Persistence.Repositories.Base;
using Domain.Entities;
using Application.Repositories;
using Domain.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        public LoanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Paged<IQueryable<TType>>> GetPaginationWithUser<TType>(
            Expression<Func<Loan, bool>>? filter = null,
            Func<IQueryable<Loan>, IOrderedQueryable<Loan>>? orderBy = null,
            Expression<Func<Loan, TType>>? selector = null,
            ulong pageNumber = 0,
            ulong pageSize = 0,
            CancellationToken cancellationToken = default) where TType : class
        {
            return await GetPagination(
                filter: filter,
                orderBy: orderBy,
                includes: x => x.Include(l => l.User),
                selector: selector,
                pageNumber: pageNumber,
                pageSize: pageSize,
                cancellationToken: cancellationToken);
        }
    }
}