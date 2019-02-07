using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.InternalExchange.Client.Api;
using Lykke.Common.InternalExchange.Client.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class InternalTraderController : Controller, IInternalTraderApi
    {
        private readonly IInternalOrderService _internalOrderService;

        public InternalTraderController(IInternalOrderService internalOrderService)
        {
            _internalOrderService = internalOrderService;
        }

        /// <inheritdoc/>
        /// <response code="200">The model that describes an order.</response>
        /// <response code="404">The order not found by identifier.</response>
        [HttpGet("Orders/{orderId}")]
        [ProducesResponseType(typeof(OrderModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<OrderModel> GetOrderAsync(string orderId)
        {
            InternalOrder internalOrder = await _internalOrderService.GetByIdAsync(orderId);

            if (internalOrder == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Order not found");

            return Mapper.Map<OrderModel>(internalOrder);
        }

        /// <inheritdoc/>
        /// <response code="204">The order successfully created.</response>
        /// <response code="400">An unexpected error occurred while creating order.</response>
        [HttpPost("Orders")]
        [ProducesResponseType(typeof(CreateOrderResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<CreateOrderResponse> CreateOrderAsync([FromBody] CreateOrderRequest model)
        {
            try
            {
                string orderId = await _internalOrderService.CreateOrderAsync(model.WalletId, model.AssetPair,
                    model.Type == OrderType.Sell ? LimitOrderType.Sell : LimitOrderType.Buy,
                    model.Price, model.Volume, model.FullExecution);

                return new CreateOrderResponse {OrderId = orderId};
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(exception.Message);
            }
        }
    }
}
