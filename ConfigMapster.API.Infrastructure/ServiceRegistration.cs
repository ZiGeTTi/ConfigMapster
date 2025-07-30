using ConfigMapster.API.Domain.Events;
using ConfigMapster.API.Domain.Messaging;
using ConfigMapster.API.Infrastructure.Queue;
using ConfigMapster.API.Persistence.Redis;
using ConfigMapster.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            var rabbitHost = configuration["RabbitMq:HostName"] ?? "localhost";
            var rabbitExchange = configuration["RabbitMq:Exchange"] ?? "domain_events";
            serviceCollection.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            serviceCollection.AddSingleton<IMessagePublisher>(new RabbitMqPublisher(rabbitHost, rabbitExchange));
     
            serviceCollection.AddScoped<DomainEventDispatcher>();

            serviceCollection.AddSingleton<IHostedService, RabbitMqConsumerService>();

        }
    }
}
