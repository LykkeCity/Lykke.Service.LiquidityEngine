using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ISummaryReportService
    {
        Task<IReadOnlyCollection<SummaryReport>> GetAllAsync();

        Task<IReadOnlyCollection<SummaryReport>> GetByPeriodAsync(DateTime startDate, DateTime endDate);
        
        Task RegisterOpenPositionAsync(Position position, IReadOnlyCollection<InternalTrade> internalTrades);

        Task RegisterClosePositionAsync(Position position);
    }
}
