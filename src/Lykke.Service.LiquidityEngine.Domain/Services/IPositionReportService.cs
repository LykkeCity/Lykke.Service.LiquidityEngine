using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IPositionReportService
    {
        Task<IReadOnlyCollection<PositionReport>> GetByPeriodAsync(DateTime startDate, DateTime endDate, int limit);
    }
}
