using ConfigMapster.API.Domain.Messaging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.Infrastructure.Queue
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly string _hostName;
        private readonly string _exchange;

        public RabbitMqPublisher(string hostName, string exchange = "domain_events")
        {
            _hostName = hostName;
            _exchange = exchange;
        }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory();
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.HostName = "localhost";
            factory.VirtualHost = "/";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Fanout);

            // Add EventType property to the message
            var eventType = @event.GetType().Name;
            var eventDict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(@event));
            eventDict["EventType"] = eventType;
            var message = JsonSerializer.Serialize(eventDict);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _exchange, routingKey: "", basicProperties: null, body: body);

            return Task.CompletedTask;
        }
    }
}
