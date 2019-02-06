using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("spot")]
    public class SpotController : Controller, ISpotController
    {
        private readonly IInternalOrderService _internalOrderService;
        private readonly IBalanceService _balanceService;

        public SpotController(
            IInternalOrderService internalOrderService,
            IBalanceService balanceService)
        {
            _internalOrderService = internalOrderService;
            _balanceService = balanceService;
        }

        [HttpGet("getWallets")]
        public async Task<GetWalletsResponse> GetWalletBalancesAsync()
        {
            IReadOnlyCollection<Balance> balances = await _balanceService.GetAsync(ExchangeNames.Lykke);

            return new GetWalletsResponse
            {
                Wallets = balances.Select(o => new WalletBalanceModel
                {
                    Asset = o.AssetId,
                    Balance = o.Amount,
                    Reserved = 0
                }).ToList()
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetLimitOrders")]
        public Task<GetLimitOrdersResponse> GetLimitOrdersAsync()
        {
            throw new ValidationApiException(HttpStatusCode.NotImplemented, "Not supported");
        }

        [HttpGet("limitOrderStatus")]
        public async Task<OrderModel> LimitOrderStatusAsync(string orderId)
        {
            InternalOrder internalOrder = await _internalOrderService.GetByIdAsync(orderId);

            if (internalOrder == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Order not found");

            return Mapper.Map<OrderModel>(internalOrder);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("marketOrderStatus")]
        public Task<OrderModel> MarketOrderStatusAsync(string orderId)
        {
            throw new ValidationApiException(HttpStatusCode.NotImplemented, "Not supported");
        }

        [HttpPost("createLimitOrder")]
        public async Task<OrderIdResponse> CreateLimitOrderAsync([FromBody] LimitOrderRequest request)
        {
            try
            {
                string orderId = await _internalOrderService.CreateOrderAsync(GetWalletId(), request.Instrument,
                    request.TradeType == Common.ExchangeAdapter.Contracts.TradeType.Sell
                        ? LimitOrderType.Sell
                        : LimitOrderType.Buy,
                    request.Price, request.Volume, true);

                return new OrderIdResponse {OrderId = orderId};
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(exception.Message);
            }
        }

        [HttpPost("cancelOrder")]
        public async Task<CancelLimitOrderResponse> CancelLimitOrderAsync([FromBody] CancelLimitOrderRequest request)
        {
            InternalOrder internalOrder = await _internalOrderService.GetByIdAsync(request.OrderId);

            if (internalOrder == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Order not found");

            if (internalOrder.Status != InternalOrderStatus.Cancelled &&
                internalOrder.Status != InternalOrderStatus.Completed)
                throw new ValidationApiException(HttpStatusCode.BadRequest, "Can not cancel order");

            return new CancelLimitOrderResponse
            {
                OrderId = request.OrderId
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("replaceLimitOrder")]
        public Task<OrderIdResponse> ReplaceLimitOrderAsync([FromBody] ReplaceLimitOrderRequest request)
        {
            throw new ValidationApiException(HttpStatusCode.NotImplemented, "Not supported");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("createMarketOrder")]
        public Task<OrderIdResponse> CreateMarketOrderAsync([FromBody] MarketOrderRequest request)
        {
            throw new ValidationApiException(HttpStatusCode.NotImplemented, "Not supported");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("getOrdersHistory")]
        public Task<GetOrdersHistoryResponse> GetOrdersHistoryAsync()
        {
            throw new ValidationApiException(HttpStatusCode.NotImplemented, "Not supported");
        }

        private string GetWalletId()
        {
            if (!HttpContext.Request.Headers.TryGetValue(ClientTokenMiddleware.ClientTokenHeader, out var token))
                throw new ValidationApiException($"Unknown {ClientTokenMiddleware.ClientTokenHeader}");

            return token.ToString();
        }
    }
}
