using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IVersionControlRepository
    {
        Task<SystemVersion> GetAsync();

        Task UpdateAsync(SystemVersion systemVersion);
    }
}
