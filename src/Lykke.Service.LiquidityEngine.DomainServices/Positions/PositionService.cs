using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Positions
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IOpenPositionRepository _openPositionRepository;
        private readonly ILog _log;

        public PositionService(
            IPositionRepository positionRepository,
            IOpenPositionRepository openPositionRepository,
            ILogFactory logFactory)
        {
            _positionRepository = positionRepository;
            _openPositionRepository = openPositionRepository;
            _log = logFactory.CreateLog(this);
        }
        
        public Task<IReadOnlyCollection<Position>> GetAllAsync(DateTime startDate, DateTime endDate, int limit)
        {
            return _positionRepository.GetAsync(startDate, endDate, limit);
        }

        public Task<IReadOnlyCollection<Position>> GetOpenedAsync(string assetPairId)
        {
            return _openPositionRepository.GetByAssetPairIdAsync(assetPairId);
        }

        public async Task OpenPositionAsync(IReadOnlyCollection<InternalTrade> internalTrades)
        {
            if (internalTrades.Count == 0)
                return;

            Position position = Position.Open(internalTrades);

            await _openPositionRepository.InsertAsync(position);

            await _positionRepository.InsertAsync(position);

            _log.InfoWithDetails("Position was opened", position);
        }

        public async Task ClosePositionAsync(Position position, ExternalTrade externalTrade)
        {
            position.Close(externalTrade);
            
            await _positionRepository.UpdateAsync(position);

            await _openPositionRepository.DeleteAsync(position.AssetPairId, position.Id);
            
            _log.InfoWithDetails("Position was closed", position);
        }
    }
}
