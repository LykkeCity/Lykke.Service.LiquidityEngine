using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IPositionRepository
    {
        Task<IReadOnlyCollection<Position>> GetAsync(
            DateTime startDate, DateTime endDate, int limit, string assetPairId, string tradeAssetPairId);

        Task<Position> GetByIdAsync(string positionId);

        Task<IReadOnlyCollection<SummaryReport>> GetReportAsync(DateTime startDate, DateTime endDate);
        
        Task InsertAsync(Position position);

        Task UpdateAsync(Position position);

        Task DeleteAsync();

        Task DeleteAsync(string positionId);
    }
}
