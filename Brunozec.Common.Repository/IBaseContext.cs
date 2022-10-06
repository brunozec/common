namespace Brunozec.Common.Repository;

public interface IBaseContext : IDisposable
{
    bool IsTransactionStarted { get; }
    
    void BeginTransaction();
    void Commit();
    void Rollback();
}