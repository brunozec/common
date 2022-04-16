using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using Brunozec.Dapper.Dommel;
using Dapper;
using DotNext.Threading;
using Newtonsoft.Json;

namespace Brunozec.Common.Repository;

public abstract class BaseContext : IBaseContext
{
    private readonly object _lock = new object();

    private bool _isTransactionStarted;

    private readonly int? _commandTimeout = 180;

    private AsyncLazy<IDbConnection> _connectionLazy;

    public bool IsTransactionStarted => _isTransactionStarted;

    private Task<IDbConnection> Connection => _connectionLazy.Task;

    private IDbTransaction Transaction { get; set; }

    protected abstract Task<IDbConnection> CreateConnection();

    protected BaseContext()
    {
        _isTransactionStarted = false;
        _connectionLazy = new AsyncLazy<IDbConnection>(async () =>
        {
            var conn = await CreateConnection();

            conn.Open();

            return conn;
        });
    }

    protected abstract Task CreateConfiguration();

    public async Task BeginTransactionAsync()
    {
        if (_isTransactionStarted)
            throw new InvalidOperationException("Transaction already started");

        Transaction = (await Connection).BeginTransaction();

        _isTransactionStarted = true;
    }

    public Task CommitAsync()
    {
        if (!_isTransactionStarted)
            throw new InvalidOperationException("Transaction not started");

        Transaction.Commit();
        Transaction.Dispose();
        Transaction = null;

        _isTransactionStarted = false;
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        if (!_isTransactionStarted)
            throw new InvalidOperationException("Transaction not started");

        Transaction.Rollback();
        Transaction.Dispose();
        Transaction = null;

        _isTransactionStarted = false;
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isTransactionStarted)
            await RollbackAsync();

        if (_connectionLazy.IsValueCreated)
        {
            (await Connection).Close();
            (await Connection).Dispose();
        }

        _connectionLazy = null;
    }

    public async Task<int> ExecuteAsync(string sql, object param = null, CommandType commandType = CommandType.Text)
    {
#if DEBUG
        Debug.Print($">>> {DateTime.UtcNow} BaseContext - Thread {Thread.CurrentThread.ManagedThreadId}: {sql}\n\r{JsonConvert.SerializeObject(param)}");
#endif
        return await (await Connection).ExecuteAsync(sql, param, Transaction, _commandTimeout, commandType);
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
    {
#if DEBUG
        Debug.Print($">>> {DateTime.UtcNow} BaseContext - Thread {Thread.CurrentThread.ManagedThreadId}: {sql}\n\r{JsonConvert.SerializeObject(param)}");
#endif
        return await (await Connection).ExecuteScalarAsync<T>(sql, param, Transaction, _commandTimeout, commandType);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
    {
#if DEBUG
        Debug.Print($">>> {DateTime.UtcNow} BaseContext - Thread {Thread.CurrentThread.ManagedThreadId}: {sql}\n\r{JsonConvert.SerializeObject(param)}");
#endif
        return await (await Connection).QueryAsync<T>(sql, param, Transaction, _commandTimeout, commandType);
    }

    public async Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType commandType = CommandType.Text)
    {
#if DEBUG
        Debug.Print($">>> {DateTime.UtcNow} BaseContext - Thread {Thread.CurrentThread.ManagedThreadId}: {sql}\n\r{JsonConvert.SerializeObject(param)}");
#endif
        return await (await Connection).ExecuteReaderAsync(sql, param, Transaction, _commandTimeout, commandType);
    }

    public async Task<long> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return await (await Connection).CountAsync<T>(predicate, Transaction);
    }

    public async Task<bool> DeleteAsync<T>(T entity) where T : class
    {
        if (!_isTransactionStarted)
            throw new InvalidOperationException("Transaction not started");

        return await (await Connection).DeleteAsync<T>(entity, Transaction);
    }

    public async Task<T> GetAsync<T>(object id, CancellationToken token = default) where T : class
    {
        return await (await Connection).GetAsync<T>(id, Transaction, token);
    }

    public async Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default) where T : class
    {
        return await (await Connection).SelectAsync<T>(predicate, Transaction, cancellationToken: token);
    }

    public async Task<IEnumerable<T>> GetPageAsync<T>(Expression<Func<T, bool>> predicate, int page, int resultsPerPage, CancellationToken token = default) where T : class
    {
        return await (await Connection).SelectPagedAsync<T>(predicate, page - 1, resultsPerPage, Transaction, token);
    }

    public async Task<KeyValuePair<long, IList<T>>> GetPageWithTotalAsync<T>(Expression<Func<T, bool>> predicate, int page, int resultsPerPage, CancellationToken token = default) where T : class
    {
        var total = await CountAsync<T>(predicate);

        var results = await GetPageAsync<T>(predicate, page, resultsPerPage, token);

        return new KeyValuePair<long, IList<T>>(total, results.ToList());
    }

    public async Task<object> InsertAsync<T>(T entity, CancellationToken token = default) where T : class
    {
        if (!_isTransactionStarted)
            throw new InvalidOperationException("Transaction not started");

        return await (await Connection).InsertAsync<T>(entity, Transaction, token);
    }

    public async Task<bool> UpdateAsync<T>(T entity, CancellationToken token = default) where T : class
    {
        if (!_isTransactionStarted)
            throw new InvalidOperationException("Transaction not started");

        return await (await Connection).UpdateAsync<T>(entity, Transaction, token);
    }
}