using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Service.LiquidityEngine.Domain;
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

        private readonly Mock<IExternalExchangeService> _externalExchangeServiceMock =
            new Mock<IExternalExchangeService>();

        private readonly Mock<ILykkeExchangeService> _lykkeExchangeServiceMock =
            new Mock<ILykkeExchangeService>();

        private readonly Mock<IOrderBookService> _orderBookServiceMock =
            new Mock<IOrderBookService>();

        private readonly List<Instrument> _instruments = new List<Instrument>();

        private MarketMakerService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _instrumentServiceMock.Setup(o => o.GetAllAsync())
                .Returns(() => Task.FromResult<IReadOnlyCollection<Instrument>>(_instruments));
            
            _service = new MarketMakerService(
                _instrumentServiceMock.Object,
                _externalExchangeServiceMock.Object,
                _lykkeExchangeServiceMock.Object,
                _orderBookServiceMock.Object,
                EmptyLogFactory.Instance);
        }

        [TestMethod]
        public async Task Calculate_Limit_Orders()
        {
            // arrange

            IReadOnlyCollection<LimitOrder> actualLimitOrders = null;

            var expectedLimitOrders = new []
            {
                LimitOrder.CreateSell(6600.35m, 10),
                LimitOrder.CreateSell(6522.917m, 6),
                LimitOrder.CreateSell(6464, 1),
                LimitOrder.CreateBuy(6237, 1),
                LimitOrder.CreateBuy(6179.25m, 6),
                LimitOrder.CreateBuy(5935.05m, 10)
            };
            
            var sellPrices = new Dictionary<decimal, decimal>
            {
                {1, 6400},
                {7, 6450},
                {17, 6500}
            };

            var buyPrices = new Dictionary<decimal, decimal>
            {
                {1, 6300},
                {7, 6250},
                {17, 6100}
            };

            _instruments.Add(new Instrument
            {
                AssetPairId = "BTCUSD",
                Mode = InstrumentMode.Active,
                Markup = 0.01m,
                Levels = new[]
                {
                    new LevelVolume {Number = 1, Volume = 1},
                    new LevelVolume {Number = 2, Volume = 6},
                    new LevelVolume {Number = 3, Volume = 10}
                }
            });

            _externalExchangeServiceMock.Setup(o => o.GetSellPriceAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns((string assetPairId, decimal volume) => Task.FromResult(sellPrices[volume]));

            _externalExchangeServiceMock.Setup(o => o.GetBuyPriceAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns((string assetPairId, decimal volume) => Task.FromResult(buyPrices[volume]));

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

        private bool AreEqual(IReadOnlyCollection<LimitOrder> a, IReadOnlyCollection<LimitOrder> b)
        {
            if (a.Count != b.Count)
                return false;

            return a.All(o => b.Any(p =>
                p.Type == o.Type && p.Volume == o.Volume && Math.Round(p.Price, 3) == Math.Round(o.Price, 3)));
        }
    }
}
