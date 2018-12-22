using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.MatchingEngine.Connector.Models.Events;
using Lykke.MatchingEngine.Connector.Models.Events.Common;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Deduplication;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Utils;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Subscribers
{
    [UsedImplicitly]
    public class LykkeTradeSubscriber : IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly ISettingsService _settingsService;
        private readonly IPositionService _positionService;
        private readonly IDeduplicator _deduplicator;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private IStopable _subscriber;

        public LykkeTradeSubscriber(
            SubscriberSettings settings,
            ISettingsService settingsService,
            IPositionService positionService,
            IDeduplicator deduplicator,
            ILogFactory logFactory)
        {
            _settings = settings;
            _settingsService = settingsService;
            _positionService = positionService;
            _deduplicator = deduplicator;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }

        public DateTime LastMessageTime { get; private set; }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.Queue)
                .UseRoutingKey(((int) MessageType.Order).ToString())
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<ExecutionEvent>(
                    _logFactory, settings, new ResilientErrorHandlingStrategy(_logFactory, settings,
                        TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                .SetMessageDeserializer(new ProtobufMessageDeserializer<ExecutionEvent>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                //.SetAlternativeExchange(_settings.AlternateConnectionString)
                //.SetDeduplicator(_deduplicator)
                .Start();

            LastMessageTime = DateTime.UtcNow;
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private async Task ProcessMessageAsync(ExecutionEvent message)
        {
            try
            {
                LastMessageTime = DateTime.UtcNow;

                if (message.Header.MessageType != MessageType.Order)
                    return;

                string walletId = await _settingsService.GetWalletIdAsync();

                Order[] orders = message.Orders
                    .Where(o => o.WalletId == walletId)
                    .Where(o => o.Side != OrderSide.UnknownOrderSide)
                    .Where(o => o.Trades?.Count > 0)
                    .ToArray();

                _log.InfoWithDetails("Trades received", new {Count = orders.Length});

                if (orders.Any())
                {
                    await TraceWrapper.TraceExecutionTimeAsync("Trades handling", () => ExecuteAsync(orders), _log);

                    _log.InfoWithDetails("Trades handled", orders);
                }
            }
            catch (Exception exception)
            {
                _log.Error(exception, "An error occurred while processing trades", message);
                throw;
            }
        }

        private async Task ExecuteAsync(Order[] orders)
        {
            var internalTrades = new List<InternalTrade>();

            foreach (Order order in orders)
            {
                // The limit order fully executed. The remaining volume is zero.
                if (order.Status == OrderStatus.Matched)
                    internalTrades.AddRange(Map(order, true));

                // The limit order partially executed.
                if (order.Status == OrderStatus.PartiallyMatched)
                    internalTrades.AddRange(Map(order, false));

                // The limit order was cancelled by matching engine after processing trades.
                // In this case order partially executed and remaining volume is less than min volume allowed by asset pair.
                if (order.Status == OrderStatus.Cancelled)
                    internalTrades.AddRange(Map(order, true));
            }

            Task processTask = _positionService.OpenAsync(internalTrades);
            Task delayTask = Task.Delay(TimeSpan.FromMinutes(1));

            Task task = await Task.WhenAny(processTask, delayTask);

            if (task == delayTask)
                _log.WarningWithDetails("Trades processing takes more than one minute", internalTrades);

            await processTask;
        }

        private static IReadOnlyList<InternalTrade> Map(Order order, bool completed)
        {
            var reports = new List<InternalTrade>();

            for (int i = 0; i < order.Trades.Count; i++)
            {
                Trade trade = order.Trades[i];

                TradeType tradeType = order.Side == OrderSide.Sell
                    ? TradeType.Sell
                    : TradeType.Buy;

                TradeStatus executionStatus = i == order.Trades.Count - 1 && completed
                    ? TradeStatus.Fill
                    : TradeStatus.PartialFill;

                reports.Add(new InternalTrade
                {
                    Id = trade.TradeId,
                    AssetPairId = order.AssetPairId,
                    ExchangeOrderId = order.Id,
                    LimitOrderId = order.ExternalId,
                    Status = executionStatus,
                    Type = tradeType,
                    Time = trade.Timestamp,
                    Price = decimal.Parse(trade.Price),
                    Volume = Math.Abs(decimal.Parse(trade.BaseVolume)),
                    OppositeClientId = trade.OppositeWalletId,
                    OppositeLimitOrderId = trade.OppositeOrderId,
                    OppositeSideVolume = Math.Abs(decimal.Parse(trade.QuotingVolume)),
                    RemainingVolume = Math.Abs(decimal.Parse(order.RemainingVolume))
                });
            }

            return reports;
        }
    }
}
