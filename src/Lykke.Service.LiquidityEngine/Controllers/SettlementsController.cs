using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Settlements;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class SettlementsController : Controller, ISettlementsApi
    {
        private readonly ISettlementService _settlementService;

        public SettlementsController(ISettlementService settlementService)
        {
            _settlementService = settlementService;
        }

        /// <inheritdoc/>
        /// <response code="204">The settlement successfully executed.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SettlementAsync([FromBody] SettlementOperationModel model)
        {
            await _settlementService.ExecuteAsync(model.AssetId, model.Amount, model.Comment, model.UserId);
        }
    }
}
