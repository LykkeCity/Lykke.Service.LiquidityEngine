using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
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
        private readonly IBalanceService _balanceService;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly ILog _log;

        public MarketMakerService(
            IInstrumentService instrumentService,
            IExternalExchangeService externalExchangeService,
            ILykkeExchangeService lykkeExchangeService,
            IOrderBookService orderBookService,
            IBalanceService balanceService,
            IAssetsServiceWithCache assetsServiceWithCache,
            ILogFactory logFactory)
        {
            _instrumentService = instrumentService;
            _externalExchangeService = externalExchangeService;
            _lykkeExchangeService = lykkeExchangeService;
            _orderBookService = orderBookService;
            _balanceService = balanceService;
            _assetsServiceWithCache = assetsServiceWithCache;
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
            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            var internalLimitOrders = new List<LimitOrder>();
            var externalLimitOrders = new List<LimitOrder>();

            decimal sellVolume = 0;
            decimal sellOppositeVolume = 0;
            decimal buyVolume = 0;
            decimal buyOppositeVolume = 0;

            foreach (LevelVolume levelVolume in instrument.Levels.OrderBy(o => o.Number))
            {
                sellVolume += levelVolume.Volume;
                buyVolume += levelVolume.Volume;

                decimal sellPrice =
                    await _externalExchangeService.GetSellPriceAsync(instrument.AssetPairId, sellVolume);

                decimal buyPrice =
                    await _externalExchangeService.GetBuyPriceAsync(instrument.AssetPairId, buyVolume);

                externalLimitOrders.Add(LimitOrder.CreateSell(sellPrice, sellVolume));
                externalLimitOrders.Add(LimitOrder.CreateBuy(buyPrice, buyVolume));

                sellPrice *= 1 + instrument.Markup;
                buyPrice *= 1 - instrument.Markup;

                decimal sellLimitOrderPrice = ((sellPrice * sellVolume - sellOppositeVolume) / levelVolume.Volume)
                    .TruncateDecimalPlaces(assetPair.Accuracy, true);
                decimal buyLimitOrderPrice = ((buyPrice * buyVolume - buyOppositeVolume) / levelVolume.Volume)
                    .TruncateDecimalPlaces(assetPair.Accuracy);

                decimal volume = Math.Round(levelVolume.Volume, assetPair.InvertedAccuracy);

                internalLimitOrders.Add(LimitOrder.CreateSell(sellLimitOrderPrice, volume));
                internalLimitOrders.Add(LimitOrder.CreateBuy(buyLimitOrderPrice, volume));

                sellOppositeVolume += levelVolume.Volume * sellLimitOrderPrice;
                buyOppositeVolume += levelVolume.Volume * buyLimitOrderPrice;
            }

            await _orderBookService.UpdateAsync(new OrderBook
            {
                AssetPairId = instrument.AssetPairId,
                ExternalLimitOrders = externalLimitOrders,
                InternalLimitOrders = internalLimitOrders
            });

            ValidateMinVolume(internalLimitOrders, (decimal) assetPair.MinVolume);

            await ValidateBalance(internalLimitOrders, assetPair);

            if (instrument.Mode == InstrumentMode.Active)
            {
                await _lykkeExchangeService.ApplyAsync(instrument.AssetPairId, internalLimitOrders);
            }
            else
            {
                foreach (LimitOrder limitOrder in internalLimitOrders)
                {
                    if (limitOrder.Error == LimitOrderError.None)
                        limitOrder.Error = LimitOrderError.Idle;
                }
            }
        }

        private void ValidateMinVolume(IReadOnlyCollection<LimitOrder> limitOrders, decimal minVolume)
        {
            foreach (LimitOrder limitOrder in limitOrders.Where(o => o.Error == LimitOrderError.None))
            {
                if (limitOrder.Volume < minVolume)
                    limitOrder.Error = LimitOrderError.TooSmallVolume;
            }
        }

        private async Task ValidateBalance(IReadOnlyCollection<LimitOrder> limitOrders, AssetPair assetPair)
        {
            Asset baseAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.BaseAssetId);
            Asset quoteAsset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.QuotingAssetId);

            List<LimitOrder> sellLimitOrders = limitOrders
                .Where(o => o.Error == LimitOrderError.None)
                .Where(o => o.Type == LimitOrderType.Sell)
                .OrderBy(o => o.Price)
                .ToList();

            List<LimitOrder> buyLimitOrders = limitOrders
                .Where(o => o.Error == LimitOrderError.None)
                .Where(o => o.Type == LimitOrderType.Buy)
                .OrderByDescending(o => o.Price)
                .ToList();

            if (sellLimitOrders.Any())
            {
                decimal balance = (await _balanceService.GetByAssetIdAsync(baseAsset.Id)).Amount;

                foreach (LimitOrder limitOrder in sellLimitOrders)
                {
                    decimal amount = limitOrder.Volume.TruncateDecimalPlaces(baseAsset.Accuracy, true);

                    if (balance - amount < 0)
                    {
                        decimal volume = balance.TruncateDecimalPlaces(assetPair.InvertedAccuracy);

                        if (volume < (decimal) assetPair.MinVolume)
                            limitOrder.Error = LimitOrderError.NotEnoughFunds;
                        else
                            limitOrder.UpdateVolume(volume);
                    }

                    balance -= amount;
                }
            }

            if (buyLimitOrders.Any())
            {
                decimal balance = (await _balanceService.GetByAssetIdAsync(quoteAsset.Id)).Amount;

                foreach (LimitOrder limitOrder in buyLimitOrders)
                {
                    decimal amount = (limitOrder.Price * limitOrder.Volume)
                        .TruncateDecimalPlaces(quoteAsset.Accuracy, true);

                    if (balance - amount < 0)
                    {
                        decimal volume = (balance / limitOrder.Price).TruncateDecimalPlaces(assetPair.InvertedAccuracy);

                        if (volume < (decimal) assetPair.MinVolume)
                            limitOrder.Error = LimitOrderError.NotEnoughFunds;
                        else
                            limitOrder.UpdateVolume(volume);
                    }

                    balance -= amount;
                }
            }
        }
    }
}
