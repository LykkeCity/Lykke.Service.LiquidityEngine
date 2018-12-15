using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IBalanceIndicatorsReportService
    {
        Task<BalanceIndicatorsReport> GetAsync();
    }
}
