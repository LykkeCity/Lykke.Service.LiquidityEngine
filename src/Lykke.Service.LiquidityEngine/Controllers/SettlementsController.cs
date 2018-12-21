using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Settlements;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
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
        /// <response code="404">No enough funds to cash out.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task SettlementAsync([FromBody] SettlementOperationModel model)
        {
            try
            {
                await _settlementService.ExecuteAsync(model.AssetId, model.Amount, model.Comment,
                    model.AllowChangeBalance, model.UserId);
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The fiat settlement successfully executed.</response>
        /// <response code="400">An error occurred while processing settlement.</response>
        /// <response code="404">Settlement trade not found.</response>
        [HttpPost("fiat")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task SettlementAsync([FromBody] FiatSettlementOperationModel model)
        {
            try
            {
                await _settlementService.ExecuteAsync(model.SettlementTradeId, model.UserId);
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Settlement trade not found.");
            }
        }
    }
}
