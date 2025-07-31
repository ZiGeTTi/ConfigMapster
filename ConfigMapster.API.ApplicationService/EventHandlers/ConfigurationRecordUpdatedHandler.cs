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
        public ConfigurationRecordUpdatedHandler(ConfigurationCacheService redisService, IMongoRepository<ConfigurationRecordDocument> mongoRepository)
        {
            _redisService = redisService;
            _mongoRepository = mongoRepository;
        }

        public async Task HandleAsync(ConfigurationRecordUpdated domainEvent, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{domainEvent.ApplicationName}_{domainEvent.Key}";
            await _redisService.KeyDeleteAsync(cacheKey);
            var document = await _mongoRepository.FindOneAsync(x => x.ApplicationName == domainEvent.ApplicationName && x.IsActive);
            await _redisService.SetValueAsync(cacheKey, document.ToEntity() );
        }
    }
}
