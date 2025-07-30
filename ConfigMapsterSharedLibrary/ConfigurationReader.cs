using ConfigMapster.API.Infrastructure.Exceptions;
using ConfigMapster.API.Infrastructure.Validators;
using ConfigMapster.Services;
using ConfigMapsterSharedLibrary.Documents;
using ConfigurationApi.Entities;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigMapsterSharedLibrary
{
    public class ConfigurationReader
    {
        private readonly ConfigurationCacheService _cacheService;
        private readonly IMongoRepository<ConfigurationRecordDocument> _configRepository;
      
        private readonly string _applicationName;
        private readonly TimeSpan _cacheTTL;

        public ConfigurationReader(
            string applicationName,
            int refreshTimerIntervalInMs,
            ConfigurationCacheService cacheService,
            IMongoRepository<ConfigurationRecordDocument> configRepository)
                    {
            _applicationName = applicationName;
            _cacheTTL = TimeSpan.FromMilliseconds(refreshTimerIntervalInMs);
            _cacheService = cacheService;
            _configRepository = configRepository;
        }

        public async Task<T> GetValueAsync<T>(string key)
        {
            var cacheKey = $"{_applicationName}_{key}";
            var lockTimeout = TimeSpan.FromSeconds(60);

            var cachedValue = await _cacheService.GetValueAsync(cacheKey);
            if (cachedValue != null && cachedValue.Count > 0)
            {
                var value = cachedValue[0].Value;
                return TypeValidator.Parse<T>(value);
            }
            var result = await _cacheService.ExecuteWithDistributedLockAsync(cacheKey, lockTimeout, async () =>
            {
                var cachedValue = await _cacheService.GetValueAsync(cacheKey);
                if (cachedValue != null && cachedValue.Count > 0)
                {
                    var value = cachedValue[0].Value;
                    return TypeValidator.Parse<T>(value);
                }

                var dbValues = await _configRepository.FilterByAsync(
                    x => x.IsActive && x.ApplicationName == _applicationName && x.Key == key,
                    default);
                if (dbValues != null && dbValues.Count > 0)
                {
                    var value = dbValues[0].Value;
                    await _cacheService.SetValueAsync(cacheKey, dbValues[0]);
                    return TypeValidator.Parse<T>(value);
                }

                throw new ObjectNotFoundException<string>($"The configuration key '{key}' was not found.");
            });

            return result;
        }
    }
}