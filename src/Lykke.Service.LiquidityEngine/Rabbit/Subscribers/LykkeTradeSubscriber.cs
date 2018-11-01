using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.MatchingEngine.Connector.Models.RabbitMq;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Subscribers
{
    [UsedImplicitly]
    public class LykkeTradeSubscriber : IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly ISettingsService _settingsService;
        private readonly ITradeService _tradeService;
        private readonly IPositionService _positionService;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private RabbitMqSubscriber<LimitOrders> _subscriber;

        public LykkeTradeSubscriber(
            SubscriberSettings settings,
            ISettingsService settingsService,
            ITradeService tradeService,
            IPositionService positionService,
            ILogFactory logFactory)
        {
            _settings = settings;
            _settingsService = settingsService;
            _tradeService = tradeService;
            _positionService = positionService;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.Queue)
                .MakeDurable();

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<LimitOrders>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<LimitOrders>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private async Task ProcessMessageAsync(LimitOrders limitOrders)
        {
            try
            {
                if (limitOrders.Orders == null || limitOrders.Orders.Count == 0)
                    return;

                string walletId = await _settingsService.GetWalletIdAsync();

                if (string.IsNullOrEmpty(walletId))
                    return;

                IEnumerable<LimitOrderWithTrades> clientLimitOrders = limitOrders.Orders
                    .Where(o => o.Order?.ClientId == walletId)
                    .Where(o => o.Trades?.Count > 0);

                IReadOnlyCollection<InternalTrade> trades = CreateReports(clientLimitOrders);

                if (trades.Any())
                {
                    await _tradeService.RegisterAsync(trades);
                    await _positionService.OpenAsync(trades);

                    _log.InfoWithDetails("Traders were handled", clientLimitOrders);
                }
            }
            catch (Exception exception)
            {
                _log.Error(exception, "An error occurred during processing trades", limitOrders);
            }
        }

        private static IReadOnlyCollection<InternalTrade> CreateReports(IEnumerable<LimitOrderWithTrades> limitOrders)
        {
            var executionReports = new List<InternalTrade>();

            foreach (LimitOrderWithTrades limitOrderModel in limitOrders)
            {
                // The limit order fully executed. The remaining volume is zero.
                if (limitOrderModel.Order.Status == OrderStatus.Matched)
                {
                    IReadOnlyList<InternalTrade> orderExecutionReports =
                        CreateInternalTrades(limitOrderModel.Order, limitOrderModel.Trades, true);

                    executionReports.AddRange(orderExecutionReports);
                }

                // The limit order partially executed.
                if (limitOrderModel.Order.Status == OrderStatus.Processing)
                {
                    IReadOnlyList<InternalTrade> orderExecutionReports =
                        CreateInternalTrades(limitOrderModel.Order, limitOrderModel.Trades, false);

                    executionReports.AddRange(orderExecutionReports);
                }

                // The limit order was cancelled by matching engine after processing trades.
                // In this case order partially executed and remaining volume is less than min volume allowed by asset pair.
                if (limitOrderModel.Order.Status == OrderStatus.Cancelled)
                {
                    IReadOnlyList<InternalTrade> orderExecutionReports =
                        CreateInternalTrades(limitOrderModel.Order, limitOrderModel.Trades, true);

                    executionReports.AddRange(orderExecutionReports);
                }
            }

            return executionReports;
        }

        private static IReadOnlyList<InternalTrade> CreateInternalTrades(
            MatchingEngine.Connector.Models.RabbitMq.LimitOrder limitOrder,
            IReadOnlyList<LimitTradeInfo> trades,
            bool completed)
        {
            var reports = new List<InternalTrade>();

            for (int i = 0; i < trades.Count; i++)
            {
                LimitTradeInfo trade = trades[i];

                TradeType tradeType = limitOrder.Volume < 0
                    ? TradeType.Sell
                    : TradeType.Buy;

                TradeStatus executionStatus = i == trades.Count - 1 && completed
                    ? TradeStatus.Fill
                    : TradeStatus.PartialFill;

                reports.Add(new InternalTrade
                {
                    Id = trade.TradeId,
                    AssetPairId = limitOrder.AssetPairId,
                    ExchangeOrderId = limitOrder.Id,
                    LimitOrderId = limitOrder.ExternalId,
                    Status = executionStatus,
                    Type = tradeType,
                    Time = trade.Timestamp,
                    Price = (decimal) trade.Price,
                    Volume = tradeType == TradeType.Buy
                        ? (decimal) trade.OppositeVolume
                        : (decimal) trade.Volume,
                    OppositeClientId = trade.OppositeClientId,
                    OppositeLimitOrderId = trade.OppositeOrderId,
                    OppositeSideVolume = tradeType == TradeType.Buy
                        ? (decimal) trade.Volume
                        : (decimal) trade.OppositeVolume,
                    RemainingVolume = (decimal) limitOrder.RemainingVolume
                });
            }

            return reports;
        }
    }
}
