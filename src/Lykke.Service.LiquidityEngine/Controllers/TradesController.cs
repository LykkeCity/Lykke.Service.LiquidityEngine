using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Trades;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class TradesController : Controller, ITradesApi
    {
        public TradesController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of external trades.</response>
        [HttpGet("external")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ExternalTradeModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<ExternalTradeModel>> GetExternalTradesAsync(DateTime startDate, DateTime endDate, int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="200">An external trade.</response>
        [HttpGet("external/{tradeId}")]
        [ProducesResponseType(typeof(ExternalTradeModel), (int) HttpStatusCode.OK)]
        public Task<ExternalTradeModel> GetExternalTradeByIdAsync(string tradeId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of internal trades.</response>
        [HttpGet("internal")]
        [ProducesResponseType(typeof(IReadOnlyCollection<InternalTradeModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<InternalTradeModel>> GetInternalTradesAsync(DateTime startDate, DateTime endDate, int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="200">An internal trade.</response>
        [HttpGet("internal/{tradeId}")]
        [ProducesResponseType(typeof(InternalTradeModel), (int) HttpStatusCode.OK)]
        public Task<InternalTradeModel> GetInternalTradeByIdAsync(string tradeId)
        {
            throw new NotImplementedException();
        }
    }
}
