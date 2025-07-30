using ConfigMapster.API.ApplicationService.Services.Interfaces;
using ConfigMapster.API.Domain.Events;
using ConfigMapster.Services;
using ConfigurationApi.Documents;
using ConfigurationApi.Entities;
using ConfigurationApi.Models.Requests;
using ConfigurationApi.Models.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.ApplicationService.Services
{
    public class ConfiguraitonService:IConfiguraitonService
    {
        private readonly IMongoRepository<ConfigurationRecordDocument> _configurationRepository;
        private readonly ConfigurationCacheService _redisCacheService;
        private readonly ILogger<ConfiguraitonService> _logger;

        public ConfiguraitonService(IMongoRepository<ConfigurationRecordDocument> mongoRepository, ConfigurationCacheService redisCacheService, ILogger<ConfiguraitonService> logger)
        {
            _configurationRepository = mongoRepository;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        public async Task<CreateConfigurationRecordResponse> CreateConfiguraitonAsync(CreateConfigurationRecordRequest request, CancellationToken token)
        {
            var config = new ConfigurationRecord
        (
            request.Environment,
            request.ApplicationName,
            request.Key,
            request.Value,
            request.Type
        );

            await _configurationRepository.InsertOneAsync(config.ToDocument());

            ConfigurationRecordCreated configurationRecordCreated = new ConfigurationRecordCreated()
            {
                ConfigurationRecordId = config.IdentityObject.Id
            };

            return new CreateConfigurationRecordResponse
            {
                Id = config.IdentityObject.Id
            };
        }

        public async Task DeleteConfiguraitonAsync(Guid id, CancellationToken token)
        {
            var existingConfigDoc = await _configurationRepository.FindByIdAsync(id);
            var configMapster = existingConfigDoc.ToEntity();
            configMapster.Delete();
             await  _configurationRepository.ReplaceOneAsync(configMapster.ToDocument(), cancellationToken:token);
        }

        public async Task<QueryConfigurationRecordsResponse> ListConfigurations(string environment, string applicationName, CancellationToken token)
        {
            var documents = await _configurationRepository.FilterByAsync(
     x => x.Environment == environment && x.ApplicationName == applicationName, token );

            var items = documents.Select(doc => new ConfigurationRecordItemResponse
            {
                Id = doc.Id,
                Version = doc.Version,
                Environment = doc.Environment,
                ApplicationName = doc.ApplicationName,
                Key = doc.Key,
                Value = doc.Value,
                Type = doc.Type
            }).ToList();

            return new QueryConfigurationRecordsResponse
            {
                Items = items,
                TotalRecords = items.Count
            };
        }

        public async Task<UpdateConfigurationRecordResponse> UpdateConfiguraitonAsync(UpdateConfigurationRecordRequest request, CancellationToken token)
        {
            // Find the existing document by Id
            var existingConfig = await _configurationRepository.FindOneAsync(x => x.Id == request.Id);
            if (existingConfig == null)
                throw new KeyNotFoundException($"Configuration with ID {request.Key} not found.");

            var config = existingConfig.ToEntity(); // Convert to entity if needed
            config.Update(request.Value, request.Type);
        
            // Persist the updated document.
            await _configurationRepository.ReplaceOneAsync(config.ToDocument(), cancellationToken:token);



            return new UpdateConfigurationRecordResponse
            {
                Id = config.IdentityObject.Id,
            };
        }

    }
}
