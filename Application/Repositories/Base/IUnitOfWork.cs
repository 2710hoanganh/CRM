namespace Application.Repositories.Base
{
    public interface IUnitOfWork
    {
        Task<Guid> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted,
           Guid transactionId = default,
           CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(Guid transactionId = default, CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(Guid transactionId = default, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Execute code within a transaction using execution strategy (supports retry on failure)
        /// </summary>
        Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);
    }
}