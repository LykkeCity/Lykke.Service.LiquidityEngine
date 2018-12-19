using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Service.LiquidityEngine.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.LiquidityEngine.DomainServices.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void Create_Limit_Orders_By_Levels()
        {
            // arrange

            int priceAccuracy = 3;

            int volumeAccuracy = 8;

            var quote = new Quote("BTCUSD", DateTime.UtcNow, 6500, 6400, "none");

            var levels = new[]
            {
                new InstrumentLevel {Number = 1, Volume = 1, Markup = .1m},
                new InstrumentLevel {Number = 2, Volume = 2, Markup = .2m},
                new InstrumentLevel {Number = 3, Volume = 3, Markup = .3m}
            };

            var expectedLimitOrders = levels.SelectMany(o => new[]
                {
                    new LimitOrder
                    {
                        Type = LimitOrderType.Sell,
                        Volume = Math.Round(o.Volume, volumeAccuracy),
                        Price = (quote.Ask * (1 + o.Markup)).TruncateDecimalPlaces(priceAccuracy, true)
                    },
                    new LimitOrder
                    {
                        Type = LimitOrderType.Buy,
                        Volume = Math.Round(o.Volume, volumeAccuracy),
                        Price = (quote.Bid * (1 - o.Markup)).TruncateDecimalPlaces(priceAccuracy, true)
                    }
                })
                .ToArray();

            // act

            IReadOnlyCollection<LimitOrder> actualLimitOrders =
                Calculator.CalculateLimitOrders(quote, levels, priceAccuracy, volumeAccuracy);

            // assert

            Assert.IsTrue(AreEqual(expectedLimitOrders, actualLimitOrders));
        }

        [TestMethod]
        public void Create_Dynamically_Distributed_Limit_Orders_By_Levels()
        {
            // arrange

            int priceAccuracy = 3;
            int volumeAccuracy = 8;

            var quote1 = new Quote("BTCUSD", DateTime.UtcNow, 6000, 5950, "none");
            var quote2 = new Quote("BTCUSD", DateTime.UtcNow, 6050, 5900, "none");

            var levels = new[]
            {
                new InstrumentLevel {Number = 1, Volume = 0.01m, Markup = 0.01m},
                new InstrumentLevel {Number = 2, Volume = 0.02m, Markup = 0.02m},
                new InstrumentLevel {Number = 3, Volume = 0.03m, Markup = 0.03m},
                new InstrumentLevel {Number = 4, Volume = 0.04m, Markup = 0.04m},
                new InstrumentLevel {Number = 5, Volume = 0.05m, Markup = 0.05m}
            };

            var expectedLimitOrders = new List<LimitOrder>
            {
                LimitOrder.CreateSell(6390.000m, 0.05m),
                LimitOrder.CreateSell(6295.715m, 0.04m),
                LimitOrder.CreateSell(6209.429m, 0.03m),
                LimitOrder.CreateSell(6130.929m, 0.02m),
                LimitOrder.CreateSell(6060.000m, 0.01m),
                LimitOrder.CreateBuy(5890.500m, 0.01m),
                LimitOrder.CreateBuy(5820.499m, 0.02m),
                LimitOrder.CreateBuy(5743.785m, 0.03m),
                LimitOrder.CreateBuy(5660.571m, 0.04m),
                LimitOrder.CreateBuy(5571.071m, 0.05m)
            };

            // act

            IReadOnlyCollection<LimitOrder> actualLimitOrders =
                Calculator.CalculateLimitOrders(quote1, quote2, levels, 0, 0, 0, 0, false, 0, priceAccuracy,
                    volumeAccuracy);

            // assert

            Assert.IsTrue(AreEqual(expectedLimitOrders, actualLimitOrders));
        }
        
        [TestMethod]
        public void Create_Dynamically_Distributed_Limit_Orders_By_Levels_With_Buy_Smart_Markup()
        {
            // arrange

            int priceAccuracy = 3;
            int volumeAccuracy = 8;

            var quote1 = new Quote("BTCUSD", DateTime.UtcNow, 6000, 5950, "none");
            var quote2 = new Quote("BTCUSD", DateTime.UtcNow, 6050, 5900, "none");

            decimal baseAssetBalance = 1;
            decimal quoteAssetBalance = -46.5m;
            int timeSinceLastTrade = 1;
            int halfLifePeriod = 30;
                
            var levels = new[]
            {
                new InstrumentLevel {Number = 1, Volume = 0.01m, Markup = 0.01m},
                new InstrumentLevel {Number = 2, Volume = 0.02m, Markup = 0.02m},
                new InstrumentLevel {Number = 3, Volume = 0.03m, Markup = 0.03m},
                new InstrumentLevel {Number = 4, Volume = 0.04m, Markup = 0.04m},
                new InstrumentLevel {Number = 5, Volume = 0.05m, Markup = 0.05m}
            };

            var expectedLimitOrders = new List<LimitOrder>
            {
                LimitOrder.CreateSell(6390.000m, 0.05m),
                LimitOrder.CreateSell(6295.715m, 0.04m),
                LimitOrder.CreateSell(6209.429m, 0.03m),
                LimitOrder.CreateSell(6130.929m, 0.02m),
                LimitOrder.CreateSell(6060.000m, 0.01m),
                LimitOrder.CreateBuy(5866.741m, 0.01m),
                LimitOrder.CreateBuy(5820.499m, 0.02m),
                LimitOrder.CreateBuy(5743.785m, 0.03m),
                LimitOrder.CreateBuy(5660.571m, 0.04m),
                LimitOrder.CreateBuy(5571.071m, 0.05m)
            };

            // act

            IReadOnlyCollection<LimitOrder> actualLimitOrders = Calculator.CalculateLimitOrders(quote1, quote2, levels,
                baseAssetBalance, quoteAssetBalance, timeSinceLastTrade, halfLifePeriod, true, 0, priceAccuracy,
                volumeAccuracy);

            // assert

            Assert.IsTrue(AreEqual(expectedLimitOrders, actualLimitOrders));
        }

        [TestMethod]
        public void Create_Dynamically_Distributed_Limit_Orders_By_Levels_With_Sell_Smart_Markup()
        {
            // arrange

            int priceAccuracy = 3;
            int volumeAccuracy = 8;

            var quote1 = new Quote("BTCUSD", DateTime.UtcNow, 6000, 5950, "none");
            var quote2 = new Quote("BTCUSD", DateTime.UtcNow, 6050, 5900, "none");

            decimal baseAssetBalance = -10;
            decimal quoteAssetBalance = -10;
            int timeSinceLastTrade = 1;
            int halfLifePeriod = 30;
                
            var levels = new[]
            {
                new InstrumentLevel {Number = 1, Volume = 0.01m, Markup = 0.01m},
                new InstrumentLevel {Number = 2, Volume = 0.02m, Markup = 0.02m},
                new InstrumentLevel {Number = 3, Volume = 0.03m, Markup = 0.03m},
                new InstrumentLevel {Number = 4, Volume = 0.04m, Markup = 0.04m},
                new InstrumentLevel {Number = 5, Volume = 0.05m, Markup = 0.05m}
            };

            var expectedLimitOrders = new List<LimitOrder>
            {
                LimitOrder.CreateSell(6390.000m, 0.05m),
                LimitOrder.CreateSell(6295.715m, 0.04m),
                LimitOrder.CreateSell(6209.429m, 0.03m),
                LimitOrder.CreateSell(6130.929m, 0.02m),
                LimitOrder.CreateSell(6083.958m, 0.01m),
                LimitOrder.CreateBuy(5890.500m, 0.01m),
                LimitOrder.CreateBuy(5820.499m, 0.02m),
                LimitOrder.CreateBuy(5743.785m, 0.03m),
                LimitOrder.CreateBuy(5660.571m, 0.04m),
                LimitOrder.CreateBuy(5571.071m, 0.05m)
            };

            // act

            IReadOnlyCollection<LimitOrder> actualLimitOrders = Calculator.CalculateLimitOrders(quote1, quote2, levels,
                baseAssetBalance, quoteAssetBalance, timeSinceLastTrade, halfLifePeriod, true, 0, priceAccuracy,
                volumeAccuracy);

            // assert

            Assert.IsTrue(AreEqual(expectedLimitOrders, actualLimitOrders));
        }
        
        [TestMethod]
        public void Create_Dynamically_Distributed_Limit_Orders_By_Levels_With_Sell_Smart_Markup_With_One_Quote()
        {
            // arrange

            int priceAccuracy = 3;
            int volumeAccuracy = 8;

            var quote1 = new Quote("BTCUSD", DateTime.UtcNow, 6000, 5950, "none");
            var quote2 = new Quote("BTCUSD", DateTime.UtcNow, 6000, 5950, "none");

            decimal baseAssetBalance = -10;
            decimal quoteAssetBalance = -10;
            int timeSinceLastTrade = 1;
            int halfLifePeriod = 30;
                
            var levels = new[]
            {
                new InstrumentLevel {Number = 1, Volume = 0.01m, Markup = 0.01m},
                new InstrumentLevel {Number = 2, Volume = 0.02m, Markup = 0.02m},
                new InstrumentLevel {Number = 3, Volume = 0.03m, Markup = 0.03m},
                new InstrumentLevel {Number = 4, Volume = 0.04m, Markup = 0.04m},
                new InstrumentLevel {Number = 5, Volume = 0.05m, Markup = 0.05m}
            };

            var expectedLimitOrders = new List<LimitOrder>
            {
                LimitOrder.CreateSell(6300.000m, 0.05m),
                LimitOrder.CreateSell(6240.000m, 0.04m),
                LimitOrder.CreateSell(6180.000m, 0.03m),
                LimitOrder.CreateSell(6120.000m, 0.02m),
                LimitOrder.CreateSell(6083.958m, 0.01m),
                LimitOrder.CreateBuy(5890.500m, 0.01m),
                LimitOrder.CreateBuy(5831.000m, 0.02m),
                LimitOrder.CreateBuy(5771.500m, 0.03m),
                LimitOrder.CreateBuy(5712.000m, 0.04m),
                LimitOrder.CreateBuy(5652.500m, 0.05m)
            };

            // act

            IReadOnlyCollection<LimitOrder> actualLimitOrders = Calculator.CalculateLimitOrders(quote1, quote2, levels,
                baseAssetBalance, quoteAssetBalance, timeSinceLastTrade, halfLifePeriod, true, 0, priceAccuracy,
                volumeAccuracy);

            // assert

            Assert.IsTrue(AreEqual(expectedLimitOrders, actualLimitOrders));
        }
        
        [TestMethod]
        public void Calculate_Direct_Cross_Sell_Price()
        {
            // arrange

            decimal price = 55.57938m;

            var quote = new Quote("USDCHF", DateTime.UtcNow, .95m, .8m, "none");

            decimal expectedCrossPrice = price * quote.Ask;

            // act

            decimal actualCrossPrice = Calculator.CalculateCrossSellPrice(quote, price, false);

            // assert

            Assert.AreEqual(expectedCrossPrice, actualCrossPrice);
        }

        [TestMethod]
        public void Calculate_Inverse_Cross_Sell_Price()
        {
            // arrange

            decimal price = 55.57938m;

            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal expectedCrossPrice = price / quote.Bid;

            // act

            decimal actualCrossPrice = Calculator.CalculateCrossSellPrice(quote, price, true);

            // assert

            Assert.AreEqual(expectedCrossPrice, actualCrossPrice);
        }

        [TestMethod]
        public void Calculate_Direct_Cross_Buy_Price()
        {
            // arrange

            decimal price = 53.99062m;

            var quote = new Quote("USDCHF", DateTime.UtcNow, .95m, .8m, "none");

            decimal expectedCrossPrice = price * quote.Bid;

            // act

            decimal actualCrossPrice = Calculator.CalculateCrossBuyPrice(quote, price, false);

            // assert

            Assert.AreEqual(expectedCrossPrice, actualCrossPrice);
        }

        [TestMethod]
        public void Calculate_Inverse_Cross_Buy_Price()
        {
            // arrange

            decimal price = 53.99062m;

            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal expectedCrossPrice = price / quote.Ask;

            // act

            decimal actualCrossPrice = Calculator.CalculateCrossBuyPrice(quote, price, true);

            // assert

            Assert.AreEqual(expectedCrossPrice, actualCrossPrice);
        }

        [TestMethod]
        public void Calculate_Direct_Sell_Price()
        {
            // arrange

            var quote = new Quote("USDCHF", DateTime.UtcNow, .95m, .8m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price / quote.Ask;

            // act

            decimal actualPrice = Calculator.CalculateDirectSellPrice(price, quote, false);

            // assert

            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [TestMethod]
        public void Calculate_Inverse_Direct_Sell_Price()
        {
            // arrange

            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price * quote.Bid;

            // act

            decimal actualPrice = Calculator.CalculateDirectSellPrice(price, quote, true);

            // assert

            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [TestMethod]
        public void Calculate_Direct_Buy_Price()
        {
            // arrange

            var quote = new Quote("USDCHF", DateTime.UtcNow, .95m, .8m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price / quote.Bid;

            // act

            decimal actualPrice = Calculator.CalculateDirectBuyPrice(price, quote, false);

            // assert

            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [TestMethod]
        public void Calculate_Inverse_Direct_Buy_Price()
        {
            // arrange

            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price * quote.Ask;

            // act

            decimal actualPrice = Calculator.CalculateDirectBuyPrice(price, quote, true);

            // assert

            Assert.AreEqual(expectedPrice, actualPrice);
        }

        private bool AreEqual(IReadOnlyCollection<LimitOrder> a, IReadOnlyCollection<LimitOrder> b)
        {
            if (a.Count != b.Count)
                return false;

            return a.All(o => b.Any(p => p.Type == o.Type && p.Volume == o.Volume && p.Price == o.Price));
        }
    }
}
