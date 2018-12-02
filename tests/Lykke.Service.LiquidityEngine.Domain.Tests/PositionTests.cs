using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.LiquidityEngine.Domain.Tests
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void LongPosition_PositivePnL()
        {
            // act

            Position position = OpenClosePosition(TradeType.Buy, 6000, 6001, 2);

            // assert

            Assert.AreEqual(PositionType.Long, position.Type);
            Assert.AreEqual(2, position.PnL);
        }

        [TestMethod]
        public void LongPosition_NegativePnL()
        {
            // act

            Position position = OpenClosePosition(TradeType.Buy, 6000, 5999, 2);

            // assert

            Assert.AreEqual(PositionType.Long, position.Type);
            Assert.AreEqual(-2, position.PnL);
        }

        [TestMethod]
        public void ShortPosition_PositivePnL()
        {
            // act

            Position position = OpenClosePosition(TradeType.Sell, 6000, 5999, 2);

            // assert

            Assert.AreEqual(PositionType.Short, position.Type);
            Assert.AreEqual(2, position.PnL);
        }

        [TestMethod]
        public void ShortPosition_NegativePnL()
        {
            // act

            Position position = OpenClosePosition(TradeType.Sell, 6000, 6001, 2);

            // assert

            Assert.AreEqual(PositionType.Short, position.Type);
            Assert.AreEqual(-2, position.PnL);
        }

        private static Position OpenClosePosition(TradeType type, decimal openPrice, decimal closePrice, decimal volume)
        {
            Position position = Position.Open("BTCUSD", openPrice, volume, type, Guid.NewGuid().ToString());

            position.Close(new ExternalTrade
            {
                Id = Guid.NewGuid().ToString(),
                Price = closePrice
            });

            return position;
        }
    }
}
