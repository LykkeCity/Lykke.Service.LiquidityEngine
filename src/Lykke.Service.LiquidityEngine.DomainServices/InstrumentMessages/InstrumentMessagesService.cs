using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.InstrumentMessages
{
    public class InstrumentMessagesService : IInstrumentMessagesService
    {
        private readonly IFiatEquityStopLossService _fiatEquityStopLossService;
        private readonly IInstrumentService _instrumentService;

        public InstrumentMessagesService(
            IFiatEquityStopLossService fiatEquityStopLossService,
            IInstrumentService instrumentService)
        {
            _fiatEquityStopLossService = fiatEquityStopLossService;
            _instrumentService = instrumentService;
        }

        public async Task<IReadOnlyCollection<Domain.InstrumentMessages>> GetAllAsync()
        {
            var result = new List<Domain.InstrumentMessages>();

            var instruments = await _instrumentService.GetAllAsync();

            var assetPairIds = instruments.Select(x => x.AssetPairId).ToList();

            foreach (var assetPairId in assetPairIds)
            {
                var messages = await _fiatEquityStopLossService.GetMessages(assetPairId);

                if (!messages.Any())
                    continue;

                result.Add(new Domain.InstrumentMessages
                {
                    AssetPairId = assetPairId,
                    Messages = messages
                });
            }

            return result;
        }
    }
}
