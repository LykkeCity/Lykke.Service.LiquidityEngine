using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IInstrumentService _instrumentService;
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
            IInstrumentService instrumentService,
            ILogFactory logFactory)
        {
            _balanceIndicatorsReportService = balanceIndicatorsReportService;
            _assetSettingsService = assetSettingsService;
            _marketMakerSettingsService = marketMakerSettingsService;
            _assetsServiceWithCache = assetsServiceWithCache;
            _instrumentService = instrumentService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<decimal> GetFiatEquityMarkup(string assetPairId)
        {
            AssetPair lykkeAssetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);

            Domain.AssetSettings quoteAssetSettings = await _assetSettingsService.GetByLykkeIdAsync(lykkeAssetPair.QuotingAssetId);

            if (quoteAssetSettings == null)
            {
                _log.WarningWithDetails("Can't find asset settings while calculating fiat equity.", lykkeAssetPair.QuotingAssetId);

                return 0;
            }

            if (quoteAssetSettings.IsCrypto)
                return 0;

            decimal fiatEquity = GetFiatEquity();

            if (fiatEquity >=0)
                return 0;

            MarketMakerSettings marketMakerSettings = await _marketMakerSettingsService.GetAsync();

            decimal thresholdFrom = marketMakerSettings.FiatEquityThresholdFrom;
            decimal thresholdTo = marketMakerSettings.FiatEquityThresholdTo;
            decimal markupFrom = marketMakerSettings.FiatEquityMarkupFrom;
            decimal markupTo = marketMakerSettings.FiatEquityMarkupTo;

            if (thresholdFrom >= thresholdTo)
                return 0;

            decimal markup =  CalculateMarkup(fiatEquity, thresholdFrom, thresholdTo, markupFrom, markupTo);

            return markup;
        }

        public static decimal CalculateMarkup(decimal fiatEquity, decimal thresholdFrom, decimal thresholdTo, decimal markupFrom, decimal markupTo)
        {
            if (fiatEquity > -thresholdFrom)
                return 0;

            if (fiatEquity <= -thresholdTo)
                return decimal.MinusOne;

            decimal result = markupFrom + (markupTo - markupFrom) * (-fiatEquity - thresholdFrom) / (thresholdTo - thresholdFrom);

            return result;
        }

        public async Task<IReadOnlyCollection<AssetPairMarkup>> GetMarkupsAsync()
        {
            var result = new List<AssetPairMarkup>();

            var instruments = await _instrumentService.GetAllAsync();

            var assetPairIds = instruments.Select(x => x.AssetPairId).ToList();

            foreach (var assetPairId in assetPairIds)
            {
                decimal fiatMarkups = await GetFiatEquityMarkup(assetPairId);

                result.Add(new AssetPairMarkup
                {
                    AssetPairId = assetPairId,
                    TotalMarkup = 0,
                    TotalAskMarkup = fiatMarkups,
                    TotalBidMarkup = 0
                });
            }

            return result;
        }

        public async Task<IReadOnlyCollection<string>> GetMessagesAsync(string assetPairId)
        {
            decimal markup = await GetFiatEquityMarkup(assetPairId);

            if (markup == 0)
                return new List<string>();

            if (markup == decimal.MinusOne)
                return new List<string> { "fiat stop sell" };

            if (markup > 0)
                return new List<string> { "fiat ask markup" };

            _log.WarningWithDetails("Fiat equity markup has wrong value.", markup);

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
