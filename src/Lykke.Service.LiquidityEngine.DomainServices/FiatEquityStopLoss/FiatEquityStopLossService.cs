using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.FiatEquityStopLoss
{
    public class FiatEquityStopLossService : IFiatEquityStopLossService
    {
        private readonly IBalanceIndicatorsReportService _balanceIndicatorsReportService;
        private readonly IAssetSettingsService _assetSettingsService;
        private readonly IMarketMakerSettingsService _marketMakerSettingsService;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly ILog _log;

        private readonly object _sync = new object();
        private decimal _fiatEquity;
        private DateTime _fiatEquityUpdated;
        private readonly TimeSpan _fiatEquityUpdateInterval = TimeSpan.FromSeconds(1);

        public FiatEquityStopLossService(
            IBalanceIndicatorsReportService balanceIndicatorsReportService,
            IAssetSettingsService assetSettingsService,
            IMarketMakerSettingsService marketMakerSettingsService,
            IAssetsServiceWithCache assetsServiceWithCache,
            ILogFactory logFactory)
        {
            _balanceIndicatorsReportService = balanceIndicatorsReportService;
            _assetSettingsService = assetSettingsService;
            _marketMakerSettingsService = marketMakerSettingsService;
            _assetsServiceWithCache = assetsServiceWithCache;
            _log = logFactory.CreateLog(this);
        }

        public async Task<decimal> GetFiatEquityMarkup(string assetPairId)
        {
            AssetPair lykkeAssetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);

            Domain.AssetSettings quoteAssetSettings = await _assetSettingsService.GetByLykkeIdAsync(lykkeAssetPair.QuotingAssetId);

            if (quoteAssetSettings.IsCrypto)
                return 0;

            decimal fiatEquity = GetFiatEquity();

            MarketMakerSettings marketMakerSettings = await _marketMakerSettingsService.GetAsync();

            decimal thresholdFrom = marketMakerSettings.FiatEquityThresholdFrom;
            decimal thresholdTo = marketMakerSettings.FiatEquityThresholdTo;
            decimal markupFrom = marketMakerSettings.FiatEquityMarkupFrom;
            decimal markupTo = marketMakerSettings.FiatEquityMarkupTo;

            return CalculateMarkup(fiatEquity, thresholdFrom, thresholdTo, markupFrom, markupTo);
        }

        public static decimal CalculateMarkup(decimal fiatEquity, decimal thresholdFrom, decimal thresholdTo, decimal markupFrom, decimal markupTo)
        {
            if (fiatEquity > -thresholdFrom)
                return 0;

            if (fiatEquity <= -thresholdTo)
                return decimal.MinusOne;

            return markupFrom + (markupTo - markupFrom) * (-fiatEquity - thresholdFrom) / (thresholdTo - thresholdFrom);
        }


        public async Task<IReadOnlyCollection<string>> GetMessages(string assetPairId)
        {
            decimal markup = await GetFiatEquityMarkup(assetPairId);

            if (markup == 0)
                return new List<string>();

            if (markup == decimal.MinusOne)
                return new List<string> { "fiat stop sell" };

            if (markup > 0)
                return new List<string> { "fiat ask markup" };

            _log.WarningWithDetails($"Fiat equity markup has wrong value.", markup);

            return new List<string>();
        }

        private decimal GetFiatEquity()
        {
            lock (_sync)
            {
                if (_fiatEquityUpdated == default(DateTime)
                    || DateTime.UtcNow - _fiatEquityUpdated > _fiatEquityUpdateInterval)
                {
                    _fiatEquity = _balanceIndicatorsReportService.GetAsync().GetAwaiter().GetResult().FiatEquity;
                    _fiatEquityUpdated = DateTime.UtcNow;
                }
                    
                return _fiatEquity;
            }
        }
    }
}
