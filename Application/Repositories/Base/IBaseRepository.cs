using System.Linq.Expressions;
using Domain.Models.Common;

namespace Application.Repositories.Base
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> Get(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            ulong pageNumber = 0, ulong pageSize = 0);

        IQueryable<TType> Get<TType>(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            Expression<Func<T, TType>>? selector = null,
            ulong pageNumber = 0, ulong pageSize = 0) where TType : class;

        Task<Paged<IQueryable<T>>> GetPagination(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            ulong pageNumber = 0, ulong pageSize = 0,
            CancellationToken cancellationToken = default);

        Task<Paged<IQueryable<TType>>> GetPagination<TType>(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            Expression<Func<T, TType>>? selector = null,
            ulong pageNumber = 0, ulong pageSize = 0,
            CancellationToken cancellationToken = default) where TType : class;

        Task<T> GetOne(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken cancellationToken = default);

        Task<TType> GetOne<TType>(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken cancellationToken = default) where TType : class;

        Task<T> GetById(object id, CancellationToken cancellationToken = default);

        Task<bool> Find(Expression<Func<T, bool>> filter,
                    Func<IQueryable<T>, IQueryable<T>>? include = null,
                    bool asNoTracking = false,
                    CancellationToken cancellationToken = default);

        Task<T> Add(T entity, CancellationToken cancellationToken = default);
        Task<List<T>> AddRange(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<T> Update(T entity, CancellationToken cancellationToken = default);
        Task<T> ExcuteUpdate(Expression<Func<T, bool>> filter,
                            Action<T> updateAction,
                            CancellationToken cancellationToken = default);
        Task<T> Delete(T entity, CancellationToken cancellationToken = default);
        Task<T> ExcuteDelete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
    }
}