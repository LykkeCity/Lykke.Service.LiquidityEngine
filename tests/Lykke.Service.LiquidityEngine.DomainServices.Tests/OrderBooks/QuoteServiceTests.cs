using System;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.OrderBooks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.LiquidityEngine.DomainServices.Tests.OrderBooks
{
    [TestClass]
    public class QuoteServiceTests
    {
        private readonly Mock<IQuoteThresholdSettingsService> _quoteThresholdSettingsServiceMock =
            new Mock<IQuoteThresholdSettingsService>();

        private readonly Mock<IQuoteThresholdLogService> _quoteThresholdLogServiceMock =
            new Mock<IQuoteThresholdLogService>();

        private QuoteThresholdSettings _quoteThresholdSettings;
        
        private QuoteService _service;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _quoteThresholdSettings = new QuoteThresholdSettings
            {
                Enabled = true,
                Value = .2m
            };

            _quoteThresholdSettingsServiceMock.Setup(o => o.GetAsync())
                .Returns(() => Task.FromResult(_quoteThresholdSettings));
            
            _service = new QuoteService(
                _quoteThresholdSettingsServiceMock.Object,
                _quoteThresholdLogServiceMock.Object);
        }

        [TestMethod]
        public async Task Skip_Wrong_Quote()
        {
            // arrange

            var quote = new Quote("BTCUSD", DateTime.UtcNow.AddMinutes(-1), 10, 9, "lykke");
            var invalidQuote = new Quote("BTCUSD", DateTime.UtcNow, 100, 90, "lykke");

            // act

            await _service.SetAsync(quote);

            await _service.SetAsync(invalidQuote);

            Quote actualQuote = await _service.GetAsync("lykke", quote.AssetPair);
            
            // assert

            Assert.IsTrue(quote.Ask == actualQuote.Ask &&
                          quote.Bid == actualQuote.Bid &&
                          quote.Time == actualQuote.Time);
        }
    }
}
