using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    [UsedImplicitly]
    public class MarketMakerService : IMarketMakerService
    {
        private readonly IInstrumentService _instrumentService;
        private readonly ILykkeExchangeService _lykkeExchangeService;
        private readonly IOrderBookService _orderBookService;
        private readonly IBalanceService _balanceService;
        private readonly IQuoteService _quoteService;
        private readonly IQuoteTimeoutSettingsService _quoteTimeoutSettingsService;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly ILog _log;

        public MarketMakerService(
            IInstrumentService instrumentService,
            ILykkeExchangeService lykkeExchangeService,
            IOrderBookService orderBookService,
            IBalanceService balanceService,
            IQuoteService quoteService,
            IQuoteTimeoutSettingsService quoteTimeoutSettingsService,
            IAssetsServiceWithCache assetsServiceWithCache,
            ILogFactory logFactory)
        {
            _instrumentService = instrumentService;
            _lykkeExchangeService = lykkeExchangeService;
            _orderBookService = orderBookService;
            _balanceService = balanceService;
            _quoteService = quoteService;
            _quoteTimeoutSettingsService = quoteTimeoutSettingsService;
            _assetsServiceWithCache = assetsServiceWithCache;
            _log = logFactory.CreateLog(this);
        }

        public async Task UpdateOrderBooksAsync()
        {
            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            IReadOnlyCollection<Instrument> activeInstruments = instruments
                .Where(o => o.Mode == InstrumentMode.Idle || o.Mode == InstrumentMode.Active)
                .ToArray();

            await Task.WhenAll(activeInstruments.Select(ProcessInstrumentAsync));
        }

        private async Task ProcessInstrumentAsync(Instrument instrument)
        {
            Quote quote = await _quoteService.GetAsync(instrument.AssetPairId);

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            var internalLimitOrders = new List<LimitOrder>();

            foreach (InstrumentLevel instrumentLevel in instrument.Levels.OrderBy(o => o.Number))
            {
                decimal sellPrice = (quote.Ask * (1 + instrumentLevel.Markup))
                    .TruncateDecimalPlaces(assetPair.Accuracy, true);
                decimal buyPrice = (quote.Bid * (1 - instrumentLevel.Markup))
                    .TruncateDecimalPlaces(assetPair.Accuracy);

                decimal volume = Math.Round(instrumentLevel.Volume, assetPair.InvertedAccuracy);

                internalLimitOrders.Add(LimitOrder.CreateSell(sellPrice, volume));
                internalLimitOrders.Add(LimitOrder.CreateBuy(buyPrice, volume));
            }

            await ValidateQuoteAsync(internalLimitOrders, quote);

            ValidateMinVolume(internalLimitOrders, (decimal) assetPair.MinVolume);

            await ValidateBalance(internalLimitOrders, assetPair);

            await _orderBookService.UpdateAsync(new OrderBook
            {
                AssetPairId = instrument.AssetPairId,
                Time = DateTime.UtcNow,
                LimitOrders = internalLimitOrders
            });

            if (instrument.Mode == InstrumentMode.Active)
            {
                await _lykkeExchangeService.ApplyAsync(instrument.AssetPairId, internalLimitOrders);
            }
            else
            {
                foreach (LimitOrder limitOrder in internalLimitOrders)
                {
                    if (limitOrder.Error == LimitOrderError.None)
                        limitOrder.Error = LimitOrderError.Idle;
                }
            }
        }

        private async Task ValidateQuoteAsync(IReadOnlyCollection<LimitOrder> limitOrders, Quote quote)
        {
            QuoteTimeoutSettings quoteTimeoutSettings = await _quoteTimeoutSettingsService.GetAsync();

            if (quoteTimeoutSettings.Enabled && DateTime.UtcNow - quote.Time > quoteTimeoutSettings.Error)
            {
                _log.WarningWithDetails("Quote timeout is expired", new
                {
                    quote,
                    Timeout = quoteTimeoutSettings.Error
                });

                foreach (LimitOrder limitOrder in limitOrders.Where(o => o.Error == LimitOrderError.None))
                    limitOrder.Error = LimitOrderError.InvalidQuote;
            }
        }

        private void ValidateMinVolume(IReadOnlyCollection<LimitOrder> limitOrders, decimal minVolume)
        {
            foreach (LimitOrder limitOrder in limitOrders.Where(o => o.Error == LimitOrderError.None))
            {
                if (limitOrder.Volume < minVolume)
                    limitOrder.Error = LimitOrderError.TooSmallVolume;
            }
        }

        private async Task ValidateBalance(IReadOnlyCollection<LimitOrder> limitOrders, AssetPair assetPair)
        {
            Asset baseAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.BaseAssetId);
            Asset quoteAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.QuotingAssetId);

            List<LimitOrder> sellLimitOrders = limitOrders
                .Where(o => o.Error == LimitOrderError.None)
                .Where(o => o.Type == LimitOrderType.Sell)
                .OrderBy(o => o.Price)
                .ToList();

            List<LimitOrder> buyLimitOrders = limitOrders
                .Where(o => o.Error == LimitOrderError.None)
                .Where(o => o.Type == LimitOrderType.Buy)
                .OrderByDescending(o => o.Price)
                .ToList();

            if (sellLimitOrders.Any())
            {
                decimal balance = (await _balanceService.GetByAssetIdAsync(baseAsset.Id)).Amount;

                foreach (LimitOrder limitOrder in sellLimitOrders)
                {
                    decimal amount = limitOrder.Volume.TruncateDecimalPlaces(baseAsset.Accuracy, true);

                    if (balance - amount < 0)
                    {
                        decimal volume = balance.TruncateDecimalPlaces(assetPair.InvertedAccuracy);

                        if (volume < (decimal) assetPair.MinVolume)
                            limitOrder.Error = LimitOrderError.NotEnoughFunds;
                        else
                            limitOrder.UpdateVolume(volume);
                    }

                    balance -= amount;
                }
            }

            if (buyLimitOrders.Any())
            {
                decimal balance = (await _balanceService.GetByAssetIdAsync(quoteAsset.Id)).Amount;

                foreach (LimitOrder limitOrder in buyLimitOrders)
                {
                    decimal amount = (limitOrder.Price * limitOrder.Volume)
                        .TruncateDecimalPlaces(quoteAsset.Accuracy, true);

                    if (balance - amount < 0)
                    {
                        decimal volume = (balance / limitOrder.Price).TruncateDecimalPlaces(assetPair.InvertedAccuracy);

                        if (volume < (decimal) assetPair.MinVolume)
                            limitOrder.Error = LimitOrderError.NotEnoughFunds;
                        else
                            limitOrder.UpdateVolume(volume);
                    }

                    balance -= amount;
                }
            }
        }
    }
}
