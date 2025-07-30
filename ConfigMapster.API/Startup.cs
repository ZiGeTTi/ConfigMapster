using ConfigMapster.API.ApplicationService;
using ConfigMapster.API.Domain.Events;
using ConfigMapster.API.Infrastructure;
using ConfigMapster.Middlewares;
using ConfigMapster.Services;
using ConfigMapsterSharedLibrary;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ConfigMapster
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSingleton<DomainEventDispatcher>();
            services.AddPersistenceServices(Configuration);
            services.AddInfraServices(Configuration);
            services.AddApplicationServices(Configuration);

            services.AddScoped<ConfigurationReader>(provider =>
            {
                var cacheService = provider.GetRequiredService<ConfigurationCacheService>();
                var configRepository = provider.GetRequiredService<IMongoRepository<ConfigMapsterSharedLibrary.Documents.ConfigurationRecordDocument>>();
               // var redLockFactory = provider.GetRequiredService<IDistributedLockFactory>();
                var appName = Configuration["ApplicationName"] ?? "SERVICE-A";
                var interval = int.TryParse(Configuration["RefreshTimerIntervalInMs"], out var val) ? val : 10000;
                return new ConfigurationReader(appName, interval, cacheService, configRepository);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConfigMapster", Version = "v1" });
            });
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConfigMapster v1"));
            }
            app.UseMiddleware<CustomExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
