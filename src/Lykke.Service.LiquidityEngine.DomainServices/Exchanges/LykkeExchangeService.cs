using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Service.ExchangeOperations.Client.AutorestClient.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Extensions;
using Lykke.Service.LiquidityEngine.DomainServices.Utils;

namespace Lykke.Service.LiquidityEngine.DomainServices.Exchanges
{
    [UsedImplicitly]
    public class LykkeExchangeService : ILykkeExchangeService
    {
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly IExchangeOperationsServiceClient _exchangeOperationsServiceClient;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public LykkeExchangeService(
            IMatchingEngineClient matchingEngineClient,
            IExchangeOperationsServiceClient exchangeOperationsServiceClient,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _matchingEngineClient = matchingEngineClient;
            _exchangeOperationsServiceClient = exchangeOperationsServiceClient;
            _settingsService = settingsService;
            _log = logFactory.CreateLog(this);
        }

        public async Task ApplyAsync(string assetPairId, IReadOnlyCollection<LimitOrder> limitOrders)
        {
            var totalOrderPlacementStartedAt = DateTime.UtcNow;

            string walletId = await _settingsService.GetWalletIdAsync();

            if (string.IsNullOrEmpty(walletId))
                throw new Exception("WalletId is not set");

            var map = new Dictionary<string, string>();

            var multiOrderItems = new List<MultiOrderItemModel>();

            foreach (LimitOrder limitOrder in limitOrders)
            {
                var multiOrderItem = new MultiOrderItemModel
                {
                    Id = Guid.NewGuid().ToString("D"),
                    OrderAction = limitOrder.Type.ToOrderAction(),
                    Price = (double) limitOrder.Price,
                    Volume = (double) Math.Abs(limitOrder.Volume)
                };

                multiOrderItems.Add(multiOrderItem);

                map[multiOrderItem.Id] = limitOrder.Id;
            }

            var multiLimitOrder = new MultiLimitOrderModel
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = walletId,
                AssetPairId = assetPairId,
                CancelPreviousOrders = true,
                Orders = multiOrderItems,
                CancelMode = CancelMode.BothSides
            };

            _log.InfoWithDetails("ME place multi limit order request", multiLimitOrder);

            MultiLimitOrderResponse response;

            try
            {
                var startedAt = DateTime.UtcNow;

                response = await _matchingEngineClient.PlaceMultiLimitOrderAsync(multiLimitOrder);

                var finishedAt = DateTime.UtcNow;

                _log.Info("_matchingEngineClient.PlaceMultiLimitOrderAsync() finished.", new
                {
                    AssetPairId = assetPairId,
                    StartedAt = finishedAt,
                    FinishedAt = finishedAt,
                    Latency = (finishedAt - startedAt).TotalMilliseconds
                });
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred during creating limit orders", multiLimitOrder);

                throw;
            }

            if (response == null)
                throw new Exception("ME response is null");

            foreach (LimitOrderStatusModel orderStatus in response.Statuses)
            {
                if (map.TryGetValue(orderStatus.Id, out var limitOrderId))
                {
                    var limitOrder = limitOrders.Single(e => e.Id == limitOrderId);

                    limitOrder.Error = orderStatus.Status.ToOrderError();
                    limitOrder.ErrorMessage = limitOrder.Error != LimitOrderError.Unknown
                        ? orderStatus.StatusReason
                        : !string.IsNullOrEmpty(orderStatus.StatusReason)
                            ? orderStatus.StatusReason
                            : "Unknown error";
                }
                else
                {
                    _log.Warning("ME returned status for order which is not in the request",
                        context: $"order: {orderStatus.Id}");
                }
            }

            string[] ignoredOrdersByMe = response.Statuses
                .Select(x => x.Id)
                .Except(multiLimitOrder.Orders.Select(x => x.Id))
                .ToArray();

            if (ignoredOrdersByMe.Any())
            {
                _log.WarningWithDetails("ME didn't return status for orders",
                    $"pair: {assetPairId}, orders: {string.Join(", ", ignoredOrdersByMe)}");
            }

            var totalOrderPlacementFinishedAt = DateTime.UtcNow;

            _log.Info("LykkeExchangeService.ApplyAsync() finished.", new
            {
                AssetPairId = assetPairId,
                StartedAt = totalOrderPlacementStartedAt,
                FinishedAt = totalOrderPlacementFinishedAt,
                Latency = (totalOrderPlacementFinishedAt - totalOrderPlacementStartedAt).TotalMilliseconds
            });

            _log.InfoWithDetails("ME place multi limit order response", response);
        }

        public async Task<string> CashInAsync(string clientId, string assetId, decimal amount, string userId,
            string comment)
        {
            _log.InfoWithDetails("Cash in request", new CashInContext(clientId, assetId, amount, userId));

            ExchangeOperationResult result;

            try
            {
                result = await RetriesWrapper.RunWithRetriesAsync(() => _exchangeOperationsServiceClient
                    .ManualCashInAsync(clientId, assetId, (double) amount, userId, comment));
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, new CashInContext(clientId, assetId, amount, userId));

                throw new Exception("An error occurred while processing cash in request", exception);
            }

            _log.InfoWithDetails("Cash in response", result);

            if (!result.IsOk())
                throw new BalanceOperationException("Unexpected cash in response status");

            return result.TransactionId;
        }

        public async Task<string> CashOutAsync(string clientId, string assetId, decimal amount, string userId,
            string comment)
        {
            _log.InfoWithDetails("Cash out request", new CashOutContext(clientId, assetId, amount));

            ExchangeOperationResult result;

            try
            {
                result = await RetriesWrapper.RunWithRetriesAsync(() => _exchangeOperationsServiceClient
                    .ManualCashOutAsync(clientId, "empty", (double) amount, assetId, comment: comment, userId: userId));
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, new CashOutContext(clientId, assetId, amount));

                throw new Exception("An error occurred while processing cash out request", exception);
            }

            _log.InfoWithDetails("Cash out response", result);

            if (!result.IsOk())
            {
                if (result.Code == (int) MeStatusCodes.LowBalance)
                    throw new NotEnoughFundsException();

                throw new BalanceOperationException("Unexpected cash out response status");
            }

            return result.TransactionId;
        }

        public async Task<string> TransferAsync(string sourceWalletId, string destinationWalletId, string assetId,
            decimal amount, string transactionId = null)
        {
            _log.InfoWithDetails("Transfer request",
                new {sourceWalletId, destinationWalletId, assetId, amount, transactionId});

            ExchangeOperationResult result;

            try
            {
                result = await _exchangeOperationsServiceClient.TransferAsync(
                    destinationWalletId,
                    sourceWalletId,
                    (double) amount,
                    assetId,
                    transactionId: transactionId);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception,
                    new {sourceWalletId, destinationWalletId, assetId, amount, transactionId});

                throw new Exception("An error occurred while processing transfer request", exception);
            }

            if (result == null)
                throw new Exception("ME result is null");

            _log.InfoWithDetails("Transfer response", result);

            if (!result.IsOk())
            {
                if (result.Code == (int) MeStatusCodes.LowBalance)
                    throw new NotEnoughFundsException();

                if (result.Code == (int) MeStatusCodes.Duplicate)
                    throw new DuplicateTransferException();
                
                throw new BalanceOperationException("Unexpected transfer response status");
            }

            return result.TransactionId;
        }
    }
}
