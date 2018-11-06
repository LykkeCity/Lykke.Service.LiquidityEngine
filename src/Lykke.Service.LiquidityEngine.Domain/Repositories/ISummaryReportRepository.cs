using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface ISummaryReportRepository
    {
        Task<IReadOnlyCollection<SummaryReport>> GetAllAsync();

        Task<SummaryReport> GetByAssetPairAsync(string assetPairId, string tradeAssetPairId);

        Task InsertAsync(SummaryReport summaryReport);

        Task UpdateAsync(SummaryReport summaryReport);

        Task DeleteAsync();
        
        Task DeleteAsync(string assetPairId, string tradeAssetPairId);
    }
}
