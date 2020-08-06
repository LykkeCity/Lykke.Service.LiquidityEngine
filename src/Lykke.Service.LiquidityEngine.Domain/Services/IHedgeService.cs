using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IHedgeService
    {
        void Start();

        void Stop();

        Task ExecuteAsync();
    }
}
