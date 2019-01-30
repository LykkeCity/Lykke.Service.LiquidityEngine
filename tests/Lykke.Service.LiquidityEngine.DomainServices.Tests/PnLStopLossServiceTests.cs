using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.PnLStopLossEngines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.LiquidityEngine.DomainServices.Tests
{
    [TestClass]
    public class PnLStopLossServiceTests
    {
        private const string AssetPairId = "BTCUSD";

        private readonly Mock<IPnLStopLossEngineRepository> _pnLStopLossEngineRepository =
            new Mock<IPnLStopLossEngineRepository>();

        private readonly Mock<IInstrumentService> _instrumentService =
            new Mock<IInstrumentService>();

        private readonly Mock<ICrossRateInstrumentService> _crossRateInstrumentService =
            new Mock<ICrossRateInstrumentService>();

        private readonly ConcurrentDictionary<string, PnLStopLossEngine> _cache
            = new ConcurrentDictionary<string,PnLStopLossEngine>();

        [TestInitialize]
        public void TestInitialize()
        {
            _pnLStopLossEngineRepository.Setup(o => o.GetAllAsync())
                .Returns(() => Task.FromResult(
                    _cache.Values.ToList() as IReadOnlyCollection<PnLStopLossEngine>));

            _pnLStopLossEngineRepository.Setup(o => o.InsertAsync(It.IsAny<PnLStopLossEngine>()))
                .Returns((PnLStopLossEngine engine) => 
                    Task.FromResult(_cache[engine.Id] = engine));

            _pnLStopLossEngineRepository.Setup(o => o.UpdateAsync(It.IsAny<PnLStopLossEngine>()))
                .Returns((PnLStopLossEngine engine) => 
                    Task.FromResult(_cache[engine.Id] = engine));

            _pnLStopLossEngineRepository.Setup(o => o.DeleteAsync(It.IsAny<string>()))
                .Returns((string id) => 
                    Task.FromResult(_cache.Remove(id, out _)));

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

            _crossRateInstrumentService.Setup(o => o.ConvertPriceAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns((string assetPairId, decimal price) => Task.FromResult(price as decimal?));
        }

        [TestMethod]
        public async Task PnL_Stop_Loss_First_Position_With_Exceeded_Threshold_Test()
        {
            // arrange

            var pnLStopLossEngineService = await GetInstance();

            var engine = new PnLStopLossEngine
            {
                AssetPairId = AssetPairId,
                Interval = TimeSpan.FromSeconds(3),
                Markup = 0.03m,
                Threshold = -100
            };

            await pnLStopLossEngineService.AddAsync(engine);

            // act

            var position = new Position
            {
                AssetPairId = AssetPairId,
                PnL = -100
            };
            await pnLStopLossEngineService.HandleClosedPositionAsync(position);

            // assert 

            engine = (await pnLStopLossEngineService.GetAllAsync()).Single();
            Assert.AreEqual(PnLStopLossEngineMode.Active, engine.Mode);
            Assert.AreEqual(-100m, engine.TotalNegativePnL);
            var markup = await pnLStopLossEngineService.GetTotalMarkupByAssetPairIdAsync(AssetPairId);
            Assert.AreEqual(0.03m, markup);
        }

        [TestMethod]
        public async Task PnL_Stop_Loss_Test()
        {
            // arrange

            var pnLStopLossEngineService = await GetInstance();

            var engine = new PnLStopLossEngine
            {
                AssetPairId = AssetPairId,
                Interval = TimeSpan.FromSeconds(3),
                Markup = 0.03m,
                Threshold = -100
            };

            await pnLStopLossEngineService.AddAsync(engine);

            // act

            var position = new Position
            {
                AssetPairId = AssetPairId,
                PnL = -50
            };
            await pnLStopLossEngineService.HandleClosedPositionAsync(position);

            // assert that negative PnL in NOT enough
            engine = (await pnLStopLossEngineService.GetAllAsync()).Single();
            Assert.AreEqual(PnLStopLossEngineMode.Idle, engine.Mode);
            Assert.AreEqual(-50m, engine.TotalNegativePnL);
            var markup = await pnLStopLossEngineService.GetTotalMarkupByAssetPairIdAsync(AssetPairId);
            Assert.AreEqual(0m, markup);

            Thread.Sleep(2 * 1000);

            // act
            position = new Position
            {
                AssetPairId = AssetPairId,
                PnL = -51
            };
            await pnLStopLossEngineService.HandleClosedPositionAsync(position);

            Thread.Sleep(2 * 1000);

            await pnLStopLossEngineService.ExecuteAsync();

            // assert that negative PnL in enough
            engine = (await pnLStopLossEngineService.GetAllAsync()).Single();
            Assert.AreEqual(PnLStopLossEngineMode.Active, engine.Mode);
            Assert.AreEqual(-101m, engine.TotalNegativePnL);
            markup = await pnLStopLossEngineService.GetTotalMarkupByAssetPairIdAsync(AssetPairId);
            Assert.AreEqual(0.03m, markup);

            // wait until interval is up

            Thread.Sleep(3 * 1000);

            await pnLStopLossEngineService.ExecuteAsync();

            // assert again that engine is off and markup is 0
            engine = (await pnLStopLossEngineService.GetAllAsync()).Single();
            Assert.AreEqual(PnLStopLossEngineMode.Idle, engine.Mode);
            Assert.AreEqual(0m, engine.TotalNegativePnL);
            markup = await pnLStopLossEngineService.GetTotalMarkupByAssetPairIdAsync(AssetPairId);
            Assert.AreEqual(0m, markup);
        }

        private async Task<IPnLStopLossEngineService> GetInstance()
        {
            var result = new PnLStopLossEngineService(
                _pnLStopLossEngineRepository.Object,
                _instrumentService.Object,
                _crossRateInstrumentService.Object,
                LogFactory.Create()
            );

            return result;
        }
    }
}
