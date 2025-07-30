using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.Domain.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
    }
}
