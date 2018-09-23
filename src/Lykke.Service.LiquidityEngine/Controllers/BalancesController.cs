using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Balances;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class BalancesController : Controller, IBalancesApi
    {
        public BalancesController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of balances.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<AssetBalanceModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<AssetBalanceModel>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="200">The balance of asset.</response>
        [HttpGet("{assetId}")]
        [ProducesResponseType(typeof(AssetBalanceModel), (int) HttpStatusCode.OK)]
        public Task<AssetBalanceModel> GetByAssetIdAsync(string assetId)
        {
            throw new System.NotImplementedException();
        }
    }
}
