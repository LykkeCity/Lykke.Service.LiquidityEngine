using Lykke.Service.LiquidityEngine.DomainServices.FiatEquityStopLoss;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.LiquidityEngine.DomainServices.Tests
{
    [TestClass]
    public class FiatEquityStopLossServiceTests
    {
        [TestMethod]
        public void Fiat_Equity_Markup_Calculation_Test()
        {
            // arrange

            decimal thresholdFrom = 10;
            decimal thresholdTo = 50;
            decimal markupFrom = 0.01m;
            decimal markupTo = 0.05m;

            decimal fiatEquity = -30;

            // act

            var result = FiatEquityStopLossService.CalculateMarkup(fiatEquity, thresholdFrom, thresholdTo, markupFrom, markupTo);

            // assert

            Assert.AreEqual(0.03m, result);
        }

        [TestMethod]
        public void Fiat_Equity_No_Markup_Test()
        {
            // arrange

            decimal thresholdFrom = 10;
            decimal thresholdTo = 50;
            decimal markupFrom = 0.01m;
            decimal markupTo = 0.05m;

            decimal fiatEquity = 1;

            // act

            var result = FiatEquityStopLossService.CalculateMarkup(fiatEquity, thresholdFrom, thresholdTo, markupFrom, markupTo);

            // assert

            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void Fiat_Equity_Stop_Asks_Test()
        {
            // arrange

            decimal thresholdFrom = 10;
            decimal thresholdTo = 50;
            decimal markupFrom = 0.01m;
            decimal markupTo = 0.05m;

            decimal fiatEquity = -50;

            // act

            var result = FiatEquityStopLossService.CalculateMarkup(fiatEquity, thresholdFrom, thresholdTo, markupFrom, markupTo);

            // assert

            Assert.AreEqual(decimal.MinusOne, result);
        }
    }
}
