using ConfigMapster.API.ApplicationService.Services.Interfaces;
using ConfigMapster.API.Domain.Events;
using ConfigMapster.API.Domain.Messaging;
using ConfigMapster.API.Infrastructure.Validators;
using ConfigMapster.Services;
using ConfigurationApi.Documents;
using ConfigurationApi.Entities;
using ConfigurationApi.Models.Requests;
using ConfigurationApi.Models.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.ApplicationService.Services
{
    public class ConfiguraitonService : IConfiguraitonService
    {
        private readonly IMongoRepository<ConfigurationRecordDocument> _configurationRepository;
        private readonly ConfigurationCacheService _redisCacheService;
        private readonly ILogger<ConfiguraitonService> _logger;
        private readonly IMessagePublisher _messagePublisher;


        public ConfiguraitonService(IMongoRepository<ConfigurationRecordDocument> mongoRepository, ConfigurationCacheService redisCacheService, ILogger<ConfiguraitonService> logger,
            IMessagePublisher messagePublisher)
        {
            _configurationRepository = mongoRepository;
            _redisCacheService = redisCacheService;
            _logger = logger;
            _messagePublisher = messagePublisher;
        }

        public async Task<CreateConfigurationRecordResponse> CreateConfiguraitonAsync(CreateConfigurationRecordRequest request, CancellationToken token)
        {
            var config = new ConfigurationRecord
        (
            request.Environment,
            request.ApplicationName,
            request.Key,
            request.Value,
            request.Type,
            isActive: true
        );
            config.Create();
            IsValidValue(request.Value, request.Type);

            await _configurationRepository.InsertOneAsync(config.ToDocument());
            await _messagePublisher.PublishAsync(new ConfigurationRecordCreated
            {
                IsActive = config.IsActive,
                Environment = config.Environment,
                ApplicationName = config.ApplicationName,
                Key = config.Key,
                Value = config.Value,
                Type = config.Type,
                Version = config.IdentityObject.Version
            });
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
            await _configurationRepository.ReplaceOneAsync(configMapster.ToDocument(), cancellationToken: token);
        }

        public async Task<QueryConfigurationRecordsResponse> ListConfigurations(string applicationName, CancellationToken token)
        {
            var documents = await _configurationRepository.FilterByAsync(
                 x => x.ApplicationName == applicationName, token);

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
            IsValidValue(request.Value, request.Type);

            var existingConfig = await _configurationRepository.FindOneAsync(x => x.Id == request.Id);
            if (existingConfig == null)
                throw new KeyNotFoundException($"Configuration with ID {request.Key} not found.");

            var config = existingConfig.ToEntity();
            config.Update(request.Value, request.Type);

            await _configurationRepository.ReplaceOneAsync(config.ToDocument(), cancellationToken: token);
            await _messagePublisher.PublishAsync(new ConfigurationRecordUpdated
            {
                IsActive = config.IsActive,
                Environment = config.Environment,
                ApplicationName = config.ApplicationName,
                Key = config.Key,
                Value = config.Value,
                Type = config.Type,
                Version = config.IdentityObject.Version
            });

            return new UpdateConfigurationRecordResponse
            {
                Id = config.IdentityObject.Id,
            };
        }
        private bool IsValidValue(string value, string type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(type)) return false;

            return type.ToLower() switch
            {
                "int" => int.TryParse(value, out _),
                "double" => double.TryParse(value, out _),
                "bool" => bool.TryParse(value, out _),
                "string" => true,
                _ => throw new ValidationException($"Unsupported type: {type}. Supported types are int, double, bool, string.")
            };
        }
    }
}
