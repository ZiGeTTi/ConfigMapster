using Autofac.Extras.Moq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using ConfigMapster.API.ApplicationService.Services;
using ConfigMapster.API.ApplicationService.Services.Interfaces;
using ConfigMapster.API.Persistence.Redis;
using ConfigMapster.Services;
using ConfigurationApi.Documents;
using ConfigurationApi.Entities;
using ConfigurationApi.Models.Requests;
using ConfigurationApi.Models.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;
using IMongoRepository = IMongoRepository<ConfigurationApi.Documents.ConfigurationRecordDocument>;

namespace ConfigMapster.Tests
{
    public class ConfiguraitonServiceTests
    {
 

        private readonly IFixture _fixture;

        public ConfiguraitonServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

            // Register mocks for constructor dependencies
            var redisConfig = new RedisConfig { Url = "localhost:6379", Database = 0, ExpireTimeSpan = 1 };
            var optionsMock = new Mock<IOptions<RedisConfig>>();
            optionsMock.Setup(x => x.Value).Returns(redisConfig);
            _fixture.Register(() => optionsMock.Object);

            var redisClientFactoryMock = new Mock<RedisClientFactory>(optionsMock.Object);
            _fixture.Register(() => redisClientFactoryMock.Object);

            var cacheServiceMock = new Mock<ConfigurationCacheService>(redisClientFactoryMock.Object, redisConfig);
            _fixture.Register(() => cacheServiceMock.Object);
        }

        [Fact]
        public async Task CreateConfiguraitonAsync_ShouldInsertAndReturnId()
        {
            var mongoRepoMock = _fixture.Freeze<Mock<IMongoRepository<ConfigurationRecordDocument>>>();
            mongoRepoMock.Setup(x => x.InsertOneAsync(It.IsAny<ConfigurationRecordDocument>()))
                .Returns(Task.CompletedTask);

            var service = _fixture.Create<ConfiguraitonService>();

            var request = new CreateConfigurationRecordRequest
            {
                Environment = "dev",
                ApplicationName = "app",
                Key = "key",
                Value = "value",
                Type = "string"
            };

            var response = await service.CreateConfiguraitonAsync(request, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, response.Id);
        }
        [Fact]
        public async Task DeleteConfiguraitonAsync_ShouldDeleteConfiguration()
        {
            var mongoRepoMock = _fixture.Freeze<Mock<IMongoRepository<ConfigurationRecordDocument>>>();
            mongoRepoMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ConfigurationRecordDocument { Id = Guid.NewGuid() });
            mongoRepoMock.Setup(x => x.ReplaceOneAsync(It.IsAny<ConfigurationRecordDocument>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var service = _fixture.Create<ConfiguraitonService>();
            await service.DeleteConfiguraitonAsync(Guid.NewGuid(), CancellationToken.None);
            mongoRepoMock.Verify(x => x.ReplaceOneAsync(It.IsAny<ConfigurationRecordDocument>(), It.IsAny<CancellationToken>()), Times.Once);
        }
 
        [Fact]
        public async Task UpdateConfiguraitonAsync_ShouldUpdateConfiguration()
        {
            var mongoRepoMock = _fixture.Freeze<Mock<IMongoRepository<ConfigurationRecordDocument>>>();
            mongoRepoMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ConfigurationRecordDocument { Id = Guid.NewGuid() });
            mongoRepoMock.Setup(x => x.ReplaceOneAsync(It.IsAny<ConfigurationRecordDocument>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var service = _fixture.Create<ConfiguraitonService>();
            var request = new UpdateConfigurationRecordRequest
            {
                Id = Guid.NewGuid(),
                Environment = "dev",
                ApplicationName = "app",
                Key = "key",
                Value = "new value",
                Type = "string"
            };
            var response = await service.UpdateConfiguraitonAsync(request, CancellationToken.None);
            Assert.NotNull(response);
        }
    }
}
