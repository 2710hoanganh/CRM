using Application.Repositories.Base; // Sửa namespace
using Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;


namespace Persistence.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Guid, IDbContextTransaction> _transactions;
        private bool disposed = false;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _transactions = new Dictionary<Guid, IDbContextTransaction>();
        }

        public async Task<Guid> BeginTransactionAsync(
            System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted,
            Guid transactionId = default,
            CancellationToken cancellationToken = default)
        {
            // Tạo transactionId nếu chưa có
            if (transactionId == default)
            {
                transactionId = Guid.NewGuid();
            }

            // Kiểm tra nếu transactionId đã tồn tại
            if (_transactions.ContainsKey(transactionId))
            {
                throw new InvalidOperationException($"Transaction with ID {transactionId} already exists.");
            }

            // Begin transaction
            var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
            
            // Lưu vào dictionary
            _transactions[transactionId] = transaction;

            return transactionId;
        }

        public async Task CommitTransactionAsync(Guid transactionId = default, CancellationToken cancellationToken = default)
        {
            if (transactionId != default && _transactions.ContainsKey(transactionId))
            {
                var transaction = _transactions[transactionId];
                try
                {
                    await transaction.CommitAsync(cancellationToken);
                }
                finally
                {
                    await transaction.DisposeAsync();
                    _transactions.Remove(transactionId);
                }
            }
            else if (transactionId == default)
            {
                // Nếu không có transactionId, commit transaction hiện tại (nếu có)
                var currentTransaction = _context.Database.CurrentTransaction;
                if (currentTransaction != null)
                {
                    await currentTransaction.CommitAsync(cancellationToken);
                }
            }
            else
            {
                throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");
            }
        }

        public async Task RollbackTransactionAsync(Guid transactionId = default, CancellationToken cancellationToken = default)
        {
            if (transactionId != default && _transactions.ContainsKey(transactionId))
            {
                var transaction = _transactions[transactionId];
                try
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                finally
                {
                    await transaction.DisposeAsync();
                    _transactions.Remove(transactionId);
                }
            }
            else if (transactionId == default)
            {
                // Nếu không có transactionId, rollback transaction hiện tại (nếu có)
                var currentTransaction = _context.Database.CurrentTransaction;
                if (currentTransaction != null)
                {
                    await currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            else
            {
                throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> operation, 
            CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync<AppDbContext, T>(
                _context,
                async (dbContext, state, ct) =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
                    try
                    {
                        var result = await operation(ct);
                        await transaction.CommitAsync(ct);
                        return result;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                },
                null,
                cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose tất cả transactions (synchronous)
                    if (_transactions != null)
                    {
                        foreach (var item in _transactions.Values)
                        {
                            item.Dispose(); // Dispose synchronous
                        }
                        _transactions.Clear();
                    }

                    _context.Dispose(); // Dispose synchronous
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}