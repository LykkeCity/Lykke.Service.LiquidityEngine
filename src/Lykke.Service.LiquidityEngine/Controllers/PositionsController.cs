using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class PositionsController : Controller, IPositionsApi
    {
        private readonly IPositionService _positionService;

        public PositionsController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionModel>> GetAllAsync(
            DateTime startDate, DateTime endDate, int limit, string assetPairId, string tradeAssetPairId)
        {
            IReadOnlyCollection<Position> positions =
                await _positionService.GetAllAsync(startDate, endDate, limit, assetPairId, tradeAssetPairId);

            return Mapper.Map<PositionModel[]>(positions);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet("open")]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionModel>> GetOpenAllAsync()
        {
            IReadOnlyCollection<Position> positions = await _positionService.GetOpenAllAsync();

            return Mapper.Map<PositionModel[]>(positions);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet("open/{assetPairId}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionModel>> GetOpenByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<Position> positions = await _positionService.GetOpenByAssetPairIdAsync(assetPairId);

            return Mapper.Map<PositionModel[]>(positions);
        }
    }
}
