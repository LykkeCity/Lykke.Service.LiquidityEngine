using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Api;

namespace Lykke.Service.LiquidityEngine.Client
{
    /// <summary>
    /// Liquidity engine service client.
    /// </summary>
    [PublicAPI]
    public interface ILiquidityEngineClient
    {
        /// <summary>
        /// Asset pair links API.
        /// </summary>
        IAssetPairLinksApi AssetPairLinks { get; }

        /// <summary>
        /// Audit API.
        /// </summary>
        IAuditApi Audit { get; }

        /// <summary>
        /// Balances API.
        /// </summary>
        IBalancesApi Balances { get; }

        /// <summary>
        /// Credits API.
        /// </summary>
        ICreditsApi Credits { get; }

        /// <summary>
        /// Cross rate instruments API.
        /// </summary>
        ICrossRateInstrumentsApi CrossRateInstruments { get; }

        /// <summary>
        /// Instruments API.
        /// </summary>
        IInstrumentsApi Instruments { get; }

        /// <summary>
        /// Internal orders API.
        /// </summary>
        IInternalOrdersApi InternalOrders { get; }

        /// <summary>
        /// Market Maker API.
        /// </summary>
        IMarketMakerApi MarketMaker { get; }

        /// <summary>
        /// Order books API.
        /// </summary>
        IOrderBooksApi OrderBooks { get; }

        /// <summary>
        /// Positions API.
        /// </summary>
        IPositionsApi Positions { get; }

        /// <summary>
        /// Quotes API.
        /// </summary>
        IQuotesApi Quotes { get; }

        /// <summary>
        /// Reports API.
        /// </summary>
        IReportsApi Reports { get; }

        /// <summary>
        /// Settlements API.
        /// </summary>
        ISettlementsApi Settlements { get; }

        /// <summary>
        /// Trades API.
        /// </summary>
        ITradesApi Trades { get; }

        /// <summary>
        /// PnL stop loss settings API.
        /// </summary>
        IPnLStopLossSettingsApi PnLStopLossSettings { get; }

        /// <summary>
        /// PnL stop loss engines API.
        /// </summary>
        IPnLStopLossEnginesApi PnLStopLossEngines { get; }

        /// <summary>
        /// Settings API.
        /// </summary>
        ISettingsApi Settings { get; }
    }
}
