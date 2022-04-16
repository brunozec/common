namespace Brunozec.Common.Repository;

public interface IBaseContext : IAsyncDisposable
{
    bool IsTransactionStarted { get; }
    
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}