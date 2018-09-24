using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Balances
{
    [UsedImplicitly]
    public class CreditService : ICreditService
    {
        private readonly ICreditRepository _creditRepository;
        private readonly IBalanceOperationService _balanceOperationService;
        private readonly ILykkeExchangeService _lykkeExchangeService;
        private readonly ISettingsService _settingsService;
        private readonly InMemoryCache<Credit> _cache;
        private readonly ILog _log;

        public CreditService(
            ICreditRepository creditRepository,
            IBalanceOperationService balanceOperationService,
            ILykkeExchangeService lykkeExchangeService,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _creditRepository = creditRepository;
            _balanceOperationService = balanceOperationService;
            _lykkeExchangeService = lykkeExchangeService;
            _settingsService = settingsService;
            _cache = new InMemoryCache<Credit>(credit => credit.AssetId, false);
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<Credit>> GetAllAsync()
        {
            IReadOnlyCollection<Credit> credits = _cache.GetAll();

            if (credits == null)
            {
                credits = await _creditRepository.GetAllAsync();

                _cache.Initialize(credits);
            }

            return credits;
        }

        public async Task<Credit> GetByAssetIdAsync(string assetId)
        {
            IReadOnlyCollection<Credit> credits = await GetAllAsync();

            return credits.SingleOrDefault(o => o.AssetId == assetId) ?? new Credit {AssetId = assetId};
        }

        public async Task UpdateAsync(string assetId, decimal amount, string comment, string userId)
        {
            Credit credit = await GetByAssetIdAsync(assetId);

            credit.Add(amount);

            string walletId = await _settingsService.GetWalletIdAsync();

            if (amount > 0)
                await _lykkeExchangeService.CashInAsync(walletId, assetId, Math.Abs(amount), userId, comment);
            else
                await _lykkeExchangeService.CashOutAsync(walletId, assetId, Math.Abs(amount), userId, comment);

            await _creditRepository.InsertOrReplaceAsync(credit);

            var balanceOperation = new BalanceOperation
            {
                Time = DateTime.UtcNow,
                AssetId = assetId,
                Type = "Credit",
                Amount = amount,
                Comment = comment,
                UserId = userId
            };

            await _balanceOperationService.AddAsync(balanceOperation);

            _log.InfoWithDetails("Credit was updated", balanceOperation);
        }
    }
}
