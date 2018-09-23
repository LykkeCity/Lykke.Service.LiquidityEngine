using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Audit
{
    [UsedImplicitly]
    public class BalanceOperationService : IBalanceOperationService
    {
        private readonly IBalanceOperationRepository _balanceOperationRepository;

        public BalanceOperationService(IBalanceOperationRepository balanceOperationRepository)
        {
            _balanceOperationRepository = balanceOperationRepository;
        }
        
        public Task<IReadOnlyCollection<BalanceOperation>> GetAsync(DateTime startDate, DateTime endDate, int limit)
        {
            return _balanceOperationRepository.GetAsync(startDate, endDate, limit);
        }

        public Task AddAsync(BalanceOperation balanceOperation)
        {
            return _balanceOperationRepository.InsertAsync(balanceOperation);
        }
    }
}
