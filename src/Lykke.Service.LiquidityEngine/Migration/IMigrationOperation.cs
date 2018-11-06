using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Migration
{
    public interface IMigrationOperation
    {
        Task ApplyAsync();
    }
}
