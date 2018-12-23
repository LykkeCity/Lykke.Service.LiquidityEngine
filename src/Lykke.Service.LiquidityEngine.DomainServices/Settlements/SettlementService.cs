using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Settlements
{
    [UsedImplicitly]
    public class SettlementService : ISettlementService
    {
        private readonly ISettlementTradeService _settlementTradeService;
        private readonly IBalanceOperationService _balanceOperationService;
        private readonly ILykkeExchangeService _lykkeExchangeService;
        private readonly ISettingsService _settingsService;
        private readonly IAssetSettingsService _assetSettingsService;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly ILog _log;

        public SettlementService(
            ISettlementTradeService settlementTradeService,
            IBalanceOperationService balanceOperationService,
            ILykkeExchangeService lykkeExchangeService,
            ISettingsService settingsService,
            IAssetSettingsService assetSettingsService,
            IAssetsServiceWithCache assetsServiceWithCache,
            ILogFactory logFactory)
        {
            _settlementTradeService = settlementTradeService;
            _balanceOperationService = balanceOperationService;
            _lykkeExchangeService = lykkeExchangeService;
            _settingsService = settingsService;
            _assetSettingsService = assetSettingsService;
            _assetsServiceWithCache = assetsServiceWithCache;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteAsync(string assetId, decimal amount, string comment, bool allowChangeBalance,
            string userId)
        {
            var balanceOperation = new BalanceOperation
            {
                Time = DateTime.UtcNow,
                AssetId = assetId,
                Type = "Settlement",
                Amount = amount,
                Comment = comment,
                UserId = userId
            };

            if (allowChangeBalance)
            {
                string walletId = await _settingsService.GetWalletIdAsync();

                if (amount > 0)
                    await _lykkeExchangeService.CashInAsync(walletId, assetId, Math.Abs(amount), userId, comment);
                else
                    await _lykkeExchangeService.CashOutAsync(walletId, assetId, Math.Abs(amount), userId, comment);
            }

            await _balanceOperationService.AddAsync(balanceOperation);

            _log.InfoWithDetails("Settlement was executed", balanceOperation);
        }

        public async Task ExecuteAsync(string settlementTradeId, string userId)
        {
            string walletId = await _settingsService.GetWalletIdAsync();

            SettlementTrade settlementTrade = (await _settlementTradeService.GetByIdAsync(settlementTradeId)).Copy();

            if (settlementTrade == null)
                throw new EntityNotFoundException();

            Domain.AssetSettings baseAssetSettings =
                await _assetSettingsService.GetByIdAsync(settlementTrade.BaseAsset);

            Domain.AssetSettings quoteAssetSettings =
                await _assetSettingsService.GetByIdAsync(settlementTrade.QuoteAsset);

            string baseAssetId = baseAssetSettings != null
                ? baseAssetSettings.LykkeAssetId
                : settlementTrade.BaseAsset;

            string quoteAssetId = quoteAssetSettings != null
                ? quoteAssetSettings.LykkeAssetId
                : settlementTrade.QuoteAsset;

            Asset baseAsset = await _assetsServiceWithCache.TryGetAssetAsync(baseAssetId);

            Asset quoteAsset = await _assetsServiceWithCache.TryGetAssetAsync(quoteAssetId);

            if (baseAsset == null)
            {
                _log.WarningWithDetails("Can not map external asset id", new
                {
                    settlementTradeId,
                    userId,
                    baseAssetId
                });

                throw new InvalidOperationException("Unknown asset");
            }

            if (quoteAsset == null)
            {
                _log.WarningWithDetails("Can not map external asset id", new
                {
                    settlementTradeId,
                    userId,
                    quoteAssetId
                });

                throw new InvalidOperationException("Unknown asset");
            }

            settlementTrade.Complete();
            
            string comment =
                $"rebalance; {settlementTrade.Type} {settlementTrade.Volume} {settlementTrade.BaseAsset} for {settlementTrade.OppositeVolume} {settlementTrade.QuoteAsset} by price {settlementTrade.Price} ({settlementTrade.AssetPair})";

            int sign = settlementTrade.Type == TradeType.Sell ? 1 : -1;

            var balanceOperations = new[]
            {
                new BalanceOperation
                {
                    Time = DateTime.UtcNow,
                    AssetId = baseAsset.Id,
                    Type = "Settlement",
                    Amount = Math.Abs(settlementTrade.Volume) * sign,
                    Comment = $"{comment}, for close position in {settlementTrade.BaseAsset}",
                    UserId = userId
                },
                new BalanceOperation
                {
                    Time = DateTime.UtcNow,
                    AssetId = quoteAsset.Id,
                    Type = "Settlement",
                    Amount = Math.Abs(settlementTrade.OppositeVolume) * -1 * sign,
                    Comment = $"{comment}, for close position in {settlementTrade.QuoteAsset}",
                    UserId = userId
                }
            };

            foreach (BalanceOperation balanceOperation in balanceOperations.OrderBy(o => o.Amount))
            {
                if (balanceOperation.Amount > 0)
                {
                    await _lykkeExchangeService.CashInAsync(walletId, balanceOperation.AssetId,
                        Math.Abs(balanceOperation.Amount), userId, comment);
                }
                else
                {
                    await _lykkeExchangeService.CashOutAsync(walletId, balanceOperation.AssetId,
                        Math.Abs(balanceOperation.Amount), userId, comment);
                }

                await _balanceOperationService.AddAsync(balanceOperation);

                _log.InfoWithDetails("Settlement was executed", balanceOperation);
            }

            await _settlementTradeService.UpdateAsync(settlementTrade);
        }
    }
}
