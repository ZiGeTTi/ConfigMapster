using ConfigMapster.API.ApplicationService.EventHandlers;
using ConfigMapster.API.ApplicationService.Services;
using ConfigMapster.API.ApplicationService.Services.Interfaces;
using ConfigMapster.API.Domain.Events;
using ConfigMapster.API.Persistence.Redis;
using ConfigMapster.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMapster.API.ApplicationService
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {

            serviceCollection.AddScoped<DomainEventDispatcher>();
            serviceCollection.AddScoped<IEventHandler<ConfigurationRecordDeleted>, ConfigurationRecordDeletedHandler>();
            serviceCollection.AddScoped<IEventHandler<ConfigurationRecordUpdated>, ConfigurationRecordUpdatedHandler>();
            serviceCollection.AddScoped<IConfiguraitonService, ConfiguraitonService>();
            var redisConfig = configuration.GetSection("RedisConfig").Get<RedisConfig>();
            serviceCollection.Configure<RedisConfig>(configuration.GetSection("RedisConfig"));
            serviceCollection.AddSingleton<ConfigurationCacheService>(provider =>
            {

                var redisClientFactory = provider.GetRequiredService<RedisClientFactory>();

                return new ConfigurationCacheService(redisClientFactory, redisConfig);
            });
        }
    }
}
