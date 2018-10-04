using Lykke.HttpClientGenerator;
using Lykke.Service.LiquidityEngine.Client.Api;

namespace Lykke.Service.LiquidityEngine.Client
{
    /// <summary>
    /// Liquidity engine service client.
    /// </summary>
    public class LiquidityEngineClient : ILiquidityEngineClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LiquidityEngineClient"/> with <param name="httpClientGenerator"></param>.
        /// </summary> 
        public LiquidityEngineClient(IHttpClientGenerator httpClientGenerator)
        {
            AssetPairLinks = httpClientGenerator.Generate<IAssetPairLinksApi>();
            Audit = httpClientGenerator.Generate<IAuditApi>();
            Balances = httpClientGenerator.Generate<IBalancesApi>();
            Credits = httpClientGenerator.Generate<ICreditsApi>();
            Instruments = httpClientGenerator.Generate<IInstrumentsApi>();
            OrderBooks = httpClientGenerator.Generate<IOrderBooksApi>();
            Positions = httpClientGenerator.Generate<IPositionsApi>();
            Reports = httpClientGenerator.Generate<IReportsApi>();
            Settlements = httpClientGenerator.Generate<ISettlementsApi>();
            Trades = httpClientGenerator.Generate<ITradesApi>();
            Settings = httpClientGenerator.Generate<ISettingsApi>();
        }

        /// <inheritdoc/>
        public IAssetPairLinksApi AssetPairLinks { get; }

        /// <inheritdoc/>
        public IAuditApi Audit { get; }

        /// <inheritdoc/>
        public IBalancesApi Balances { get; }

        /// <inheritdoc/>
        public ICreditsApi Credits { get; }

        /// <inheritdoc/>
        public IInstrumentsApi Instruments { get; }

        /// <inheritdoc/>
        public IOrderBooksApi OrderBooks { get; }

        /// <inheritdoc/>
        public IPositionsApi Positions { get; }

        /// <inheritdoc/>
        public IReportsApi Reports { get; }

        /// <inheritdoc/>
        public ISettlementsApi Settlements { get; }

        /// <inheritdoc/>
        public ITradesApi Trades { get; }

        /// <inheritdoc/>
        public ISettingsApi Settings { get; }
    }
}
