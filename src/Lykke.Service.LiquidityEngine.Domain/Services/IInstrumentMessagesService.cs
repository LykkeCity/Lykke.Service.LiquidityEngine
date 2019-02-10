using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IInstrumentMessagesService
    {
        Task<IReadOnlyCollection<InstrumentMessages>> GetAllAsync();
    }
}
