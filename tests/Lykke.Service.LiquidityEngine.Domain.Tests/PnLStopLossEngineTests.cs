using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.LiquidityEngine.Domain.Tests
{
    [TestClass]
    public class PnLStopLossEngineTests
    {
        [TestMethod]
        public void Initialized_And_Refreshed_Without_Negative_PnL()
        {
            // arrange

            var interval = TimeSpan.FromSeconds(1);
            var threshold = 100;
            var markup = 0.002m;

            var pnLStopLossEngine = new PnLStopLossEngine
            {
                Id = Guid.NewGuid().ToString(),
                Mode = PnLStopLossEngineMode.Idle,
                Interval = interval,
                Threshold = threshold,
                Markup = markup
            };

            // act

            pnLStopLossEngine.Refresh();

            // assert

            Assert.IsFalse(string.IsNullOrWhiteSpace(pnLStopLossEngine.Id));
            Assert.AreEqual(pnLStopLossEngine.Interval, interval);
            Assert.AreEqual(pnLStopLossEngine.Threshold, threshold);
            Assert.AreEqual(pnLStopLossEngine.Markup, markup);
            Assert.AreEqual(pnLStopLossEngine.Mode, PnLStopLossEngineMode.Idle);
            Assert.AreEqual(pnLStopLossEngine.TotalNegativePnL, 0);
            Assert.IsNull(pnLStopLossEngine.StartTime);
            Assert.IsNull(pnLStopLossEngine.LastTime);
        }

        [TestMethod]
        public void One_Negative_PnL_And_Threshold_Is_Not_Exceeded()
        {
            // arrange

            var interval = TimeSpan.FromSeconds(1);
            var threshold = 100;
            var markup = 0.002m;
            var negativePnL = -99.99999999m;

            var pnLStopLossEngine = new PnLStopLossEngine
            {
                Id = Guid.NewGuid().ToString(),
                Mode = PnLStopLossEngineMode.Idle,
                Interval = interval,
                Threshold = threshold,
                Markup = markup
            };

            // act

            pnLStopLossEngine.AddNegativePnL(negativePnL);

            pnLStopLossEngine.Refresh();

            // assert

            Assert.IsFalse(string.IsNullOrWhiteSpace(pnLStopLossEngine.Id));
            Assert.AreEqual(pnLStopLossEngine.Interval, interval);
            Assert.AreEqual(pnLStopLossEngine.Threshold, threshold);
            Assert.AreEqual(pnLStopLossEngine.Markup, markup);
            Assert.AreEqual(pnLStopLossEngine.Mode, PnLStopLossEngineMode.Idle);
            Assert.AreEqual(pnLStopLossEngine.TotalNegativePnL, negativePnL);
            Assert.IsNotNull(pnLStopLossEngine.StartTime);
            Assert.IsNotNull(pnLStopLossEngine.LastTime);
        }

        [TestMethod]
        public void One_Negative_PnL_And_Threshold_Is_Exceeded()
        {
            // arrange

            var interval = TimeSpan.FromSeconds(1);
            var threshold = 100;
            var markup = 0.002m;
            var negativePnL = -100.00000001m;

            var pnLStopLossEngine = new PnLStopLossEngine
            {
                Id = Guid.NewGuid().ToString(),
                Mode = PnLStopLossEngineMode.Idle,
                Interval = interval,
                Threshold = threshold,
                Markup = markup
            };

            // act

            pnLStopLossEngine.AddNegativePnL(negativePnL);

            pnLStopLossEngine.Refresh();

            // assert

            Assert.IsFalse(string.IsNullOrWhiteSpace(pnLStopLossEngine.Id));
            Assert.AreEqual(pnLStopLossEngine.Interval, interval);
            Assert.AreEqual(pnLStopLossEngine.Threshold, threshold);
            Assert.AreEqual(pnLStopLossEngine.Markup, markup);
            Assert.AreEqual(pnLStopLossEngine.Mode, PnLStopLossEngineMode.Active);
            Assert.AreEqual(pnLStopLossEngine.TotalNegativePnL, negativePnL);
            Assert.IsNotNull(pnLStopLossEngine.StartTime);
            Assert.IsNotNull(pnLStopLossEngine.LastTime);
        }

        [TestMethod]
        public void Threshold_Is_Exceeded_And_Then_Last_Time_Expired()
        {
            // arrange

            var interval = TimeSpan.FromSeconds(1);
            var threshold = 100;
            var markup = 0.002m;
            var negativePnL = -100.00000001m;

            var pnLStopLossEngine = new PnLStopLossEngine
            {
                Id = Guid.NewGuid().ToString(),
                Mode = PnLStopLossEngineMode.Idle,
                Interval = interval,
                Threshold = threshold,
                Markup = markup
            };

            // act

            pnLStopLossEngine.AddNegativePnL(negativePnL);

            pnLStopLossEngine.Refresh();

            // assert

            Assert.IsFalse(string.IsNullOrWhiteSpace(pnLStopLossEngine.Id));
            Assert.AreEqual(pnLStopLossEngine.Interval, interval);
            Assert.AreEqual(pnLStopLossEngine.Threshold, threshold);
            Assert.AreEqual(pnLStopLossEngine.Markup, markup);
            Assert.AreEqual(pnLStopLossEngine.Mode, PnLStopLossEngineMode.Active);
            Assert.AreEqual(pnLStopLossEngine.TotalNegativePnL, negativePnL);
            Assert.IsNotNull(pnLStopLossEngine.StartTime);
            Assert.IsNotNull(pnLStopLossEngine.LastTime);

            // act again

            Thread.Sleep(1000);

            pnLStopLossEngine.Refresh();

            // assert again

            Assert.IsFalse(string.IsNullOrWhiteSpace(pnLStopLossEngine.Id));
            Assert.AreEqual(pnLStopLossEngine.Interval, interval);
            Assert.AreEqual(pnLStopLossEngine.Threshold, threshold);
            Assert.AreEqual(pnLStopLossEngine.Markup, markup);
            Assert.AreEqual(pnLStopLossEngine.Mode, PnLStopLossEngineMode.Idle);
            Assert.AreEqual(0, pnLStopLossEngine.TotalNegativePnL);
            Assert.IsNull(pnLStopLossEngine.StartTime);
            Assert.IsNull(pnLStopLossEngine.LastTime);
        }

        [TestMethod]
        public void Threshold_Is_Not_Exceeded_And_Start_Time_Expired()
        {
            // arrange

            var interval = TimeSpan.FromSeconds(1);
            var threshold = 100;
            var markup = 0.002m;
            var negativePnL = -10;

            var pnLStopLossEngine = new PnLStopLossEngine
            {
                Id = Guid.NewGuid().ToString(),
                Mode = PnLStopLossEngineMode.Idle,
                Interval = interval,
                Threshold = threshold,
                Markup = markup
            };

            // act

            pnLStopLossEngine.AddNegativePnL(negativePnL);

            pnLStopLossEngine.Refresh();

            Thread.Sleep(900);

            pnLStopLossEngine.AddNegativePnL(negativePnL);

            pnLStopLossEngine.Refresh();

            // assert

            Assert.IsFalse(string.IsNullOrWhiteSpace(pnLStopLossEngine.Id));
            Assert.AreEqual(pnLStopLossEngine.Interval, interval);
            Assert.AreEqual(pnLStopLossEngine.Threshold, threshold);
            Assert.AreEqual(pnLStopLossEngine.Markup, markup);
            Assert.AreEqual(pnLStopLossEngine.Mode, PnLStopLossEngineMode.Idle);
            Assert.AreEqual(pnLStopLossEngine.TotalNegativePnL, negativePnL * 2);
            Assert.IsNotNull(pnLStopLossEngine.StartTime);
            Assert.IsNotNull(pnLStopLossEngine.LastTime);

            // act again

            Thread.Sleep(100);

            pnLStopLossEngine.Refresh();

            // assert again

            Assert.IsFalse(string.IsNullOrWhiteSpace(pnLStopLossEngine.Id));
            Assert.AreEqual(pnLStopLossEngine.Interval, interval);
            Assert.AreEqual(pnLStopLossEngine.Threshold, threshold);
            Assert.AreEqual(pnLStopLossEngine.Markup, markup);
            Assert.AreEqual(pnLStopLossEngine.Mode, PnLStopLossEngineMode.Idle);
            Assert.AreEqual(pnLStopLossEngine.TotalNegativePnL, 0);
            Assert.IsNull(pnLStopLossEngine.StartTime);
            Assert.IsNull(pnLStopLossEngine.LastTime);
        }
    }
}
