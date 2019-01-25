using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Handlers
{
    public interface IClosedPositionHandler
    {
        Task HandleClosedPositionAsync(Position position);
    }
}
