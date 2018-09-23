using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Balances
{
    public class CreditService : ICreditService
    {
        private readonly ICreditRepository _creditRepository;
        private readonly IBalanceOperationService _balanceOperationService;
        private readonly InMemoryCache<Credit> _cache;
        private readonly ILog _log;

        public CreditService(
            ICreditRepository creditRepository,
            IBalanceOperationService balanceOperationService,
            ILogFactory logFactory)
        {
            _creditRepository = creditRepository;
            _balanceOperationService = balanceOperationService;
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

        public async Task UpdateAsync(Credit credit)
        {
            IReadOnlyCollection<Credit> credits = await GetAllAsync();

            Credit currentCredit = credits.SingleOrDefault(o => o.AssetId == credit.AssetId) ??
                                   new Credit {AssetId = credit.AssetId};
            
            currentCredit.Add(credit.Amount);
            
            // TODO: Manual cash-in/out

            await _creditRepository.InsertOrReplaceAsync(currentCredit);

            await _balanceOperationService.AddAsync(new BalanceOperation
            {
                Time = DateTime.UtcNow,
                AssetId = credit.AssetId,
                Type = "Credit",
                Amount = credit.Amount,
                Comment = null,
                UserId = null
            });
            
            // TODO: write log
        }
    }
}
