using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Service.ExchangeOperations.Client.AutorestClient.Models;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Utils;

namespace Lykke.Service.LiquidityEngine.DomainServices.Exchanges
{
    [UsedImplicitly]
    public class LykkeExchangeService : ILykkeExchangeService
    {
        private readonly IExchangeOperationsServiceClient _exchangeOperationsServiceClient;
        private readonly ILog _log;

        public LykkeExchangeService(
            IExchangeOperationsServiceClient exchangeOperationsServiceClient,
            ILogFactory logFactory)
        {
            _exchangeOperationsServiceClient = exchangeOperationsServiceClient;
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<string> CashInAsync(string clientId, string assetId, decimal amount, string userId, string comment)
        {
            _log.InfoWithDetails("Cash in request", new CashInContext(clientId, assetId, amount, userId));

            ExchangeOperationResult result;

            try
            {
                result = await RetriesWrapper.RunWithRetriesAsync(() => _exchangeOperationsServiceClient
                    .ManualCashInAsync(clientId, assetId, (double) amount, userId, comment));
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, new CashInContext(clientId, assetId, amount, userId));
                
                throw new Exception("An error occurred while processing cash in request", exception);
            }

            _log.InfoWithDetails("Cash in response", result);

            if (result.Code != 0)
                throw new Exception($"Unexpected cash in response status '{result.Code}'");

            return result.TransactionId;
        }

        public async Task<string> CashOutAsync(string clientId, string assetId, decimal amount, string userId, string comment)
        {
            _log.InfoWithDetails("Cash out request", new CashOutContext(clientId, assetId, amount));

            ExchangeOperationResult result;

            try
            {
                result = await RetriesWrapper.RunWithRetriesAsync(() => _exchangeOperationsServiceClient
                    .ManualCashOutAsync(clientId, "empty", (double) amount, assetId, comment: comment, userId: userId));
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, new CashOutContext(clientId, assetId, amount));
                
                throw new Exception("An error occurred while processing cash out request", exception);
            }

            _log.InfoWithDetails("Cash out response", result);

            if (result.Code != 0)
                throw new Exception($"Unexpected cash out response status '{result.Code}'");
            
            return result.TransactionId;
        }
    }
}
