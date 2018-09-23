using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ICreditService
    {
        Task<IReadOnlyCollection<Credit>> GetAllAsync();

        Task UpdateAsync(Credit credit);
    }
}
