using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.Balances.Client;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Service.LiquidityEngine.Managers;
using Lykke.Service.LiquidityEngine.Settings;
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
                _settings.CurrentValue.LiquidityEngineService.WalletId));
            builder.RegisterModule(new AzureRepositories.AutofacModule(
                _settings.Nested(o => o.LiquidityEngineService.Db.DataConnectionString)));

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            
            RegisterClients(builder);
        }

        private void RegisterClients(ContainerBuilder builder)
        {
            builder.RegisterBalancesClient(_settings.CurrentValue.BalancesServiceClient);
            
            builder.Register(container =>
                    new ExchangeOperationsServiceClient(_settings.CurrentValue.ExchangeOperationsServiceClient
                        .ServiceUrl))
                .As<IExchangeOperationsServiceClient>()
                .SingleInstance();
        }
    }
}
