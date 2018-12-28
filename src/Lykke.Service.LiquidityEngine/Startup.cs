using JetBrains.Annotations;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using AutoMapper;
using AutoMapper.Data;
using Microsoft.Extensions.Logging;

namespace Lykke.Service.LiquidityEngine
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "LiquidityEngine API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.Extend = (serviceCollection, settings) =>
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.AddDataReaderMapping();
                        cfg.AddProfiles(typeof(AzureRepositories.AutoMapperProfile));
                        cfg.AddProfiles(typeof(PostgresRepositories.AutoMapperProfile));
                        cfg.AddProfiles(typeof(AutoMapperProfile));
                    });

                    Mapper.AssertConfigurationIsValid();
                };

                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "LiquidityEngineLog";
                    logs.AzureTableConnectionStringResolver =
                        settings => settings.LiquidityEngineService.Db.LogsConnectionString;

                    logs.Extended = extendedLogs =>
                    {
                        extendedLogs.AddAdditionalSlackChannel("liquidity-market-maker-errors",
                            channelOptions => { channelOptions.MinLogLevel = LogLevel.Warning; });
                    };
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options => { options.SwaggerOptions = _swaggerOptions; });
        }
    }
}
