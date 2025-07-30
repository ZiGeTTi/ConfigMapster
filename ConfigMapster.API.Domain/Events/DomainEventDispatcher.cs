using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.Domain.Events
{
    public class DomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(BaseEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });
                    await task.ConfigureAwait(false);
                }
            }
        }

        public async Task DispatchAllAsync(IEnumerable<BaseEvent> events, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in events)
            {
                await DispatchAsync(domainEvent, cancellationToken);
            }
        }
    }
}
