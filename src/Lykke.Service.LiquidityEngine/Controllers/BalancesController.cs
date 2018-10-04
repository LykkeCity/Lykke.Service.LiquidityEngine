using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Balances;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class BalancesController : Controller, IBalancesApi
    {
        private readonly IBalanceService _balanceService;
        private readonly ICreditService _creditService;

        public BalancesController(IBalanceService balanceService, ICreditService creditService)
        {
            _balanceService = balanceService;
            _creditService = creditService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of balances.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<AssetBalanceModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<AssetBalanceModel>> GetAllAsync()
        {
            IReadOnlyCollection<Balance> balances = await _balanceService.GetAllAsync();
            IReadOnlyCollection<Credit> credits = await _creditService.GetAllAsync();

            string[] assets = balances.Select(o => o.AssetId)
                .Union(credits.Select(o => o.AssetId))
                .ToArray();

            var model = new List<AssetBalanceModel>();

            foreach (string assetId in assets)
            {
                Balance balance = balances.SingleOrDefault(o => o.AssetId == assetId);
                Credit credit = credits.SingleOrDefault(o => o.AssetId == assetId);

                decimal balanceAmount = balance?.Amount ?? decimal.Zero;
                decimal creditAmount = credit?.Amount ?? decimal.Zero;

                model.Add(new AssetBalanceModel
                {
                    AssetId = assetId,
                    Amount = balanceAmount,
                    CreditAmount = creditAmount,
                    Disbalance = balanceAmount - creditAmount
                });
            }

            return model;
        }

        /// <inheritdoc/>
        /// <response code="200">The balance of asset.</response>
        [HttpGet("{assetId}")]
        [ProducesResponseType(typeof(AssetBalanceModel), (int) HttpStatusCode.OK)]
        public async Task<AssetBalanceModel> GetByAssetIdAsync(string assetId)
        {
            Balance balance = await _balanceService.GetByAssetIdAsync(assetId);
            Credit credit = await _creditService.GetByAssetIdAsync(assetId);

            decimal balanceAmount = balance?.Amount ?? decimal.Zero;
            decimal creditAmount = credit?.Amount ?? decimal.Zero;

            return new AssetBalanceModel
            {
                AssetId = assetId,
                Amount = balanceAmount,
                CreditAmount = creditAmount,
                Disbalance = balanceAmount - creditAmount
            };
        }
    }
}
