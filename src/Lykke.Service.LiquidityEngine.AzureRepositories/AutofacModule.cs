using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.AzureRepositories.AssetPairLinks;
using Lykke.Service.LiquidityEngine.AzureRepositories.BalanceOperations;
using Lykke.Service.LiquidityEngine.AzureRepositories.Credits;
using Lykke.Service.LiquidityEngine.AzureRepositories.Instruments;
using Lykke.Service.LiquidityEngine.AzureRepositories.MarketMaker;
using Lykke.Service.LiquidityEngine.AzureRepositories.Positions;
using Lykke.Service.LiquidityEngine.AzureRepositories.Reports;
using Lykke.Service.LiquidityEngine.AzureRepositories.Settings;
using Lykke.Service.LiquidityEngine.AzureRepositories.Trades;
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

            builder.Register(container => new BalanceOperationRepository(
                    AzureTableStorage<BalanceOperationEntity>.Create(_connectionString,
                        "BalanceOperations", container.Resolve<ILogFactory>())))
                .As<IBalanceOperationRepository>()
                .SingleInstance();

            builder.Register(container => new CreditRepository(
                    AzureTableStorage<CreditEntity>.Create(_connectionString,
                        "Credits", container.Resolve<ILogFactory>())))
                .As<ICreditRepository>()
                .SingleInstance();

            builder.Register(container => new InstrumentRepository(
                    AzureTableStorage<InstrumentEntity>.Create(_connectionString,
                        "Instruments", container.Resolve<ILogFactory>())))
                .As<IInstrumentRepository>()
                .SingleInstance();

            builder.Register(container => new TimersSettingsRepository(
                    AzureTableStorage<TimersSettingsEntity>.Create(_connectionString,
                        "Settings", container.Resolve<ILogFactory>())))
                .As<ITimersSettingsRepository>()
                .SingleInstance();

            builder.Register(container => new QuoteTimeoutSettingsRepository(
                    AzureTableStorage<QuoteTimeoutSettingsEntity>.Create(_connectionString,
                        "Settings", container.Resolve<ILogFactory>())))
                .As<IQuoteTimeoutSettingsRepository>()
                .SingleInstance();

            builder.Register(container => new QuoteThresholdSettingsRepository(
                    AzureTableStorage<QuoteThresholdSettingsEntity>.Create(_connectionString,
                        "Settings", container.Resolve<ILogFactory>())))
                .As<IQuoteThresholdSettingsRepository>()
                .SingleInstance();

            builder.Register(container => new InternalTradeRepository(
                    AzureTableStorage<InternalTradeEntity>.Create(_connectionString,
                        "InternalTrades", container.Resolve<ILogFactory>()),
                    AzureTableStorage<AzureIndex>.Create(_connectionString,
                        "InternalTradesIndices", container.Resolve<ILogFactory>())))
                .As<IInternalTradeRepository>()
                .SingleInstance();

            builder.Register(container => new ExternalTradeRepository(
                    AzureTableStorage<ExternalTradeEntity>.Create(_connectionString,
                        "ExternalTrades", container.Resolve<ILogFactory>()),
                    AzureTableStorage<AzureIndex>.Create(_connectionString,
                        "ExternalTradesIndices", container.Resolve<ILogFactory>())))
                .As<IExternalTradeRepository>()
                .SingleInstance();

            builder.Register(container => new PositionRepository(
                    AzureTableStorage<PositionEntity>.Create(_connectionString,
                        "Positions", container.Resolve<ILogFactory>()),
                    AzureTableStorage<AzureIndex>.Create(_connectionString,
                        "PositionsIndices", container.Resolve<ILogFactory>())))
                .As<IPositionRepository>()
                .SingleInstance();

            builder.Register(container => new OpenPositionRepository(
                    AzureTableStorage<PositionEntity>.Create(_connectionString,
                        "OpenPositions", container.Resolve<ILogFactory>())))
                .As<IOpenPositionRepository>()
                .SingleInstance();

            builder.Register(container => new SummaryReportRepository(
                    AzureTableStorage<SummaryReportEntity>.Create(_connectionString,
                        "SummaryReports", container.Resolve<ILogFactory>())))
                .As<ISummaryReportRepository>()
                .SingleInstance();

            builder.Register(container => new MarketMakerStateRepository(
                    AzureTableStorage<MarketMakerStateEntity>.Create(_connectionString,
                        "MarketMakerState", container.Resolve<ILogFactory>())))
                .As<IMarketMakerStateRepository>()
                .SingleInstance();
        }
    }
}
