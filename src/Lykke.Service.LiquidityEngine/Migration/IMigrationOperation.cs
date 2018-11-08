using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Migration
{
    public interface IMigrationOperation
    {
        int ApplyToVersion { get; }

        int UpdatedVersion { get; }

        Task ApplyAsync();
    }
}
