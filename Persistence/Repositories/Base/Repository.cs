using Application.Repositories.Base;
using Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Domain.Models.Common;

namespace Persistence.Repositories.Base
{
    public abstract class Repository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual IQueryable<T> Get(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            ulong pageNumber = 0,
            ulong pageSize = 0)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageSize > 0)
            {
                query = query.Skip((int)(pageNumber * pageSize)).Take((int)pageSize);
            }

            return query;
        }

        public virtual IQueryable<TType> Get<TType>(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            Expression<Func<T, TType>>? selector = null,
            ulong pageNumber = 0,
            ulong pageSize = 0) where TType : class
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (selector != null)
            {
                return query.Select(selector);
            }

            if (pageSize > 0)
            {
                query = query.Skip((int)(pageNumber * pageSize)).Take((int)pageSize);
            }

            return query.Select(selector ?? throw new ArgumentNullException(nameof(selector)));
        }

        public virtual async Task<Paged<IQueryable<T>>> GetPagination(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            ulong pageNumber = 0,
            ulong pageSize = 0,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = Get(filter, orderBy, includes, 0, 0);
            var totalRecords = await query.CountAsync(cancellationToken);

            var pagedQuery = Get(filter, orderBy, includes, pageNumber, pageSize);
            var paged = new Paged<IQueryable<T>>(pagedQuery, pageNumber, pageSize, (ulong)totalRecords, null);

            return paged;
        }

        public virtual async Task<Paged<IQueryable<TType>>> GetPagination<TType>(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            Expression<Func<T, TType>>? selector = null,
            ulong pageNumber = 0,
            ulong pageSize = 0,
            CancellationToken cancellationToken = default) where TType : class
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageSize > 0)
            {
                query = query.Skip((int)(pageNumber * pageSize)).Take((int)pageSize);
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var pagedQuery = query.Select(selector);
            var paged = new Paged<IQueryable<TType>>(pagedQuery, pageNumber, pageSize, (ulong)totalRecords, null);

            return paged;
        }

        public virtual async Task<T> GetOne(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.FirstOrDefaultAsync(cancellationToken) ?? default!;
        }

        public virtual async Task<TType> GetOne<TType>(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken cancellationToken = default) where TType : class
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // This would need a selector, but interface doesn't provide one
            // This method signature seems incomplete in the interface
            throw new NotImplementedException("GetOne<TType> requires a selector which is not provided in the interface");
        }

        public virtual async Task<T> GetById(object id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new[] { id }, cancellationToken) ?? throw new InvalidOperationException($"Entity with id {id} not found");
        }

        public virtual async Task<bool> Find(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter, cancellationToken) != null ? true : false;
        }

        public virtual async Task<T> Add(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task<List<T>> AddRange(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            return entities.ToList();
        }

        public virtual Task<T> Update(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }

        public virtual async Task<T> ExcuteUpdate(
            Expression<Func<T, bool>> filter,
            Action<T> updateAction,
            CancellationToken cancellationToken = default)
        {
            var entities = await _dbSet.Where(filter).ToListAsync(cancellationToken);
            foreach (var entity in entities)
            {
                updateAction(entity);
            }
            _dbSet.UpdateRange(entities);
            return entities.FirstOrDefault() ?? default!;
        }

        public virtual Task<T> Delete(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.FromResult(entity);
        }

        public virtual async Task<T> ExcuteDelete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            var entities = await _dbSet.Where(filter).ToListAsync(cancellationToken);
            if (!entities.Any())
            {
                return default!;
            }
            _dbSet.RemoveRange(entities);
            return entities.First() ?? default!;
        }
    }
}