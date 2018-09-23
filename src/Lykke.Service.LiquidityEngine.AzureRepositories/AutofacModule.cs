using Autofac;
using AzureStorage.Tables;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.AzureRepositories.AssetPairLinks;
using Lykke.Service.LiquidityEngine.AzureRepositories.Instruments;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.LiquidityEngine.AzureRepositories
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<string> _connectionString;

        public AutofacModule(IReloadingManager<string> connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(container => new AssetPairLinkRepository(
                    AzureTableStorage<AssetPairLinkEntity>.Create(_connectionString,
                        "AssetPairLinks", container.Resolve<ILogFactory>())))
                .As<IAssetPairLinkRepository>()
                .SingleInstance();
            
            builder.Register(container => new InstrumentRepository(
                    AzureTableStorage<InstrumentEntity>.Create(_connectionString,
                        "Instruments", container.Resolve<ILogFactory>())))
                .As<IInstrumentRepository>()
                .SingleInstance();
        }
    }
}
