using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Balances
{
    [UsedImplicitly]
    public class BalanceService : IBalanceService
    {
        private readonly ISettingsService _settingsService;
        private readonly IBalancesClient _balancesClient;
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly InMemoryCache<Balance> _cache;
        private readonly ILog _log;

        public BalanceService(
            ISettingsService settingsService,
            IBalancesClient balancesClient,
            IExternalExchangeService externalExchangeService,
            ILogFactory logFactory)
        {
            _settingsService = settingsService;
            _balancesClient = balancesClient;
            _externalExchangeService = externalExchangeService;
            _cache = new InMemoryCache<Balance>(GetKey, true);
            _log = logFactory.CreateLog(this);
        }

        public Task<IReadOnlyCollection<Balance>> GetAsync(string exchange)
        {
            Balance[] balances = _cache.GetAll()
                .Where(o => o.Exchange == exchange)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Balance>>(balances);
        }

        public Task<Balance> GetByAssetIdAsync(string exchange, string assetId)
        {
            Balance balance = _cache.Get(GetKey(assetId, exchange)) ?? new Balance(exchange, assetId, decimal.Zero);
            
            return Task.FromResult(balance);
        }

        public async Task UpdateLykkeBalancesAsync()
        {
            string walletId = await _settingsService.GetWalletIdAsync();

            if (string.IsNullOrEmpty(walletId))
                return;

            try
            {
                IEnumerable<ClientBalanceResponseModel> balances =
                    await _balancesClient.GetClientBalances(walletId);

                _cache.Set(balances.Select(o => new Balance(ExchangeNames.Lykke, o.AssetId, o.Balance)).ToArray());
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while getting balances from Lykke exchange.");
            }
        }

        public async Task UpdateExternalBalancesAsync()
        {
            try
            {
                IReadOnlyCollection<Balance> balances = await _externalExchangeService.GetBalancesAsync();

                _cache.Set(balances);
            }
            catch (ExternalExchangeThrottlingException exception)
            {
                _log.WarningWithDetails("Balance request was throttled", exception, new { });
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while getting balances from External exchange.");
            }
        }

        private static string GetKey(Balance balance)
            => GetKey(balance.AssetId, balance.Exchange);

        private static string GetKey(string assetId, string exchange)
            => $"{assetId}_{exchange}".ToUpper();
    }
}
