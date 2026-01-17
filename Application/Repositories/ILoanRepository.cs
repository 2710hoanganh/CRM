using Application.Repositories.Base;
using Domain.Entities;
using Domain.Models.Common;
using System.Linq.Expressions;

namespace Application.Repositories
{
    public interface ILoanRepository : IBaseRepository<Loan>
    {
        Task<Paged<IQueryable<TType>>> GetPaginationWithUser<TType>(
            Expression<Func<Loan, bool>>? filter = null,
            Func<IQueryable<Loan>, IOrderedQueryable<Loan>>? orderBy = null,
            Expression<Func<Loan, TType>>? selector = null,
            ulong pageNumber = 0,
            ulong pageSize = 0,
            CancellationToken cancellationToken = default) where TType : class;
    }
}