using Microsoft.Extensions.Options;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Collections.Generic;

namespace ConfigMapster.API.Persistence.Redis
{
    public class RedisClientFactory
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public readonly IDistributedLockFactory distributedLockFactory;

        public RedisClientFactory(IOptions<RedisConfig> redisConfig)
        {
            string redisServer = $"{redisConfig.Value.Url}";
            var connectionMultiplexers = new List<RedLockMultiplexer>();
            _connectionMultiplexer = ConnectionMultiplexer.Connect(redisServer);
            connectionMultiplexers.Add(new RedLockMultiplexer(_connectionMultiplexer));
            distributedLockFactory = RedLockFactory.Create(connectionMultiplexers);
        }

        public IDatabase GetDb(int dbIndex) => _connectionMultiplexer.GetDatabase(dbIndex);
    
    }
}