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
        private readonly ILog _log;

        public SettlementService(
            IBalanceOperationService balanceOperationService,
            ILogFactory logFactory)
        {
            _balanceOperationService = balanceOperationService;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteAsync(string assetId, decimal amount, string comment, string userId)
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

            await _balanceOperationService.AddAsync(balanceOperation);

            _log.InfoWithDetails("Settlement was executed", balanceOperation);
        }
    }
}
