using ConfigMapster.API.Persistence.Redis;
using ConfigurationApi.Entities;
using RedLockNet;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigMapster.Services
{
    public class CacheService<T> : RedisCacheServiceBase<T>
    {
        public CacheService(RedisClientFactory redisClientFactory, int database, TimeSpan expireTimeSpan) : base(redisClientFactory, database, expireTimeSpan)
        {
        }

        internal async Task<bool> KeyExistsAsync(string key)
        {
            throw new NotImplementedException();
        }
    }

    public class ConfigurationCacheService
    {
        private readonly CacheService<ConfigurationRecord> _cacheService;
        private RedisConfig _redisConfig;
        private readonly IDistributedLockFactory _redLockFactory;
        private readonly RedisClientFactory _redisClientFactory;
        public ConfigurationCacheService(RedisClientFactory redisClientFactory, RedisConfig redisConfig)
        { 
            _redisClientFactory = redisClientFactory;
            _redLockFactory = redisClientFactory.distributedLockFactory;
            _redisConfig = redisConfig;
            _cacheService = new CacheService<ConfigurationRecord>(redisClientFactory, redisConfig.Database, TimeSpan.FromMinutes(_redisConfig.ExpireTimeSpan));
        }
        public async Task<List<ConfigurationRecord>> GetOrAddDataAsync(string key, Func<Task<List<ConfigurationRecord>>> action)
        {
            return await _cacheService.GetOrAddAsync(key, action);
        }
        public async Task KeyDeleteAsync(string key)
        {
            await _cacheService.KeyDeleteAsync(key);
        }
        public async Task<bool> SetValueAsync(string key, ConfigurationRecord value)
        {
            return await _cacheService.SetValueAsync(key, value);
        }
        public async Task<List<ConfigurationRecord>> GetValueAsync(string key)
        {
            return await _cacheService.GetValueAsync(key);
        }

        public async Task<TResult> ExecuteWithDistributedLockAsync<TResult>(string key, TimeSpan timeout, Func<Task<TResult>> action)
        {
            var redLockFactory = _redisClientFactory.distributedLockFactory;
            using (var redLock = await redLockFactory.CreateLockAsync(key, timeout))
            {
                if (redLock.IsAcquired)
                {
                    return await action();
                }
                throw new Exception("Could not acquire distributed lock.");
            }
        }

    }
}
