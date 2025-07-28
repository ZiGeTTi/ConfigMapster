using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Campaign.Service.Persistence.Redis;

public class RedisClientFactory
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    

    public RedisClientFactory(IOptions<RedisConfig> redisConfig)
    {
        string redisServer = $"{redisConfig.Value.Url} ,password={redisConfig.Value.Password}";
        _connectionMultiplexer = ConnectionMultiplexer.Connect(redisServer);
    }

    public IDatabase GetDb(int dbIndex) => (_connectionMultiplexer.GetDatabase(dbIndex));
    
}