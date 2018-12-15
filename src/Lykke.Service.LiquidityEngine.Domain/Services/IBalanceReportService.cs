using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IBalanceReportService
    {
        Task<IReadOnlyCollection<BalanceReport>> GetAsync();
    }
}
