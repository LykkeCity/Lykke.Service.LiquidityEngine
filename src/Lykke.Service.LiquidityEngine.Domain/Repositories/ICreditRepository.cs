using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface ICreditRepository
    {
        Task<IReadOnlyCollection<Credit>> GetAllAsync();

        Task InsertOrReplaceAsync(Credit credit);
    }
}
