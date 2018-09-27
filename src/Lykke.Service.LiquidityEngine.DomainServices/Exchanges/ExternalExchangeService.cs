using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.B2c2Client;
using Lykke.B2c2Client.Models.Rest;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Exchanges
{
    [UsedImplicitly]
    public class ExternalExchangeService : IExternalExchangeService
    {
        private readonly IB2С2RestClient _client;
        private readonly IAssetPairLinkService _assetPairLinkService;
        private readonly ILog _log;

        public ExternalExchangeService(
            IB2С2RestClient client,
            IAssetPairLinkService assetPairLinkService,
            ILogFactory logFactory)
        {
            _client = client;
            _assetPairLinkService = assetPairLinkService;
            _log = logFactory.CreateLog(this);
        }

        public Task<decimal> GetSellPriceAsync(string assetPairId, decimal volume)
            => GetPriceAsync(assetPairId, volume, Side.Sell);

        public Task<decimal> GetBuyPriceAsync(string assetPairId, decimal volume)
            => GetPriceAsync(assetPairId, volume, Side.Buy);

        public Task<ExternalTrade> ExecuteLimitOrderAsync(string assetPairId, decimal volume)
        {
            throw new NotImplementedException();
        }

        private async Task<decimal> GetPriceAsync(string assetPairId, decimal volume, Side side)
        {
            string instrument = await GetInstrumentAsync(assetPairId);

            var request = new RequestForQuoteRequest(instrument, side, volume);

            RequestForQuoteResponse response;
            
            try
            {
                response = await _client.RequestForQuoteAsync(request);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while getting price", request);
                throw;
            }

            return response.Price;
        }

        private async Task<string> GetInstrumentAsync(string assetPairId)
        {
            IReadOnlyCollection<AssetPairLink> assetPairLinks = await _assetPairLinkService.GetAllAsync();

            AssetPairLink assetPairLink = assetPairLinks.SingleOrDefault(o => o.AssetPairId == assetPairId);

            return assetPairLink != null ? assetPairLink.ExternalAssetPairId : assetPairId;
        }
    }
}
