using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            IReadOnlyCollection<LimitOrder> limitOrders = await CalculateDirectLimitOrders(instrument);

            if (limitOrders == null)
                return;

            var orderBooks = new List<OrderBook>
            {
                new OrderBook
                {
                    AssetPairId = instrument.AssetPairId,
                    Time = DateTime.UtcNow,
                    LimitOrders = limitOrders
                }
            };

            foreach (CrossInstrument crossInstrument in instrument.CrossInstruments)
            {
                IReadOnlyCollection<LimitOrder> crossLimitOrders =
                    await CalculateCrossLimitOrders(limitOrders, crossInstrument);

                if (crossLimitOrders == null)
                    continue;

                orderBooks.Add(new OrderBook
                {
                    AssetPairId = crossInstrument.AssetPairId,
                    Time = DateTime.UtcNow,
                    LimitOrders = crossLimitOrders
                });
            }

            await ValidatePnLThresholdAsync(orderBooks, instrument);

            await ValidateInventoryThresholdAsync(orderBooks, instrument);

            await ValidateMarketMakerStateAsync(orderBooks);

            ValidateInstrumentMode(orderBooks, instrument.Mode);

            foreach (OrderBook orderBook in orderBooks)
                await _orderBookService.UpdateAsync(orderBook);

            foreach (OrderBook orderBook in orderBooks)
            {
                LimitOrder[] allowedLimitOrders = orderBook.LimitOrders
                    .Where(o => o.Error == LimitOrderError.None)
                    .ToArray();

                await _lykkeExchangeService.ApplyAsync(orderBook.AssetPairId, allowedLimitOrders);
            }
        }

        private async Task<IReadOnlyCollection<LimitOrder>> CalculateDirectLimitOrders(Instrument instrument)
        {
            Quote quote = await _quoteService.GetAsync(ExchangeNames.B2C2, instrument.AssetPairId);

            if (quote == null)
            {
                _log.WarningWithDetails("No quote for instrument", instrument.AssetPairId);
                return null;
            }

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            Asset baseAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.BaseAssetId);

            IReadOnlyCollection<LimitOrder> limitOrders =
                Calculator.CalculateLimitOrders(quote, instrument.Levels, assetPair.Accuracy, baseAsset.Accuracy);

            await ValidateQuoteTimeoutAsync(limitOrders, quote);

            ValidateMinVolume(limitOrders, (decimal) assetPair.MinVolume);

            await ValidateBalanceAsync(limitOrders, assetPair);

            WriteInfoLog(instrument.AssetPairId, quote, limitOrders, "Direct limit orders calculated");

            return limitOrders;
        }

        private async Task<IReadOnlyCollection<LimitOrder>> CalculateCrossLimitOrders(
            IReadOnlyCollection<LimitOrder> directLimitOrders, CrossInstrument crossInstrument)
        {
            Quote quote = await _quoteService
                .GetAsync(crossInstrument.QuoteSource, crossInstrument.ExternalAssetPairId);

            if (quote == null)
            {
                _log.WarningWithDetails("No quote for instrument", new
                {
                    Source = crossInstrument.QuoteSource,
                    AssetPair = crossInstrument.AssetPairId
                });
                return null;
            }

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(crossInstrument.AssetPairId);

            Asset baseAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.BaseAssetId);

            IReadOnlyCollection<LimitOrder> crossLimitOrders =
                Calculator.CalculateCrossLimitOrders(quote, directLimitOrders, crossInstrument.IsInverse,
                    assetPair.Accuracy, baseAsset.Accuracy);

            await ValidateQuoteTimeoutAsync(crossLimitOrders, quote);

            ValidateMinVolume(crossLimitOrders, (decimal) assetPair.MinVolume);

            await ValidateBalanceAsync(crossLimitOrders, assetPair);

            WriteInfoLog(crossInstrument.AssetPairId, quote, crossLimitOrders, "Cross limit orders calculated");

            return crossLimitOrders;
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

        private async Task ValidatePnLThresholdAsync(IEnumerable<OrderBook> orderBooks, Instrument instrument)
        {
            if (instrument.PnLThreshold == 0)
                return;

            IReadOnlyCollection<SummaryReport> summaryReports = await _summaryReportService.GetAllAsync();

            SummaryReport summaryReport = summaryReports.SingleOrDefault(o => o.AssetPairId == instrument.AssetPairId);

            if (summaryReport != null && summaryReport.PnL < 0 && instrument.PnLThreshold < Math.Abs(summaryReport.PnL))
                SetError(orderBooks.SelectMany(o => o.LimitOrders), LimitOrderError.LowPnL);
        }

        private async Task ValidateInventoryThresholdAsync(IEnumerable<OrderBook> orderBooks, Instrument instrument)
        {
            if (instrument.InventoryThreshold == 0)
                return;

            IReadOnlyCollection<Position> positions =
                await _positionService.GetOpenByAssetPairIdAsync(instrument.AssetPairId);

            decimal volume = positions.Sum(o => Math.Abs(o.Volume));

            if (instrument.InventoryThreshold <= volume)
                SetError(orderBooks.SelectMany(o => o.LimitOrders), LimitOrderError.InventoryTooMuch);
        }

        private async Task ValidateMarketMakerStateAsync(IEnumerable<OrderBook> orderBooks)
        {
            MarketMakerState state = await _marketMakerStateService.GetStateAsync();

            if (state.Status == MarketMakerStatus.Error)
                SetError(orderBooks.SelectMany(o => o.LimitOrders), LimitOrderError.MarketMakerError);

            else if (state.Status == MarketMakerStatus.Disabled)
                SetError(orderBooks.SelectMany(o => o.LimitOrders), LimitOrderError.Idle);
        }

        private void WriteInfoLog(string assetPair, Quote quote, IReadOnlyCollection<LimitOrder> limitOrders,
            string message, [CallerMemberName] string process = nameof(WriteInfoLog))
        {
            _log.InfoWithDetails(message, new
            {
                AssetPair = assetPair,
                Quote = new
                {
                    quote.Ask,
                    quote.Bid
                },
                LimitOrders = limitOrders.Select(o => new
                {
                    Type = o.Type.ToString(),
                    o.Price,
                    o.Volume
                })
            }, "data", process);
        }

        private static void ValidateInstrumentMode(IEnumerable<OrderBook> orderBooks, InstrumentMode instrumentMode)
        {
            if (instrumentMode != InstrumentMode.Active)
                SetError(orderBooks.SelectMany(o => o.LimitOrders), LimitOrderError.Idle);
        }

        private static void SetError(IEnumerable<LimitOrder> limitOrders, LimitOrderError limitOrderError)
        {
            foreach (LimitOrder limitOrder in limitOrders.Where(o => o.Error == LimitOrderError.None))
                limitOrder.Error = limitOrderError;
        }
    }
}
