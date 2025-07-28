using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Campaign.Service.Persistence.Redis;
using StackExchange.Redis;

 

public abstract class RedisCacheServiceBase<T>
{
    private readonly IDatabase _redisDb;
    private readonly TimeSpan _expireTimeSpan;
   

    protected RedisCacheServiceBase(RedisClientFactory redisClientFactory, int database, TimeSpan expireTimeSpan)
    {
        _redisDb = redisClientFactory.GetDb(database);
        _expireTimeSpan = expireTimeSpan;
    }

    public bool TryGetValue(string key, out List<T> result)
    {
        if (!_redisDb.KeyExists(key))
        {
            result = default;
            return false;
        }

        var data = _redisDb.StringGet(key);
        result = JsonSerializer.Deserialize<List<T>>(data);
        return true;
    }

    public async Task<string> GetValueAsync(string key)
    {
        return await _redisDb.StringGetAsync(key);
    }

    public async Task<bool> SetValueAsync(string key, T value)
    {
        var byteData = JsonSerializer.SerializeToUtf8Bytes(value);
        return await _redisDb.StringSetAsync(key, byteData, _expireTimeSpan);
    }

    public async Task<List<T>> GetOrAddAsync(string key, Func<Task<List<T>>> action)
    {
        if (TryGetValue(key, out var result))
            return result;

        var data = await action();
        if (data == null)
            return default;
        
        var byteData = JsonSerializer.SerializeToUtf8Bytes(data);
        await _redisDb.StringSetAsync(key, byteData, _expireTimeSpan);
        return data;
    }

    // public T GetOrAdd(string key, Func<T> action)
    // {
    //     if (TryGetValue(key, out var result))
    //         return result;
    //
    //     var data = action();
    //     var byteData = JsonSerializer.SerializeToUtf8Bytes(data);
    //     _redisDb.StringSet(key, byteData, _expireTimeSpan);
    //     return data;
    // }

    public async Task Clear(string key)
    {
        await _redisDb.KeyDeleteAsync(key);
    }

    public void ClearAll()
    {
        // Implement clearing all keys if necessary
    }
}