using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.InstrumentMessages
{
    public class InstrumentMessagesService : IInstrumentMessagesService
    {
        private readonly IFiatEquityStopLossService _fiatEquityStopLossService;
        private readonly INoFreshQuotesStopLossService _noFreshQuotesStopLossService;
        private readonly IInstrumentService _instrumentService;

        public InstrumentMessagesService(
            IFiatEquityStopLossService fiatEquityStopLossService,
            INoFreshQuotesStopLossService noFreshQuotesStopLossService,
            IInstrumentService instrumentService)
        {
            _fiatEquityStopLossService = fiatEquityStopLossService;
            _noFreshQuotesStopLossService = noFreshQuotesStopLossService;
            _instrumentService = instrumentService;
        }

        public async Task<IReadOnlyCollection<Domain.InstrumentMessages>> GetAllAsync()
        {
            var result = new List<Domain.InstrumentMessages>();

            var instruments = await _instrumentService.GetAllAsync();

            var assetPairIds = instruments.Select(x => x.AssetPairId).ToList();

            foreach (var assetPairId in assetPairIds)
            {
                var fiatEquityStopLossMessages = await _fiatEquityStopLossService.GetMessagesAsync(assetPairId);

                var noFreshQuotesStopLossMessages = await _noFreshQuotesStopLossService.GetMessagesAsync(assetPairId);

                if (!fiatEquityStopLossMessages.Any() && !noFreshQuotesStopLossMessages.Any())
                    continue;

                List<string> instrumentMessages = new List<string>();

                instrumentMessages.AddRange(fiatEquityStopLossMessages);

                instrumentMessages.AddRange(noFreshQuotesStopLossMessages);

                result.Add(new Domain.InstrumentMessages
                {
                    AssetPairId = assetPairId,
                    Messages = instrumentMessages
                });
            }

            return result;
        }
    }
}
