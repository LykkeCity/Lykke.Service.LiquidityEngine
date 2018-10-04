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
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Balances
{
    [UsedImplicitly]
    public class BalanceService : IBalanceService
    {
        private readonly ISettingsService _settingsService;
        private readonly IBalancesClient _balancesClient;
        private readonly InMemoryCache<Balance> _cache;
        private readonly ILog _log;

        public BalanceService(ISettingsService settingsService, IBalancesClient balancesClient, ILogFactory logFactory)
        {
            _settingsService = settingsService;
            _balancesClient = balancesClient;
            _cache = new InMemoryCache<Balance>(balance => balance.AssetId, true);
            _log = logFactory.CreateLog(this);
        }

        public Task<IReadOnlyCollection<Balance>> GetAllAsync()
        {
            return Task.FromResult(_cache.GetAll());
        }

        public Task<Balance> GetByAssetIdAsync(string assetId)
        {
            return Task.FromResult(_cache.Get(assetId) ?? new Balance(assetId, decimal.Zero));
        }

        public async Task UpdateAsync()
        {
            string walletId = await _settingsService.GetWalletIdAsync();

            if (string.IsNullOrEmpty(walletId))
                return;

            try
            {
                IEnumerable<ClientBalanceResponseModel> balances =
                    await _balancesClient.GetClientBalances(walletId);

                _cache.Set(balances.Select(o => new Balance(o.AssetId, o.Balance)).ToArray());
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while getting balances from Lykke exchange.");
            }
        }
    }
}
