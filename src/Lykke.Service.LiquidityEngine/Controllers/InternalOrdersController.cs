using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.InternalOrders;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class InternalOrdersController : Controller, IInternalOrdersApi
    {
        private readonly IInternalOrderService _internalOrderService;

        public InternalOrdersController(IInternalOrderService internalOrderService)
        {
            _internalOrderService = internalOrderService;
        }

        /// <inheritdoc/>
        /// <response code="200">An internal order.</response>
        [HttpGet("{internalOrderId}")]
        [ProducesResponseType(typeof(InternalOrderModel), (int) HttpStatusCode.OK)]
        public async Task<InternalOrderModel> GetByIdAsync(string internalOrderId)
        {
            InternalOrder internalOrder = await _internalOrderService.GetByIdAsync(internalOrderId);

            return Mapper.Map<InternalOrderModel>(internalOrder);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of internal orders.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<InternalOrderModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<InternalOrderModel>> GetAsync(DateTime startDate, DateTime endDate,
            int limit)
        {
            IReadOnlyCollection<InternalOrder> internalOrders =
                await _internalOrderService.GetByPeriodAsync(startDate, endDate, limit);

            return Mapper.Map<List<InternalOrderModel>>(internalOrders);
        }
    }
}
