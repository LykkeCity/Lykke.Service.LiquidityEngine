using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Trades;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class TradesController : Controller, ITradesApi
    {
        private readonly ITradeService _tradeService;
        private readonly ISettlementTradeService _settlementTradeService;

        public TradesController(ITradeService tradeService, ISettlementTradeService settlementTradeService)
        {
            _tradeService = tradeService;
            _settlementTradeService = settlementTradeService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of external trades.</response>
        [HttpGet("external")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ExternalTradeModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<ExternalTradeModel>> GetExternalTradesAsync(DateTime startDate,
            DateTime endDate, int limit)
        {
            IReadOnlyCollection<ExternalTrade> externalTrades =
                await _tradeService.GetExternalTradesAsync(startDate, endDate, limit);

            return Mapper.Map<ExternalTradeModel[]>(externalTrades);
        }

        /// <inheritdoc/>
        /// <response code="200">An external trade.</response>
        /// <response code="404">External trade does not exist.</response>
        [HttpGet("external/{tradeId}")]
        [ProducesResponseType(typeof(ExternalTradeModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<ExternalTradeModel> GetExternalTradeByIdAsync(string tradeId)
        {
            ExternalTrade externalTrade = await _tradeService.GetExternalTradeByIdAsync(tradeId);

            if (externalTrade == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "External trade does not exist.");

            return Mapper.Map<ExternalTradeModel>(externalTrade);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of internal trades.</response>
        [HttpGet("internal")]
        [ProducesResponseType(typeof(IReadOnlyCollection<InternalTradeModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<InternalTradeModel>> GetInternalTradesAsync(DateTime startDate,
            DateTime endDate, int limit)
        {
            IReadOnlyCollection<InternalTrade> internalTrades =
                await _tradeService.GetInternalTradesAsync(startDate, endDate, limit);

            return Mapper.Map<InternalTradeModel[]>(internalTrades);
        }

        /// <inheritdoc/>
        /// <response code="200">An internal trade.</response>
        /// <response code="404">Internal trade does not exist.</response>
        [HttpGet("internal/{tradeId}")]
        [ProducesResponseType(typeof(InternalTradeModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<InternalTradeModel> GetInternalTradeByIdAsync(string tradeId)
        {
            InternalTrade internalTrade = await _tradeService.GetInternalTradeByIdAsync(tradeId);

            if (internalTrade == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Internal trade does not exist.");

            return Mapper.Map<InternalTradeModel>(internalTrade);
        }

        /// <inheritdoc/>
        /// <response code="200">An internal trade.</response>
        /// <response code="404">Asset pair does not exist.</response>
        [HttpGet("internal/{assetPairId}/time")]
        [ProducesResponseType(typeof(InternalTradeModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public Task<LastInternalTradeTimeModel> GetLastInternalTradeTimeAsync(string assetPairId)
        {
            DateTime lastInternalTradeTime = _tradeService.GetLastInternalTradeTime(assetPairId);

            return Task.FromResult(
                new LastInternalTradeTimeModel
                {
                    AssetPairId = assetPairId,
                    LastInternalTradeTime = lastInternalTradeTime
                });
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of settlement trades.</response>
        [HttpGet("settlement")]
        [ProducesResponseType(typeof(IReadOnlyCollection<SettlementTradeModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<SettlementTradeModel>> GetSettlementTradesAsync()
        {
            IReadOnlyCollection<SettlementTrade> settlementTrades =
                await _settlementTradeService.GetAllAsync();

            return Mapper.Map<SettlementTradeModel[]>(settlementTrades);
        }
    }
}
