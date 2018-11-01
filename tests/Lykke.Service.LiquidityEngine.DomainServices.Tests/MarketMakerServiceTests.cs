using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.LiquidityEngine.DomainServices.Tests
{
    [TestClass]
    public class MarketMakerServiceTests
    {
        private readonly Mock<IInstrumentService> _instrumentServiceMock =
            new Mock<IInstrumentService>();

        private readonly Mock<ILykkeExchangeService> _lykkeExchangeServiceMock =
            new Mock<ILykkeExchangeService>();

        private readonly Mock<IOrderBookService> _orderBookServiceMock =
            new Mock<IOrderBookService>();

        private readonly Mock<IBalanceService> _balanceServiceMock =
            new Mock<IBalanceService>();

        private readonly Mock<IMarketMakerStateService> _marketMakerStateServiceMock =
            new Mock<IMarketMakerStateService>();

        private readonly Mock<IQuoteService> _quoteServiceMock =
            new Mock<IQuoteService>();

        private readonly Mock<IB2C2OrderBookService> _b2C2OrderBookServiceMock =
            new Mock<IB2C2OrderBookService>();

        private readonly Mock<IQuoteTimeoutSettingsService> _quoteTimeoutSettingsServiceMock =
            new Mock<IQuoteTimeoutSettingsService>();

        private readonly Mock<ISummaryReportService> _summaryReportServiceMock =
            new Mock<ISummaryReportService>();

        private readonly Mock<IPositionService> _positionServiceMock =
            new Mock<IPositionService>();

        private readonly Mock<IAssetsServiceWithCache> _assetsServiceWithCacheMock =
            new Mock<IAssetsServiceWithCache>();

        private readonly Mock<IMarketMakerSettingsService> _marketMakerSettingsServiceMock =
            new Mock<IMarketMakerSettingsService>();

        private readonly Mock<ITradeService> _tradeServiceMock = new Mock<ITradeService>();
        
        private readonly Mock<IAssetPairLinkService> _assetPairLinkServiceMock = new Mock<IAssetPairLinkService>();
        
        private readonly List<Instrument> _instruments = new List<Instrument>();

        private readonly List<Balance> _balances = new List<Balance>();

        private readonly List<Asset> _assets = new List<Asset>();

        private readonly List<AssetPair> _assetPairs = new List<AssetPair>();

        private QuoteTimeoutSettings _quoteTimeoutSettings;

        private MarketMakerService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _assets.AddRange(new[]
            {
                new Asset
                {
                    Id = "USD",
                    Accuracy = 2
                },
                new Asset
                {
                    Id = "BTC",
                    Accuracy = 8
                }
            });

            _assetPairs.Add(new AssetPair
            {
                Id = "BTCUSD",
                BaseAssetId = "BTC",
                QuotingAssetId = "USD",
                Accuracy = 3,
                InvertedAccuracy = 8,
                MinVolume = 0.0001,
                MinInvertedVolume = 1
            });

            _balances.AddRange(new[]
            {
                new Balance(ExchangeNames.Lykke, "BTC", 1000000),
                new Balance(ExchangeNames.Lykke, "USD", 1000000)
            });

            _quoteTimeoutSettings = new QuoteTimeoutSettings
            {
                Enabled = true,
                Error = TimeSpan.FromSeconds(1)
            };

            _balanceServiceMock.Setup(o => o.GetByAssetIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string exchange, string assetId) =>
                {
                    return Task.FromResult(_balances.Single(o => o.Exchange == exchange && o.AssetId == assetId));
                });

            _assetsServiceWithCacheMock
                .Setup(o => o.TryGetAssetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string assetId, CancellationToken token) =>
                {
                    return Task.FromResult(_assets.Single(o => o.Id == assetId));
                });

            _assetsServiceWithCacheMock
                .Setup(o => o.TryGetAssetPairAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string assetPairId, CancellationToken token) =>
                {
                    return Task.FromResult(_assetPairs.Single(o => o.Id == assetPairId));
                });

            _instrumentServiceMock.Setup(o => o.GetAllAsync())
                .Returns(() => Task.FromResult<IReadOnlyCollection<Instrument>>(_instruments));

            _quoteTimeoutSettingsServiceMock.Setup(o => o.GetAsync())
                .Returns(() => Task.FromResult(_quoteTimeoutSettings));

            _service = new MarketMakerService(
                _instrumentServiceMock.Object,
                _lykkeExchangeServiceMock.Object,
                _orderBookServiceMock.Object,
                _balanceServiceMock.Object,
                _marketMakerStateServiceMock.Object,
                _quoteServiceMock.Object,
                _b2C2OrderBookServiceMock.Object,
                _quoteTimeoutSettingsServiceMock.Object,
                _summaryReportServiceMock.Object,
                _positionServiceMock.Object,
                _assetsServiceWithCacheMock.Object,
                _marketMakerSettingsServiceMock.Object,
                _tradeServiceMock.Object,
                _assetPairLinkServiceMock.Object,
                EmptyLogFactory.Instance);
        }

        //[TestMethod]
        public async Task Calculate_Limit_Orders()
        {
            // arrange

            IReadOnlyCollection<LimitOrder> actualLimitOrders = null;

            var expectedLimitOrders = new[]
            {
                LimitOrder.CreateSell(6600.35m, 10),
                LimitOrder.CreateSell(6522.917m, 6),
                LimitOrder.CreateSell(6464, 1),
                LimitOrder.CreateBuy(6237, 1),
                LimitOrder.CreateBuy(6179.25m, 6),
                LimitOrder.CreateBuy(5935.05m, 10)
            };

            _instruments.Add(new Instrument
            {
                AssetPairId = "BTCUSD",
                Mode = InstrumentMode.Active,
                Levels = new[]
                {
                    new InstrumentLevel {Number = 1, Volume = 1, Markup = .01m},
                    new InstrumentLevel {Number = 2, Volume = 6, Markup = .03m},
                    new InstrumentLevel {Number = 3, Volume = 10, Markup = .05m}
                }
            });

            _lykkeExchangeServiceMock
                .Setup(o => o.ApplyAsync(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<LimitOrder>>()))
                .Returns((string assetPairId, IReadOnlyCollection<LimitOrder> limitOrders) => Task.CompletedTask)
                .Callback((string assetPairId, IReadOnlyCollection<LimitOrder> limitOrders) =>
                    actualLimitOrders = limitOrders);

            _orderBookServiceMock.Setup(o => o.UpdateAsync(It.IsAny<OrderBook>()))
                .Returns((OrderBook orderBook) => Task.CompletedTask);

            // act

            await _service.UpdateOrderBooksAsync();

            // assert

            Assert.IsTrue(AreEqual(expectedLimitOrders, actualLimitOrders));
        }

        //https://lykkex.atlassian.net/browse/LIQ-745
        //[TestMethod]
        public async Task InventoryExceeded_OrdersAreNotPlaced()
        {
            // arrange

            IReadOnlyCollection<LimitOrder> actualLimitOrders = null;

            _instruments.Add(new Instrument
            {
                AssetPairId = "BTCUSD",
                Mode = InstrumentMode.Active,
                InventoryThreshold = 1,
                Levels = new[]
                {
                    new InstrumentLevel {Number = 1, Volume = 1, Markup = .01m},
                    new InstrumentLevel {Number = 2, Volume = 1, Markup = .02m}
                },
                CrossInstruments = new List<CrossInstrument>()
            });

            _marketMakerStateServiceMock
                .Setup(o => o.GetStateAsync())
                .Returns(() =>
                    Task.FromResult(new MarketMakerState {Status = MarketMakerStatus.Active}));

            _lykkeExchangeServiceMock
                .Setup(o => o.ApplyAsync(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<LimitOrder>>()))
                .Returns((string assetPairId, IReadOnlyCollection<LimitOrder> limitOrders) => Task.CompletedTask)
                .Callback((string assetPairId, IReadOnlyCollection<LimitOrder> limitOrders) =>
                    actualLimitOrders = limitOrders);

            _orderBookServiceMock.Setup(o => o.UpdateAsync(It.IsAny<OrderBook>()))
                .Returns((OrderBook orderBook) => Task.CompletedTask);

            _quoteServiceMock
                .Setup(o => o.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string source, string assetPairId) =>
                    Task.FromResult(new Quote(assetPairId, DateTime.UtcNow, 6001, 6000, "b2c2")));

            // Make sure inventory is exceeded
            _positionServiceMock.Setup(o => o.GetOpenByAssetPairIdAsync(It.IsAny<string>()))
                .Returns((string assetPairId) => Task.FromResult<IReadOnlyCollection<Position>>(new[]
                {
                    new Position {AssetPairId = assetPairId, Volume = 10}
                }));

            // act

            await _service.UpdateOrderBooksAsync();

            // assert

            Assert.IsFalse(actualLimitOrders.Any());
        }

        private bool AreEqual(IReadOnlyCollection<LimitOrder> a, IReadOnlyCollection<LimitOrder> b)
        {
            if (a.Count != b.Count)
                return false;

            return a.All(o => b.Any(p => p.Type == o.Type && p.Volume == o.Volume && p.Price == o.Price));
        }
    }
}
