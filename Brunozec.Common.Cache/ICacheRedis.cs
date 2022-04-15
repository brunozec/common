using Brunozec.Common.Validators;

namespace Brunozec.Common.Cache;

public interface ICacheRedis
{
    Task<FunctionResult<T>> ReturnIfCachedAsync<T>(
        string key,
        Func<Task<FunctionResult<T>>> funcDB,
        CacheDuration type = CacheDuration.Day1);

    Task<FunctionResult<TEntity>> ReturnIfEntityCachedAsync<TEntity>(
        string key,
        Func<Task<FunctionResult<TEntity>>> funcDB,
        CacheDuration type = CacheDuration.Day1) where TEntity : class;

    Task InvalidateCacheAsync(params string[] keys);

    Task RemoveWithWildCardAsync(params string[] keys);

    Task<IEnumerable<string>> GetKeysWildCardAsync(params string[] keys);

    Task<TEntity?> GetStringAsync<TEntity>(string key);

    Task<FunctionResult<TEntity>> SetStringAsync<TEntity>(
        string key,
        TEntity o,
        CacheDuration type = CacheDuration.Day1);
}