
using ConfigMapster.API.Persistence.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
 
        serviceCollection.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
    
        serviceCollection.AddSingleton<IMongoDbSettings>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        serviceCollection.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        serviceCollection.Configure<RedisConfig>(configuration.GetSection("RedisConfig"));
        //serviceCollection.AddSingleton<IConnectionMultiplexer>();
        serviceCollection.AddSingleton<RedisClientFactory>();
    
        
    }
}
