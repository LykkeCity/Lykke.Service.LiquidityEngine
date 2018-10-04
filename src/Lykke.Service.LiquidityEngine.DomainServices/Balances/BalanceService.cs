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
using Lykke.Service.LiquidityEngine.Domain.Cache;
using Lykke.Service.LiquidityEngine.Domain.Consts;
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
        private readonly IBalanceCache _cache;
        private readonly ILog _log;

        public BalanceService(
            ISettingsService settingsService,
            IBalancesClient balancesClient,
            IExternalExchangeService externalExchangeService,
            IBalanceCache cache,
            ILogFactory logFactory)
        {
            _settingsService = settingsService;
            _balancesClient = balancesClient;
            _externalExchangeService = externalExchangeService;
            _cache = cache;
            _log = logFactory.CreateLog(this);
        }

        public Task<IReadOnlyCollection<Balance>> GetAsync(string exchange)
        {
            return Task.FromResult(_cache.Get(exchange));
        }

        public Task<Balance> GetByAssetIdAsync(string exchange, string assetId)
        {
            return Task.FromResult(_cache.Get(exchange, assetId) ?? new Balance(exchange, assetId, decimal.Zero));
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
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while getting balances from External exchange.");
            }
        }
    }
}
