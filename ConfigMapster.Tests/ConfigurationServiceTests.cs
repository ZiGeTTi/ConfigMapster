using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using ConfigMapster.API.ApplicationService.Services;
using ConfigMapster.API.ApplicationService.Services.Interfaces;
using ConfigurationApi.Documents;
using ConfigurationApi.Models.Requests;
using ConfigurationApi.Models.Responses;
using IMongoRepository = IMongoRepository<ConfigurationApi.Documents.ConfigurationRecordDocument>;
using ConfigMapster.Services;
using ConfigurationApi.Entities;

namespace ConfigMapster.Tests
{
    public class ConfiguraitonServiceTests
    {
        private readonly Mock<IMongoRepository> _mongoRepoMock;
        private readonly Mock<ConfigurationCacheService> _cacheServiceMock;
        private readonly Mock<ILogger<ConfiguraitonService>> _loggerMock;
        private readonly ConfiguraitonService _service;

        public ConfiguraitonServiceTests()
        {
            _mongoRepoMock = new Mock<IMongoRepository>();
            _cacheServiceMock = new Mock<ConfigurationCacheService>(null, null);
            _loggerMock = new Mock<ILogger<ConfiguraitonService>>();
            _service = new ConfiguraitonService(_mongoRepoMock.Object, _cacheServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateConfiguraitonAsync_ShouldInsertAndReturnId()
        {
            var request = new CreateConfigurationRecordRequest
            {
                Environment = "dev",
                ApplicationName = "app",
                Key = "key",
                Value = "value",
                Type = "string"
            };
            _mongoRepoMock.Setup(x => x.InsertOneAsync(It.IsAny<ConfigurationRecordDocument>()))
                .Returns(Task.CompletedTask);

            var response = await _service.CreateConfiguraitonAsync(request, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, response.Id);
        }

        [Fact]
        public async Task DeleteConfiguraitonAsync_ShouldDeleteConfig()
        {
            var id = Guid.NewGuid();
            var doc = new ConfigurationRecordDocument { Id = id, Environment = "dev", ApplicationName = "app", Key = "key", Value = "value", Type = "string", IsActive = true };
            _mongoRepoMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(doc);
            _mongoRepoMock.Setup(x => x.ReplaceOneAsync(It.IsAny<ConfigurationRecordDocument>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            await _service.DeleteConfiguraitonAsync(id, CancellationToken.None);
            _mongoRepoMock.Verify(x => x.ReplaceOneAsync(It.IsAny<ConfigurationRecordDocument>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ListConfigurations_ShouldReturnRecords()
        {
            var docs = new List<ConfigurationRecordDocument>
            {
                new ConfigurationRecordDocument { Id = Guid.NewGuid(), Environment = "dev", ApplicationName = "app", Key = "key", Value = "value", Type = "string", IsActive = true }
            };
            _mongoRepoMock.Setup(x => x.FilterByAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ConfigurationRecordDocument, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(docs);

            var result = await _service.ListConfigurations("dev", "app", CancellationToken.None);
            Assert.Single(result.Items);
            Assert.Equal("dev", result.Items[0].Environment);
        }

        [Fact]
        public async Task UpdateConfiguraitonAsync_ShouldUpdateAndReturnId()
        {
            var id = Guid.NewGuid();
            var doc = new ConfigurationRecordDocument { Id = id, Environment = "dev", ApplicationName = "app", Key = "key", Value = "value", Type = "string", IsActive = true };
            _mongoRepoMock.Setup(x => x.FindOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ConfigurationRecordDocument, bool>>>())).ReturnsAsync(doc);
            _mongoRepoMock.Setup(x => x.ReplaceOneAsync(It.IsAny<ConfigurationRecordDocument>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var request = new UpdateConfigurationRecordRequest { Id = id, Value = "newValue", Type = "string" };
            var response = await _service.UpdateConfiguraitonAsync(request, CancellationToken.None);
            Assert.Equal(id, response.Id);
        }
    }
}
