using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.MarketMaker;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class MarketMakerController : Controller, IMarketMakerApi
    {
        private readonly IMarketMakerStateService _marketMakerStateService;

        public MarketMakerController(IMarketMakerStateService marketMakerStateService)
        {
            _marketMakerStateService = marketMakerStateService;
        }

        /// <response code="200">The market maker state.</response>
        [HttpGet("state")]
        [ProducesResponseType(typeof(MarketMakerStateModel), (int) HttpStatusCode.OK)]
        public async Task<MarketMakerStateModel> GetStateAsync()
        {
            MarketMakerState state = await _marketMakerStateService.GetStateAsync();

            return Mapper.Map<MarketMakerStateModel>(state);
        }

        /// <response code="204">The market maker state successfully saved.</response>
        /// <response code="400">Comment required to change error status.</response>
        [HttpPost("state")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task SetStateAsync([FromBody] MarketMakerStateUpdateModel model)
        {
            var status = Mapper.Map<Domain.MarketMaker.MarketMakerStatus>(model.Status);

            MarketMakerState currentState = await _marketMakerStateService.GetStateAsync();

            if (status == currentState.Status)
                return;

            if (currentState.Status == Domain.MarketMaker.MarketMakerStatus.Error &&
                string.IsNullOrEmpty(model.Comment))
            {
                throw new ValidationApiException("Comment required.");
            }

            await _marketMakerStateService.SetStateAsync(status, model.Comment, model.UserId);
        }
    }
}
