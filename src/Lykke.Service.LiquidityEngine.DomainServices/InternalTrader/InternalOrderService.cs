using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.InternalTrader
{
    public class InternalOrderService : IInternalOrderService
    {
        private readonly IInternalOrderRepository _internalOrderRepository;

        public InternalOrderService(IInternalOrderRepository internalOrderRepository)
        {
            _internalOrderRepository = internalOrderRepository;
        }

        public Task<InternalOrder> GetByIdAsync(string internalOrderId)
        {
            return _internalOrderRepository.GetByIdAsync(internalOrderId);
        }

        public Task<IReadOnlyCollection<InternalOrder>> GetByPeriodAsync(DateTime startDate, DateTime endDate,
            int limit)
        {
            return _internalOrderRepository.GetByPeriodAsync(startDate, endDate, limit);
        }

        public async Task<string> CreateOrderAsync(string walletId, string assetPairId, LimitOrderType type,
            decimal price, decimal volume, bool fullExecution)
        {
            var internalOrder = new InternalOrder(walletId, assetPairId, type, price, volume, fullExecution);

            await _internalOrderRepository.InsertAsync(internalOrder);

            return internalOrder.Id;
        }
    }
}
