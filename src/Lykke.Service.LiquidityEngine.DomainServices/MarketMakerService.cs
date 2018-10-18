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
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
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
        private readonly IMarketMakerStateService _marketMakerStateService;
        private readonly IQuoteService _quoteService;
        private readonly IQuoteTimeoutSettingsService _quoteTimeoutSettingsService;
        private readonly ISummaryReportService _summaryReportService;
        private readonly IPositionService _positionService;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly ILog _log;

        public MarketMakerService(
            IInstrumentService instrumentService,
            ILykkeExchangeService lykkeExchangeService,
            IOrderBookService orderBookService,
            IBalanceService balanceService,
            IMarketMakerStateService marketMakerStateService,
            IQuoteService quoteService,
            IQuoteTimeoutSettingsService quoteTimeoutSettingsService,
            ISummaryReportService summaryReportService,
            IPositionService positionService,
            IAssetsServiceWithCache assetsServiceWithCache,
            ILogFactory logFactory)
        {
            _instrumentService = instrumentService;
            _lykkeExchangeService = lykkeExchangeService;
            _orderBookService = orderBookService;
            _balanceService = balanceService;
            _marketMakerStateService = marketMakerStateService;
            _quoteService = quoteService;
            _quoteTimeoutSettingsService = quoteTimeoutSettingsService;
            _summaryReportService = summaryReportService;
            _positionService = positionService;
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
            Quote quote = await _quoteService.GetAsync(ExchangeNames.B2C2, instrument.AssetPairId);

            if (quote == null)
            {
                _log.WarningWithDetails("No quote for instrument", new {instrument.AssetPairId});
                return;
            }

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            var limitOrders = new List<LimitOrder>();

            foreach (InstrumentLevel instrumentLevel in instrument.Levels.OrderBy(o => o.Number))
            {
                decimal sellPrice = (quote.Ask * (1 + instrumentLevel.Markup))
                    .TruncateDecimalPlaces(assetPair.Accuracy, true);
                decimal buyPrice = (quote.Bid * (1 - instrumentLevel.Markup))
                    .TruncateDecimalPlaces(assetPair.Accuracy);

                decimal volume = Math.Round(instrumentLevel.Volume, assetPair.InvertedAccuracy);

                limitOrders.Add(LimitOrder.CreateSell(sellPrice, volume));
                limitOrders.Add(LimitOrder.CreateBuy(buyPrice, volume));
            }

            await ValidateQuoteTimeoutAsync(limitOrders, quote);

            ValidateMinVolume(limitOrders, (decimal) assetPair.MinVolume);

            await ValidatePnLThresholdAsync(limitOrders, instrument);

            await ValidateInventoryThresholdAsync(limitOrders, instrument);

            await ValidateBalanceAsync(limitOrders, assetPair);

            await ValidateMarketMakerStateAsync(limitOrders);

            await _orderBookService.UpdateAsync(new OrderBook
            {
                AssetPairId = instrument.AssetPairId,
                Time = DateTime.UtcNow,
                LimitOrders = limitOrders
            });

            if (instrument.Mode != InstrumentMode.Active)
                SetError(limitOrders, LimitOrderError.Idle);

            await _lykkeExchangeService.ApplyAsync(
                instrument.AssetPairId,
                limitOrders.Where(o => o.Error == LimitOrderError.None).ToArray());
        }

        private async Task ValidateQuoteTimeoutAsync(IReadOnlyCollection<LimitOrder> limitOrders, Quote quote)
        {
            QuoteTimeoutSettings quoteTimeoutSettings = await _quoteTimeoutSettingsService.GetAsync();

            if (quoteTimeoutSettings.Enabled && DateTime.UtcNow - quote.Time > quoteTimeoutSettings.Error)
            {
                _log.WarningWithDetails("Quote timeout is expired", new
                {
                    quote,
                    Timeout = quoteTimeoutSettings.Error
                });

                SetError(limitOrders, LimitOrderError.InvalidQuote);
            }
        }

        private async Task ValidatePnLThresholdAsync(IReadOnlyCollection<LimitOrder> limitOrders,
            Instrument instrument)
        {
            if (instrument.PnLThreshold == 0 || limitOrders.All(o => o.Error != LimitOrderError.None))
                return;

            IReadOnlyCollection<SummaryReport> summaryReports = await _summaryReportService.GetAllAsync();

            SummaryReport summaryReport = summaryReports.SingleOrDefault(o => o.AssetPairId == instrument.AssetPairId);

            if (summaryReport != null && summaryReport.PnL < 0 && instrument.PnLThreshold < Math.Abs(summaryReport.PnL))
                SetError(limitOrders, LimitOrderError.LowPnL);
        }

        private async Task ValidateInventoryThresholdAsync(IReadOnlyCollection<LimitOrder> limitOrders,
            Instrument instrument)
        {
            if (instrument.InventoryThreshold == 0 || limitOrders.All(o => o.Error != LimitOrderError.None))
                return;

            IReadOnlyCollection<Position> positions =
                await _positionService.GetOpenByAssetPairIdAsync(instrument.AssetPairId);

            decimal volume = positions.Sum(o => Math.Abs(o.Volume));

            if (instrument.InventoryThreshold <= volume)
                SetError(limitOrders, LimitOrderError.InventoryTooMuch);
        }

        private void ValidateMinVolume(IReadOnlyCollection<LimitOrder> limitOrders, decimal minVolume)
        {
            foreach (LimitOrder limitOrder in limitOrders.Where(o => o.Error == LimitOrderError.None))
            {
                if (limitOrder.Volume < minVolume)
                    limitOrder.Error = LimitOrderError.TooSmallVolume;
            }
        }

        private async Task ValidateBalanceAsync(IReadOnlyCollection<LimitOrder> limitOrders, AssetPair assetPair)
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
                decimal balance = (await _balanceService.GetByAssetIdAsync(ExchangeNames.Lykke, baseAsset.Id)).Amount;

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
                decimal balance = (await _balanceService.GetByAssetIdAsync(ExchangeNames.Lykke, quoteAsset.Id)).Amount;

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

        private async Task ValidateMarketMakerStateAsync(IReadOnlyCollection<LimitOrder> limitOrders)
        {
            MarketMakerState state = await _marketMakerStateService.GetStateAsync();

            if (state.Status == MarketMakerStatus.Error)
                SetError(limitOrders, LimitOrderError.MarketMakerError);

            if (state.Status == MarketMakerStatus.Disabled)
                SetError(limitOrders, LimitOrderError.Idle);
        }

        private void SetError(IReadOnlyCollection<LimitOrder> limitOrders, LimitOrderError limitOrderError)
        {
            foreach (LimitOrder limitOrder in limitOrders.Where(o => o.Error == LimitOrderError.None))
                limitOrder.Error = limitOrderError;
        }
    }
}
