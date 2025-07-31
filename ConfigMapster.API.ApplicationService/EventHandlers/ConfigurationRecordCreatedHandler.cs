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
    public class ConfigurationRecordCreatedHandler : IEventHandler<ConfigurationRecordCreated>
    {
        private readonly ConfigurationCacheService _redisService;
        private readonly IMongoRepository<ConfigurationRecordDocument> _mongoRepository;

        public ConfigurationRecordCreatedHandler(ConfigurationCacheService redisService, IMongoRepository<ConfigurationRecordDocument> mongoRepository)
        {
            _mongoRepository = mongoRepository;
            _redisService = redisService;
        }

        public async Task HandleAsync(ConfigurationRecordCreated domainEvent, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{domainEvent.ApplicationName}_{domainEvent.Key}";
            var document = await _mongoRepository.FindOneAsync(x => x.ApplicationName == domainEvent.ApplicationName && x.Key == domainEvent.Key && x.IsActive);
            await _redisService.SetValueAsync(cacheKey, document.ToEntity());
        }
    }
}
