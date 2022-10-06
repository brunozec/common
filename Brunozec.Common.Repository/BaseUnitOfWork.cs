namespace Brunozec.Common.Repository;

//https://github.com/zdz72113/NETCore_BasicKnowledge.Examples/blob/master/Documents/1.6%20%5BBasic%5D%20ASP.NET%20Core%20%E4%B8%AD%E7%9A%84%20ORM%20%E4%B9%8B%20Dapper.md
public sealed class BaseUnitOfWork : IBaseUnitOfWork
{
    private readonly IBaseContext _context;

    public BaseUnitOfWork(IBaseContext context)
    {
        _context = context;
        _context.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        if (!_context.IsTransactionStarted)
            throw new InvalidOperationException("Transaction have already been commited or disposed of");

        _context.Commit();
    }

    public async ValueTask DisposeAsync()
    {
        if (_context.IsTransactionStarted)
            _context.Rollback();
    }

    public async Task BeginTransaction()
    {
        _context.BeginTransaction();
    }
}