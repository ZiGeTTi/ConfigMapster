using ConfigMapster.API.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.Infrastructure.Queue
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly DomainEventDispatcher _dispatcher;
        public RabbitMqConsumerService(
           IOptions<RabbitMqSettings> options,
            IServiceProvider serviceProvider,
            ILogger<RabbitMqConsumerService> logger = null)
        {
            _settings = options.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_settings.HostName) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _settings.Exchange, type: ExchangeType.Fanout);
            _channel.QueueDeclare(queue: _settings.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _settings.QueueName, exchange: _settings.Exchange, routingKey: "");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                _ = Task.Run(async () =>
                {


                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);


                    _logger?.LogInformation($"[RabbitMQ] Received event: {message}");

                    try
                    {

                        var baseEvent = JsonSerializer.Deserialize<ConfigurationRecordDeleted>(message);
                        if (baseEvent != null)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var dispatcher = scope.ServiceProvider.GetRequiredService<DomainEventDispatcher>();
                                await dispatcher.DispatchAsync(baseEvent, stoppingToken);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Failed to handle event from RabbitMQ.");
                    }
                });
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
