using ConfigMapster.API.Domain.Events;
using ConfigMapster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.ApplicationService.EventHandlers
{
    public class ConfigurationRecordDeletedHandler : IEventHandler<ConfigurationRecordDeleted>
    {
        private readonly ConfigurationCacheService _redisService;

        public ConfigurationRecordDeletedHandler(ConfigurationCacheService redisService)
        {
            _redisService = redisService;
        }

        public async Task HandleAsync(ConfigurationRecordDeleted domainEvent, CancellationToken cancellationToken = default)
        {
            await _redisService.KeyDeleteAsync(domainEvent.Key);
        }
    }
}
