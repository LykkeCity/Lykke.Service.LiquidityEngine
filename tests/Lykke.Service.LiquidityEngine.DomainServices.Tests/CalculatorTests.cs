using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Service.Assets.Client.Models;
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
        public void Calculate_Direct_Cross_Sell_Price()
        {
            // arrange

            decimal price = 55.57938m;

            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal expectedCrossPrice = price / quote.Bid;
            
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

            var quote = new Quote("USDEUR", DateTime.UtcNow, 0.900901m, 0.869565m, "none");

            decimal expectedCrossPrice = price * quote.Bid;
            
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

            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal expectedCrossPrice = price / quote.Ask;
            
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

            var quote = new Quote("USDEUR", DateTime.UtcNow, 0.869565m, 0.900901m, "none");

            decimal expectedCrossPrice = price * quote.Ask;
            
            // act

            decimal actualCrossPrice = Calculator.CalculateCrossBuyPrice(quote, price, true);

            // assert

            Assert.AreEqual(expectedCrossPrice, actualCrossPrice);
        }

        [TestMethod]
        public void Calculate_Direct_Sell_Price()
        {
            // arrange
            
            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price * quote.Bid;
            
            // act

            decimal actualPrice = Calculator.CalculateDirectSellPrice(price, quote, false);

            // assert

            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [TestMethod]
        public void Calculate_Inverse_Direct_Sell_Price()
        {
            // arrange
            
            var quote = new Quote("USDEUR", DateTime.UtcNow, 0.869565m, 0.900901m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price / quote.Bid;
            
            // act

            decimal actualPrice = Calculator.CalculateDirectSellPrice(price, quote, true);

            // assert

            Assert.AreEqual(expectedPrice, actualPrice);
        }
        
        [TestMethod]
        public void Calculate_Direct_Buy_Price()
        {
            // arrange
            
            var quote = new Quote("EURUSD", DateTime.UtcNow, 1.15m, 1.11m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price * quote.Ask;
            
            // act

            decimal actualPrice = Calculator.CalculateDirectBuyPrice(price, quote, false);

            // assert

            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [TestMethod]
        public void Calculate_Inverse_Direct_Buy_Price()
        {
            // arrange
            
            var quote = new Quote("USDEUR", DateTime.UtcNow, 0.869565m, 0.900901m, "none");

            decimal price = 50.07151m;

            decimal expectedPrice = price / quote.Ask;
            
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
