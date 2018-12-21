using System;
using System.Net;
using Autofac;
using JetBrains.Annotations;
using Lykke.B2c2Client;
using Lykke.B2c2Client.Settings;
using Lykke.Sdk;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.Dwh.Client;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Service.LiquidityEngine.Managers;
using Lykke.Service.LiquidityEngine.Migration;
using Lykke.Service.LiquidityEngine.Migration.Operations;
using Lykke.Service.LiquidityEngine.Rabbit;
using Lykke.Service.LiquidityEngine.Rabbit.Subscribers;
using Lykke.Service.LiquidityEngine.Settings;
using Lykke.Service.LiquidityEngine.Settings.Clients.MatchingEngine;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers.Quotes;
using Lykke.SettingsReader;

namespace Lykke.Service.LiquidityEngine
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public AutofacModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DomainServices.AutofacModule(
                _settings.CurrentValue.LiquidityEngineService.Name,
                _settings.CurrentValue.LiquidityEngineService.WalletId,
                _settings.CurrentValue.LiquidityEngineService.Dwh.DatabaseName,
                _settings.CurrentValue.LiquidityEngineService.Dwh.StoredProcedures.Trades));
            builder.RegisterModule(new AzureRepositories.AutofacModule(
                _settings.Nested(o => o.LiquidityEngineService.Db.DataConnectionString),
                _settings.Nested(o => o.LiquidityEngineService.Db.LykkeTradesMeQueuesDeduplicatorConnectionString)));
            builder.RegisterModule(new PostgresRepositories.AutofacModule(
                _settings.CurrentValue.LiquidityEngineService.Db.PostgresConnectionString));

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterMigration(builder);

            RegisterClients(builder);

            RegisterRabbit(builder);
        }

        private void RegisterClients(ContainerBuilder builder)
        {
            builder.RegisterAssetsClient(AssetServiceSettings.Create(
                new Uri(_settings.CurrentValue.AssetsServiceClient.ServiceUrl),
                _settings.CurrentValue.LiquidityEngineService.AssetsCacheExpirationPeriod));

            builder.RegisterBalancesClient(_settings.CurrentValue.BalancesServiceClient);

            builder.Register(container =>
                    new ExchangeOperationsServiceClient(_settings.CurrentValue.ExchangeOperationsServiceClient
                        .ServiceUrl))
                .As<IExchangeOperationsServiceClient>()
                .SingleInstance();

            builder.RegisterB2С2RestClient(new B2C2ClientSettings(_settings.CurrentValue.B2C2Client.Url,
                _settings.CurrentValue.B2C2Client.AuthorizationToken));

            builder.RegisterLykkeServiceClient(_settings.CurrentValue.DwhServiceClient.ServiceUrl, null);

            MatchingEngineClientSettings matchingEngineClientSettings = _settings.CurrentValue.MatchingEngineClient;

            if (!IPAddress.TryParse(matchingEngineClientSettings.IpEndpoint.Host, out var address))
                address = Dns.GetHostAddressesAsync(matchingEngineClientSettings.IpEndpoint.Host).Result[0];

            var endPoint = new IPEndPoint(address, matchingEngineClientSettings.IpEndpoint.Port);

            builder.RegisgterMeClient(endPoint);
        }

        private void RegisterRabbit(ContainerBuilder builder)
        {
            builder.RegisterType<LykkeTradeSubscriberMonitor>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<LykkeTradeSubscriber>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.LiquidityEngineService.Rabbit.Subscribers
                    .LykkeTrades))
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<B2C2QuoteSubscriber>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.LiquidityEngineService.Rabbit.Subscribers
                    .B2C2Quotes))
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<B2C2OrderBooksSubscriber>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.LiquidityEngineService.Rabbit.Subscribers
                    .B2C2OrderBooks))
                .AsSelf()
                .SingleInstance();

            QuotesSettings quotesSettings = _settings.CurrentValue.LiquidityEngineService.Rabbit.Subscribers.Quotes;

            foreach (Exchange exchange in quotesSettings.Exchanges)
            {
                builder.RegisterType<QuoteSubscriber>()
                    .AsSelf()
                    .WithParameter(TypedParameter.From(new SubscriberSettings
                    {
                        Exchange = exchange.Endpoint,
                        Queue = quotesSettings.Queue,
                        ConnectionString = quotesSettings.ConnectionString
                    }))
                    .SingleInstance();
            }
        }

        private void RegisterMigration(ContainerBuilder builder)
        {
            builder.RegisterType<StorageMigrationService>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<MigrateSummaryReportOperation>()
                .As<IMigrationOperation>()
                .SingleInstance();
        }
    }
}
