using ConfigMapster.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMapster.API.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfraServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IRedisService>(provider =>
            {
                var redisConnectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";
                return new RedisService(redisConnectionString);
            });
        }
    }
}
