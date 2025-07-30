using ConfigMapster.API.Domain.Events;
using ConfigMapster.Services;
using ConfigurationApi.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.ApplicationService.EventHandlers
{
    public class ConfigurationRecordUpdatedHandler : IEventHandler<ConfigurationRecordUpdated>
    {
        private readonly ConfigurationCacheService _redisService;
        private readonly IMongoRepository<ConfigurationRecordDocument> _mongoRepository;
        public ConfigurationRecordUpdatedHandler(ConfigurationCacheService redisService)
        {
            _redisService = redisService;
        }

        public async Task HandleAsync(ConfigurationRecordUpdated domainEvent, CancellationToken cancellationToken = default)
        {
            await _redisService.KeyDeleteAsync(domainEvent.Key);
            var document = await _mongoRepository.FindOneAsync(x => x.Key == domainEvent.Key);
            await _redisService.SetValueAsync(domainEvent.Key,document.ToEntity() );
        }
    }
}
