using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.NoFreshQuotesStopLoss
{
    public class NoFreshQuotesStopLossService : INoFreshQuotesStopLossService
    {
        private readonly IQuoteService _quoteService;
        private readonly IMarketMakerSettingsService _marketMakerSettingsService;
        private readonly IInstrumentService _instrumentService;
        private readonly ILog _log;

        public NoFreshQuotesStopLossService(
            IQuoteService quoteService,
            IMarketMakerSettingsService marketMakerSettingsService,
            IInstrumentService instrumentService,
            ILogFactory logFactory)
        {
            _quoteService = quoteService;
            _marketMakerSettingsService = marketMakerSettingsService;
            _instrumentService = instrumentService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<decimal> GetNoFreshQuotesMarkup(string assetPairId)
        {
            MarketMakerSettings marketMakerSettings = await _marketMakerSettingsService.GetAsync();

            TimeSpan noFreshQuotesInterval = marketMakerSettings.NoFreshQuotesInterval;
            decimal noFreshQuotesMarkup = marketMakerSettings.NoFreshQuotesMarkup;

            if (noFreshQuotesInterval == default(TimeSpan) || noFreshQuotesMarkup == default(decimal))
                return 0;

            Quote quote = await _quoteService.GetAsync(ExchangeNames.B2C2, assetPairId);

            if (quote == null)
                return 0;

            if (DateTime.UtcNow - quote.Time > noFreshQuotesInterval)
                return noFreshQuotesMarkup;

            return 0;
        }

        public async Task<IReadOnlyCollection<string>> GetMessagesAsync(string assetPairId)
        {
            decimal markup = await GetNoFreshQuotesMarkup(assetPairId);

            if (markup == 0)
                return new List<string>();

            if (markup > 0)
                return new List<string> { "no fresh quotes" };

            _log.WarningWithDetails("No fresh quotes markup has wrong value.", markup);

            return new List<string>();
        }

        public async Task<IReadOnlyCollection<AssetPairMarkup>> GetMarkupsAsync()
        {
            var result = new List<AssetPairMarkup>();

            var instruments = await _instrumentService.GetAllAsync();

            var assetPairIds = instruments.Select(x => x.AssetPairId).ToList();

            foreach (var assetPairId in assetPairIds)
            {
                decimal noFreshQuotesMarkups = await GetNoFreshQuotesMarkup(assetPairId);

                result.Add(new AssetPairMarkup
                {
                    AssetPairId = assetPairId,
                    TotalMarkup = noFreshQuotesMarkups,
                    TotalAskMarkup = noFreshQuotesMarkups,
                    TotalBidMarkup = noFreshQuotesMarkups
                });
            }

            return result;
        }
    }
}
