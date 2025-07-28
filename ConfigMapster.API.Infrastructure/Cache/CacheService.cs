using Campaign.Service.Persistence.Redis;
using ConfigMapster.API.Persistence.Redis;
using ConfigurationApi.Entities;
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

    }

    public class ConfigurationCacheService
    {
        private readonly CacheService<ConfigurationRecord> _cacheService;
        private RedisConfig _redisConfig;
        public ConfigurationCacheService(RedisClientFactory redisClientFactory, RedisConfig redisConfig)
        {
            _redisConfig = redisConfig;
            _cacheService = new CacheService<ConfigurationRecord>(redisClientFactory, redisConfig.Database, TimeSpan.FromMinutes(_redisConfig.ExpireTimeSpan);
        }
        public async Task<List<ConfigurationRecord>> GetOrAddDataAsync(string key, Func<Task<List<ConfigurationRecord>>> action)
        {
            return await _cacheService.GetOrAddAsync(key,action);
        }
    }

 
}
