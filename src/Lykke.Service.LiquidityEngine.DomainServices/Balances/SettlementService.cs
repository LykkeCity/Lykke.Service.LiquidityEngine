using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Balances
{
    [UsedImplicitly]
    public class SettlementService : ISettlementService
    {
        private readonly IBalanceOperationService _balanceOperationService;
        private readonly ILykkeExchangeService _lykkeExchangeService;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public SettlementService(
            IBalanceOperationService balanceOperationService,
            ILykkeExchangeService lykkeExchangeService,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _balanceOperationService = balanceOperationService;
            _lykkeExchangeService = lykkeExchangeService;
            _settingsService = settingsService;
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
    }
}
