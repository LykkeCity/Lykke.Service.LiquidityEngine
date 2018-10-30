using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Rates
{
    [UsedImplicitly]
    public class RateService : IRateService
    {
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly IQuoteService _quoteService;
        private readonly ICrossRateInstrumentService _crossInstrumentService;
        private readonly ILog _log;

        public RateService(
            IAssetsServiceWithCache assetsServiceWithCache,
            IQuoteService quoteService,
            ICrossRateInstrumentService crossInstrumentService,
            ILogFactory logFactory)
        {
            _assetsServiceWithCache = assetsServiceWithCache;
            _quoteService = quoteService;
            _crossInstrumentService = crossInstrumentService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<decimal?> CalculatePriceInUsd(string assetPairId, decimal price, bool isSell)
        {
            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);

            if (assetPair == null)
            {
                _log.WarningWithDetails("Asset pair is not found.", new
                {
                    AssetPairId = assetPairId
                });
                return null;
            }

            if (assetPair.QuotingAssetId == AssetConsts.UsdAssetId)
            {
                return price;
            }

            CrossRateInstrument crossInstrument = await _crossInstrumentService.GetByAssetPairIdAsync(assetPairId);

            if (crossInstrument == null)
            {
                _log.WarningWithDetails("Cross instrument is not configured", new
                {
                    AssetPair = assetPairId
                });
                return null;
            }

            Quote quote = await _quoteService
                .GetAsync(crossInstrument.QuoteSource, crossInstrument.ExternalAssetPairId);

            if (quote == null)
            {
                _log.WarningWithDetails("No quote for instrument", new
                {
                    Source = crossInstrument.QuoteSource,
                    AssetPair = crossInstrument.ExternalAssetPairId
                });
                return null;
            }

            return isSell
                ? Calculator.CalculateDirectSellPrice(price, quote, crossInstrument.IsInverse)
                : Calculator.CalculateDirectBuyPrice(price, quote, crossInstrument.IsInverse);
        }
    }
}
