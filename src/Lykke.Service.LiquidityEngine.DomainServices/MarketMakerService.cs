﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
using Lykke.Service.LiquidityEngine.Domain.Publishers;
using Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Metrics;

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
        private readonly IB2C2OrderBookService _b2C2OrderBookService;
        private readonly IQuoteTimeoutSettingsService _quoteTimeoutSettingsService;
        private readonly ISummaryReportService _summaryReportService;
        private readonly IPositionService _positionService;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly IMarketMakerSettingsService _marketMakerSettingsService;
        private readonly ITradeService _tradeService;
        private readonly IAssetPairLinkService _assetPairLinkService;
        private readonly IPnLStopLossEngineService _pnLStopLossEngineService;
        private readonly IFiatEquityStopLossService _fiatEquityStopLossService;
        private readonly INoFreshQuotesStopLossService _noFreshQuotesStopLossService;
        private readonly IOrderBooksUpdatesReportPublisher _orderBooksUpdatesReportPublisher;
        private readonly bool _isOrderBooksUpdateReportEnabled;
        private readonly ILog _log;
        private readonly object _syncInstruments = new object();
        private readonly Dictionary<string, Instrument> _instrumentsToUpdate = new Dictionary<string, Instrument>();
        private readonly object _syncManualResetEvent = new object();
        private readonly ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim(false, 1);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public MarketMakerService(
            IInstrumentService instrumentService,
            ILykkeExchangeService lykkeExchangeService,
            IOrderBookService orderBookService,
            IBalanceService balanceService,
            IMarketMakerStateService marketMakerStateService,
            IQuoteService quoteService,
            IB2C2OrderBookService b2C2OrderBookService,
            IQuoteTimeoutSettingsService quoteTimeoutSettingsService,
            ISummaryReportService summaryReportService,
            IPositionService positionService,
            IAssetsServiceWithCache assetsServiceWithCache,
            IMarketMakerSettingsService marketMakerSettingsService,
            ITradeService tradeService,
            IAssetPairLinkService assetPairLinkService,
            IPnLStopLossEngineService pnLStopLossEngineService,
            IFiatEquityStopLossService fiatEquityStopLossService,
            INoFreshQuotesStopLossService noFreshQuotesStopLossService,
            IOrderBooksUpdatesReportPublisher orderBooksUpdatesReportPublisher,
            bool isOrderBooksUpdateReportEnabled,
            ILogFactory logFactory)
        {
            _instrumentService = instrumentService;
            _lykkeExchangeService = lykkeExchangeService;
            _orderBookService = orderBookService;
            _balanceService = balanceService;
            _marketMakerStateService = marketMakerStateService;
            _quoteService = quoteService;
            _b2C2OrderBookService = b2C2OrderBookService;
            _quoteTimeoutSettingsService = quoteTimeoutSettingsService;
            _summaryReportService = summaryReportService;
            _positionService = positionService;
            _assetsServiceWithCache = assetsServiceWithCache;
            _marketMakerSettingsService = marketMakerSettingsService;
            _tradeService = tradeService;
            _assetPairLinkService = assetPairLinkService;
            _pnLStopLossEngineService = pnLStopLossEngineService;
            _fiatEquityStopLossService = fiatEquityStopLossService;
            _noFreshQuotesStopLossService = noFreshQuotesStopLossService;
            _orderBooksUpdatesReportPublisher = orderBooksUpdatesReportPublisher;
            _isOrderBooksUpdateReportEnabled = isOrderBooksUpdateReportEnabled;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            try
            {
                Task.Run(TryUpdateOrderBooksAsync);
            }
            catch (Exception ex)
            {
                _log.Warning("Something went wrong in MarketMakerService.Start().", ex);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task TryUpdateOrderBooksAsync()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        _manualResetEvent.Wait(_cancellationTokenSource.Token);

                        lock (_syncManualResetEvent)
                        {
                            _manualResetEvent.Reset();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    var startedAt = DateTime.UtcNow;

                    List<Instrument> instrumentsToUpdate;

                    lock (_syncInstruments)
                    {
                        instrumentsToUpdate = _instrumentsToUpdate.Values.ToList();

                        _instrumentsToUpdate.Clear();
                    }

                    var iterationDateTime = DateTime.UtcNow;

                    foreach (Instrument instrument in instrumentsToUpdate)
                        await ProcessInstrumentAsync(instrument, iterationDateTime);

                    var finishedAt = DateTime.UtcNow;

                    //if (instrumentsToUpdate.Count > 0)
                    //    _log.Info("MarketMakerService.TryUpdateOrderBooksAsync() completed.", new
                    //    {
                    //        IstrumentsCount = instrumentsToUpdate.Count,
                    //        StartedAt = startedAt,
                    //        FinishedAt = finishedAt,
                    //        Latency = (finishedAt - startedAt).TotalMilliseconds
                    //    });

                    PrometheusMetrics.MarketMakingLatency.Inc((finishedAt - startedAt).TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    _log.Error("Something went wrong in MarketMakerService.TryUpdateOrderBooksAsync().", ex);
                }
            }
        }

        public async Task UpdateOrderBooksAsync(string assetPairId = null)
        {
            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            IReadOnlyCollection<Instrument> activeInstruments = instruments
                .Where(o => o.Mode == InstrumentMode.Idle || o.Mode == InstrumentMode.Active)
                .ToArray();

            lock (_syncInstruments)
            {
                if (!string.IsNullOrWhiteSpace(assetPairId))
                {
                    var instrument = instruments.FirstOrDefault(x => x.AssetPairId == assetPairId);

                    if (instrument != null)
                        _instrumentsToUpdate[assetPairId] = instrument;
                }
                else
                {
                    foreach (var instrument in activeInstruments)
                        _instrumentsToUpdate[instrument.AssetPairId] = instrument;
                }
            }

            lock (_syncManualResetEvent)
            {
                _manualResetEvent.Set();
            }
        }

        private async Task ProcessInstrumentAsync(Instrument instrument, DateTime iterationDateTime)
        {
            try
            {
                var startedAt = DateTime.UtcNow;

                OrderBook directOrderBook = await CalculateDirectOrderBookAsync(instrument, iterationDateTime);

                if (directOrderBook == null)
                    return;

                var orderBooks = new List<OrderBook>
                {
                    directOrderBook
                };

                if (instrument.CrossInstruments != null)
                {
                    foreach (CrossInstrument crossInstrument in instrument.CrossInstruments)
                    {
                        OrderBook crossOrderBook = await CalculateCrossOrderBookAsync(directOrderBook, crossInstrument);

                        if (crossOrderBook == null)
                            continue;

                        orderBooks.Add(crossOrderBook);
                    }
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

                var finishedAt = DateTime.UtcNow;

                PrometheusMetrics.MarketMakingAssetPairLatency.Inc((finishedAt - startedAt).TotalMilliseconds);
            }
            catch (Exception exception)
            {
                _log.WarningWithDetails("An error occured while processing instrument", exception, instrument);
            }
        }

        private async Task<OrderBook> CalculateDirectOrderBookAsync(Instrument instrument, DateTime iterationDateTime)
        {
            Quote[] quotes = _b2C2OrderBookService.GetQuotes(instrument.AssetPairId);

            if (quotes == null || quotes.Length != 2)
            {
                _log.WarningWithDetails("No quotes for instrument", instrument.AssetPairId);
                return null;
            }

            Balance baseAssetBalance = null;
            Balance quoteAssetBalance = null;
            TimeSpan timeSinceLastTrade = TimeSpan.Zero;

            if (instrument.AllowSmartMarkup)
            {
                AssetPairLink assetPairLink =
                    await _assetPairLinkService.GetByInternalAssetPairIdAsync(instrument.AssetPairId);

                if (assetPairLink != null && !assetPairLink.IsEmpty())
                {
                    baseAssetBalance =
                        await _balanceService.GetByAssetIdAsync(ExchangeNames.B2C2, assetPairLink.ExternalBaseAssetId);
                    quoteAssetBalance =
                        await _balanceService.GetByAssetIdAsync(ExchangeNames.B2C2, assetPairLink.ExternalQuoteAssetId);
                    timeSinceLastTrade =
                        DateTime.UtcNow - _tradeService.GetLastInternalTradeTime(instrument.AssetPairId);
                }
                else
                {
                    _log.WarningWithDetails("The asset pair link does not configured", new {instrument.AssetPairId});
                }
            }

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            Asset baseAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.BaseAssetId);

            MarketMakerSettings marketMakerSettings = await _marketMakerSettingsService.GetAsync();

            decimal globalMarkup = marketMakerSettings.LimitOrderPriceMarkup;

            decimal noQuotesMarkup = await _noFreshQuotesStopLossService.GetNoFreshQuotesMarkup(assetPair.Id);

            decimal pnLStopLossMarkup = await _pnLStopLossEngineService.GetTotalMarkupByAssetPairIdAsync(assetPair.Id);

            decimal fiatEquityStopLossMarkup = await _fiatEquityStopLossService.GetFiatEquityMarkup(assetPair.Id);

            //_log.InfoWithDetails("Arguments for Calculator.CalculateLimitOrders(...).", new
            //{
            //    instrument.AssetPairId,
            //    quotes,
            //    levels = instrument.Levels.ToArray(),
            //    baseAmountBalance = baseAssetBalance?.Amount ?? 0,
            //    quoteAmountBalance = quoteAssetBalance?.Amount ?? 0,
            //    timeSinceLastTradeTotalSeconds = (int)timeSinceLastTrade.TotalSeconds,
            //    instrumentHalfLifePeriod = instrument.HalfLifePeriod,
            //    instrumentAllowSmartMarkup = instrument.AllowSmartMarkup,
            //    marketMakerSettingsLimitOrderPriceMarkup = globalMarkup,
            //    pnLStopLossMarkup,
            //    fiatEquityStopLossMarkup,
            //    noQuotesMarkup,
            //    assetPairAccuracy = assetPair.Accuracy,
            //    baseAssetAccuracy = baseAsset.Accuracy,
            //    instrument
            //});

            OrderBookUpdateReport orderBookUpdateReport = null;
            if (_isOrderBooksUpdateReportEnabled)
            {
                orderBookUpdateReport = new OrderBookUpdateReport(iterationDateTime);
                orderBookUpdateReport.AssetPair = instrument.AssetPairId;
                orderBookUpdateReport.FirstQuoteAsk = quotes[0].Ask;
                orderBookUpdateReport.FirstQuoteBid = quotes[0].Bid;
                orderBookUpdateReport.SecondQuoteAsk = quotes[1].Ask;
                orderBookUpdateReport.SecondQuoteBid = quotes[1].Bid;
                orderBookUpdateReport.QuoteDateTime = quotes[0].Time;
                orderBookUpdateReport.GlobalMarkup = globalMarkup;
                orderBookUpdateReport.NoFreshQuoteMarkup = noQuotesMarkup;
                orderBookUpdateReport.PnLStopLossMarkup = pnLStopLossMarkup;
                orderBookUpdateReport.FiatEquityMarkup = fiatEquityStopLossMarkup;
            }

            IReadOnlyCollection<LimitOrder> limitOrders = Calculator.CalculateLimitOrders(
                quotes[0],
                quotes[1],
                instrument.Levels.ToArray(),
                baseAssetBalance?.Amount ?? 0,
                quoteAssetBalance?.Amount ?? 0,
                (int) timeSinceLastTrade.TotalSeconds,
                instrument.HalfLifePeriod,
                instrument.AllowSmartMarkup,
                globalMarkup,
                pnLStopLossMarkup,
                fiatEquityStopLossMarkup,
                noQuotesMarkup,
                assetPair.Accuracy,
                baseAsset.Accuracy,
                orderBookUpdateReport);

            await ValidateQuoteTimeoutAsync(limitOrders, quotes[0]);

            await ValidateQuoteTimeoutAsync(limitOrders, quotes[1]);

            ValidateMinVolume(limitOrders, (decimal) assetPair.MinVolume);

            await ValidatePriceAsync(limitOrders);

            await ValidateBalanceAsync(limitOrders, assetPair);

            WriteInfoLog(instrument.AssetPairId, quotes, timeSinceLastTrade, limitOrders,
                "Direct limit orders calculated");

            if (orderBookUpdateReport != null)
                await _orderBooksUpdatesReportPublisher.PublishAsync(orderBookUpdateReport);

            return new OrderBook
            {
                AssetPairId = instrument.AssetPairId,
                Time = DateTime.UtcNow,
                LimitOrders = limitOrders,
                IsDirect = true
            };
        }

        private async Task<OrderBook> CalculateCrossOrderBookAsync(OrderBook directOrderBook,
            CrossInstrument crossInstrument)
        {
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

            LimitOrder[] validDirectLimitOrders = directOrderBook.LimitOrders
                .Where(o => o.Error == LimitOrderError.None)
                .ToArray();

            if (validDirectLimitOrders.Length == 0)
                return null;

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(crossInstrument.AssetPairId);

            Asset baseAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.BaseAssetId);

            IReadOnlyCollection<LimitOrder> crossLimitOrders = Calculator.CalculateCrossLimitOrders(quote,
                validDirectLimitOrders, crossInstrument.IsInverse, assetPair.Accuracy, baseAsset.Accuracy);

            await ValidateQuoteTimeoutAsync(crossLimitOrders, quote);

            ValidateMinVolume(crossLimitOrders, (decimal) assetPair.MinVolume);

            await ValidateBalanceAsync(crossLimitOrders, assetPair);

            WriteInfoLog(crossInstrument.AssetPairId, directOrderBook.AssetPairId, quote, crossLimitOrders,
                "Cross limit orders calculated");

            return new OrderBook
            {
                AssetPairId = crossInstrument.AssetPairId,
                Time = DateTime.UtcNow,
                LimitOrders = crossLimitOrders,
                IsDirect = false,
                BaseAssetPairId = directOrderBook.AssetPairId,
                CrossQuote = quote
            };
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

        private async Task ValidatePriceAsync(IReadOnlyCollection<LimitOrder> limitOrders)
        {
            MarketMakerSettings marketMakerSettings = await _marketMakerSettingsService.GetAsync();

            if (marketMakerSettings.LimitOrderPriceMaxDeviation == 0)
                return;

            LimitOrder[] sellLimitOrders = limitOrders
                .Where(o => o.Type == LimitOrderType.Sell)
                .OrderBy(o => o.Price)
                .ToArray();

            LimitOrder[] buyLimitOrders = limitOrders
                .Where(o => o.Type == LimitOrderType.Buy)
                .OrderByDescending(o => o.Price)
                .ToArray();

            if (sellLimitOrders.Any())
            {
                decimal maxSellPrice = sellLimitOrders[0].Price * (1 + marketMakerSettings.LimitOrderPriceMaxDeviation);

                foreach (LimitOrder limitOrder in sellLimitOrders.Where(o => o.Error == LimitOrderError.None))
                {
                    if (limitOrder.Price > maxSellPrice)
                        limitOrder.Error = LimitOrderError.PriceIsOutOfTheRange;
                }
            }

            if (buyLimitOrders.Any())
            {
                decimal minBuyPrice = buyLimitOrders[0].Price * (1 - marketMakerSettings.LimitOrderPriceMaxDeviation);

                foreach (LimitOrder limitOrder in buyLimitOrders.Where(o => o.Error == LimitOrderError.None))
                {
                    if (limitOrder.Price < minBuyPrice)
                        limitOrder.Error = LimitOrderError.PriceIsOutOfTheRange;
                }
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
                        decimal volume = balance.TruncateDecimalPlaces(baseAsset.Accuracy);

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
                        decimal volume = (balance / limitOrder.Price).TruncateDecimalPlaces(baseAsset.Accuracy);

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

            IReadOnlyCollection<SummaryReport> summaryReports = (await _summaryReportService.GetAllAsync())
                .Where(o => o.AssetPairId == instrument.AssetPairId)
                .ToArray();

            decimal totalPnL = summaryReports.Select(o => o.PnL).DefaultIfEmpty(0).Sum();

            if (summaryReports.Count > 0 && totalPnL < 0 && instrument.PnLThreshold < Math.Abs(totalPnL))
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

        private void WriteInfoLog(string assetPair, Quote[] quotes, TimeSpan timeSinceLastTrade,
            IEnumerable<LimitOrder> limitOrders, string message,
            [CallerMemberName] string process = nameof(WriteInfoLog))
        {
            _log.InfoWithDetails(message, new
            {
                AssetPair = assetPair,
                TimeSinceLastTrade = timeSinceLastTrade,
                Quotes = quotes.Select(quote => new
                {
                    quote.Ask,
                    quote.Bid
                }),
                LimitOrders = limitOrders.Select(o => new
                {
                    Type = o.Type.ToString(),
                    o.Price,
                    o.Volume
                })
            }, "data", process);
        }

        private void WriteInfoLog(string assetPair, string directAssetPair, Quote quote,
            IEnumerable<LimitOrder> limitOrders, string message,
            [CallerMemberName] string process = nameof(WriteInfoLog))
        {
            _log.InfoWithDetails(message, new
            {
                AssetPair = assetPair,
                DirectAssetPair = directAssetPair,
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
