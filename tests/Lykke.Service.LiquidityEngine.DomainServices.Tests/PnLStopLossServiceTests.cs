using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.PnLStopLosses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.LiquidityEngine.DomainServices.Tests
{
    [TestClass]
    public class PnLStopLossServiceTests
    {
        private const string AssetPairId = "BTCUSD";

        private readonly Mock<IPnLStopLossSettingsRepository> _pnLStopLossSettingsRepository =
            new Mock<IPnLStopLossSettingsRepository>();

        private readonly Mock<IPnLStopLossEngineRepository> _pnLStopLossEngineRepository =
            new Mock<IPnLStopLossEngineRepository>();

        private readonly Mock<IInstrumentService> _instrumentService =
            new Mock<IInstrumentService>();

        [TestInitialize]
        public void TestInitialize()
        {
            _pnLStopLossSettingsRepository.Setup(o => o.GetAllAsync())
                .Returns(() => Task.FromResult(new List<PnLStopLossSettings>() as IReadOnlyCollection<PnLStopLossSettings>));

            _pnLStopLossSettingsRepository.Setup(o => o.InsertAsync(It.IsAny<PnLStopLossSettings>()))
                .Returns(() => Task.CompletedTask);

            _pnLStopLossSettingsRepository.Setup(o => o.DeleteAsync(It.IsAny<string>()))
                .Returns(() => Task.CompletedTask);


            _pnLStopLossEngineRepository.Setup(o => o.GetAllAsync())
                .Returns(() => Task.FromResult(new List<PnLStopLossEngine>() as IReadOnlyCollection<PnLStopLossEngine>));

            _pnLStopLossEngineRepository.Setup(o => o.InsertAsync(It.IsAny<PnLStopLossEngine>()))
                .Returns(() => Task.CompletedTask);

            _pnLStopLossEngineRepository.Setup(o => o.UpdateAsync(It.IsAny<PnLStopLossEngine>()))
                .Returns(() => Task.CompletedTask);

            _pnLStopLossEngineRepository.Setup(o => o.DeleteAsync(It.IsAny<string>()))
                .Returns(() => Task.CompletedTask);


            _instrumentService.Setup(o => o.GetAllAsync())
                .Returns(() => Task.FromResult(new List<Instrument> { new Instrument
                {
                    AssetPairId = AssetPairId
                } } as IReadOnlyCollection<Instrument>));

            _instrumentService.Setup(o => o.TryGetByAssetPairIdAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(new Instrument
                {
                    AssetPairId = AssetPairId
                }));

        }

        [TestMethod]
        public async Task PnL_Stop_Loss_Test()
        {
            // arrange

            var pnLStopLossService = new PnLStopLossService(
                _pnLStopLossSettingsRepository.Object,
                _pnLStopLossEngineRepository.Object,
                _instrumentService.Object,
                LogFactory.Create()
                );

            await pnLStopLossService.Initialize();

            var settings = new PnLStopLossSettings
            {
                AssetPairId = AssetPairId,
                Interval = TimeSpan.FromSeconds(3),
                Markup = 0.03m,
                PnLThreshold = -100
            };

            await pnLStopLossService.CreateAsync(settings);

            // act

            var position = new Position
            {
                AssetPairId = AssetPairId,
                PnL = -50
            };

            await pnLStopLossService.HandleClosedPositionAsync(position);

            Thread.Sleep(2 * 1000);

            position = new Position
            {
                AssetPairId = AssetPairId,
                PnL = -51
            };

            await pnLStopLossService.HandleClosedPositionAsync(position);

            Thread.Sleep(2 * 1000);

            await pnLStopLossService.ExecuteAsync();

            // assert

            var engine = (await pnLStopLossService.GetAllEnginesAsync()).Single();

            Assert.AreEqual(-101m, engine.TotalNegativePnL);

            var markup = await pnLStopLossService.GetTotalMarkupByAssetPairIdAsync(AssetPairId);

            Assert.AreEqual(0.03m, markup);

            // wait until interval is up

            Thread.Sleep(3 * 1000);

            await pnLStopLossService.ExecuteAsync();

            // assert again that engine is off and markup is 0

            engine = (await pnLStopLossService.GetAllEnginesAsync()).Single();

            Assert.AreEqual(0m, engine.TotalNegativePnL);

            markup = await pnLStopLossService.GetTotalMarkupByAssetPairIdAsync(AssetPairId);

            Assert.AreEqual(0m, markup);
        }
    }
}
