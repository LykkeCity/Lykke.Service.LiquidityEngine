using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    [UsedImplicitly]
    public class MarketMakerService : IMarketMakerService
    {
        private readonly IInstrumentService _instrumentService;
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly ILykkeExchangeService _lykkeExchangeService;
        private readonly IOrderBookService _orderBookService;
        private readonly ILog _log;
        
        public MarketMakerService(
            IInstrumentService instrumentService,
            IExternalExchangeService externalExchangeService,
            ILykkeExchangeService lykkeExchangeService,
            IOrderBookService orderBookService,
            ILogFactory logFactory)
        {
            _instrumentService = instrumentService;
            _externalExchangeService = externalExchangeService;
            _lykkeExchangeService = lykkeExchangeService;
            _orderBookService = orderBookService;
            _log = logFactory.CreateLog(this);
        }

        public async Task UpdateOrderBooksAsync()
        {
            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            IReadOnlyCollection<Instrument> activeInstruments = instruments
                .Where(o => o.Mode == InstrumentMode.Idle || o.Mode == InstrumentMode.Active)
                .ToArray();

            await Task.WhenAll(activeInstruments.Select(ProcessInstrumentAsync));
        }

        private async Task ProcessInstrumentAsync(Instrument instrument)
        {
            var internalLimitOrders = new List<LimitOrder>();
            var externalLimitOrders = new List<LimitOrder>();

            decimal sellVolume = 0;
            decimal sellOppositeVolume = 0;
            decimal buyVolume = 0;
            decimal buyOppositeVolume = 0;
            
            foreach (LevelVolume levelVolume in instrument.Levels.OrderBy(o=>o.Number))
            {
                sellVolume += levelVolume.Volume;
                buyVolume += levelVolume.Volume;

                decimal sellPrice =
                    await _externalExchangeService.GetSellPriceAsync(instrument.AssetPairId, sellVolume);

                decimal buyPrice =
                    await _externalExchangeService.GetBuyPriceAsync(instrument.AssetPairId, buyVolume);

                sellPrice *= 1 + instrument.Markup;
                buyPrice *= 1 - instrument.Markup;
                
                externalLimitOrders.Add(LimitOrder.CreateSell(sellPrice, sellVolume));
                externalLimitOrders.Add(LimitOrder.CreateBuy(buyPrice, buyVolume));

                internalLimitOrders.Add(LimitOrder.CreateSell(
                    (sellPrice * sellVolume - sellOppositeVolume) / levelVolume.Volume, levelVolume.Volume));
                internalLimitOrders.Add(LimitOrder.CreateBuy(
                    (buyPrice * buyVolume - buyOppositeVolume) / levelVolume.Volume, levelVolume.Volume));
                
                sellOppositeVolume += sellVolume * sellPrice * (1 + instrument.Markup);
                buyOppositeVolume += buyVolume * buyPrice * (1 - instrument.Markup);
            }

            await _orderBookService.UpdateAsync(new OrderBook
            {
                AssetPairId = instrument.AssetPairId,
                ExternalLimitOrders = externalLimitOrders,
                InternalLimitOrders = internalLimitOrders
            });

            await _lykkeExchangeService.ApplyAsync(instrument.AssetPairId, internalLimitOrders);
        }
    }
}
