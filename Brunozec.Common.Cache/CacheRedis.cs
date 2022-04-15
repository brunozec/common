using System.Diagnostics;
using Brunozec.Common.ErrorLogging;
using Brunozec.Common.Extensions;
using Brunozec.Common.Helpers;
using Brunozec.Common.Validators;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Brunozec.Common.Cache;

public class CacheRedis : ICacheRedis
{
    private readonly IErrorLogging _errorLogging;
    private readonly ConnectionMultiplexer _redis;

    public CacheRedis(IErrorLogging errorLogging)
    {
        _errorLogging = errorLogging;

        try
        {
            _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints =
                {
                    {
                        ConfigHelper.GetSetting<string>("DATABASE:REDIS:INSTANCE"),
                        ConfigHelper.GetSetting<int>("DATABASE:REDIS:PORT")
                    }
                },
                DefaultDatabase = ConfigHelper.GetSetting<int>("DATABASE:REDIS:DEFAULTDATABASE"),
                Password = ConfigHelper.GetSetting<string>("DATABASE:REDIS:PASSWORD"),
                ConnectTimeout = ConfigHelper.GetSetting<int>("DATABASE:REDIS:TIMEOUT"),
                AbortOnConnectFail = false
            });
        }
        catch (Exception ex)
        {
            _errorLogging.Log($"FALHA AO CONECTAR AO REDIS");
            _errorLogging.Log(ex);
        }
    }

    /// <summary>
    /// Método para verificar se existe cache do objeto requisitado, caso exista retorna do cache, caso não exista retorna do banco de dados
    /// </summary>
    /// <typeparam name="TEntity">Tipo da Entidade</typeparam>
    /// <param name="key">Chave no cache redis</param>
    /// <param name="funcDB">Função para carregar objeto do banco de dados</param>
    /// <param name="type">Tipo do cache que indica a validade do registro</param>
    /// <returns></returns>
    public async Task<FunctionResult<T>> ReturnIfCachedAsync<T>(
        string key,
        Func<Task<FunctionResult<T>>> funcDB,
        CacheDuration type = CacheDuration.Day1)
    {
        try
        {
            T? cached = default;
            try
            {
                cached = await GetStringAsync<T>(key);
            }
            catch (Exception e)
            {
                _errorLogging.Log(e);
            }

            if (cached == null)
            {
#if DEBUG
                Debug.Print(">>> REDIS CACHE MISS - Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, $"Key: {key}, type:{type}");
#endif
                return await funcDB.Invoke()
                    .Then(async model =>
                {
                    try
                    {
                        return await SetStringAsync(key, model, type);
                    }
                    catch (Exception e)
                    {
                        _errorLogging.Log(e);
                    }

                    return model;

                });
            }

#if DEBUG
            Debug.Print(">>> REDIS CACHE HIT - Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, $"Key: {key}, type:{type}");
#endif

            return new FunctionResult<T>(cached);
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
            return await funcDB.Invoke();
        }
    }

    /// <summary>
    /// Método para verificar se existe cache do objeto requisitado, caso exista retorna do cache, caso não exista retorna do banco de dados
    /// </summary>
    /// <typeparam name="TEntity">Tipo da Entidade</typeparam>
    /// <param name="key">Chave no cache redis</param>
    /// <param name="funcDB">Função para carregar objeto do banco de dados</param>
    /// <param name="type">Tipo do cache que indica a validade do registro</param>
    /// <returns></returns>
    public async Task<FunctionResult<TEntity>> ReturnIfEntityCachedAsync<TEntity>(
        string key,
        Func<Task<FunctionResult<TEntity>>> funcDB,
        CacheDuration type = CacheDuration.Day1) where TEntity : class
    {
        try
        {
            TEntity? cached = null;
            try
            {
                cached = await GetStringAsync<TEntity>(key);
            }
            catch (Exception e)
            {
                _errorLogging.Log(e);
            }

            if (cached == null)
            {
#if DEBUG
                Debug.Print(">>> REDIS CACHE MISS - Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, $"Key: {key}, type:{type}");
#endif
                return await funcDB.Invoke()
                    .Then(async model =>
                {
                    try
                    {
                        return await SetStringAsync(key, model, type);
                    }
                    catch (Exception e)
                    {
                        _errorLogging.Log(e);
                    }

                    return model;
                });
            }

#if DEBUG
            Debug.Print(">>> REDIS CACHE HIT - Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, $"Key: {key}, type:{type}");
#endif

            return new FunctionResult<TEntity>(cached);
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
            return await funcDB.Invoke();
        }
    }

    /// <summary>
    /// Salva
    /// </summary>
    /// <typeparam name="TEntity">Tipo da Entidade</typeparam>
    /// <param name="key">Chave no cache redis</param>
    /// <param name="type">Tipo do cache que indica a validade do registro</param>
    /// <returns></returns>
    public async Task<FunctionResult<TEntity>> SaveCacheAsync<TEntity>(string key, TEntity model,
        CacheDuration type = CacheDuration.Day1) where TEntity : class
    {
        try
        {
            await SetStringAsync(key, model, type);
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
        }

        return new FunctionResult<TEntity>(model);
    }

    public async Task InvalidateCacheAsync(params string[] keys)
    {
        try
        {
            var database = _redis.GetDatabase();
            foreach (var key in keys)
            {
                await database.KeyDeleteAsync(key);

#if DEBUG
                Debug.Print(">>> REDIS CACHE INVALIDATING - Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, $"Key: {key}");
#endif
            }
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
            throw;
        }
    }

    public async Task RemoveWithWildCardAsync(params string[] keys)
    {
        try
        {
            var database = _redis.GetDatabase();

            if (keys == null || !keys.Any())
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(keys));

            var keysToDelete = new List<string>();

            foreach (var endpoint in _redis.GetEndPoints())
            {
                var server = _redis.GetServer(endpoint: endpoint);

                if (server != null)
                {
                    try
                    {
                        foreach (var keyRoot in keys)
                        {
                            foreach (var key in server.Keys(pattern: $"*{keyRoot}*"))
                            {
                                await database.KeyDeleteAsync(key);
#if DEBUG
                                Debug.Print(">>> REDIS CACHE INVALIDATING - Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, $"Key: {key}");
#endif
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorLogging.Log(ex);
                        throw;
                    }
                }
            }
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetKeysWildCardAsync(params string[] keys)
    {
        try
        {
            var retorno = new List<string>();
            var database = _redis.GetDatabase();

            if (keys == null || !keys.Any())
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(keys));

            var keysToDelete = new List<string>();

            foreach (var endpoint in _redis.GetEndPoints())
            {
                var server = _redis.GetServer(endpoint: endpoint);

                if (server != null)
                {
                    try
                    {
                        foreach (var keyRoot in keys)
                        {
                            foreach (var key in server.Keys(pattern: $"*{keyRoot}*"))
                            {
                                retorno.Add(key);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorLogging.Log(ex);
                        throw;
                    }
                }
            }

            return retorno;
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
            throw;
        }
    }

    public async Task<TEntity?> GetStringAsync<TEntity>(string key)
    {
        try
        {
            var database = _redis.GetDatabase();

            var redisCacheValue = await database.StringGetAsync(key);
            if (!string.IsNullOrEmpty(redisCacheValue))
            {
                return JsonConvert.DeserializeObject<TEntity>(redisCacheValue);
            }
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
        }

        return default;
    }

    public async Task<FunctionResult<TEntity>> SetStringAsync<TEntity>(
        string key,
        TEntity o,
        CacheDuration type = CacheDuration.Day1)
    {
        try
        {
            var database = _redis.GetDatabase();

#if DEBUG
            Debug.Print(">>> REDIS CACHE SAVE - Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, $"Key: {key}, type:{type}");
#endif

            var expiry = TimeSpan.FromDays((int)type);

            await database.StringSetAsync(key, JsonConvert.SerializeObject(o), expiry);
        }
        catch (Exception e)
        {
            _errorLogging.Log(e);
        }

        return new FunctionResult<TEntity>(o);
    }
}