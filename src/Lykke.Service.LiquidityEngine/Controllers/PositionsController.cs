using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class PositionsController : Controller, IPositionsApi
    {
        public PositionsController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<PositionModel>> GetAllAsync(DateTime startDate, DateTime endDate, int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet("open")]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<PositionModel>> GetOpenedAsync(string assetPairId)
        {
            throw new NotImplementedException();
        }
    }
}
