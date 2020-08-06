using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Subscribers
{
    public class B2C2OrderBooksSubscriber : IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly IB2C2OrderBookService _b2C2OrderBookService;
        private readonly IMarketMakerService _marketMakerService;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private RabbitMqSubscriber<OrderBook> _subscriber;
        
        public B2C2OrderBooksSubscriber(
            SubscriberSettings settings,
            IB2C2OrderBookService b2C2OrderBookService,
            IMarketMakerService marketMakerService,
            ILogFactory logFactory)
        {
            _settings = settings;
            _b2C2OrderBookService = b2C2OrderBookService;
            _marketMakerService = marketMakerService;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }
        
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.Queue);

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<OrderBook>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OrderBook>())
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

        private async Task ProcessMessageAsync(OrderBook orderBook)
        {
            // workaround for Lykke production
            var internalAssetPair = orderBook.Asset.Replace("EOS", "EOScoin");

            var now = DateTime.UtcNow;

            _log.Info("B2C2 Order Book handled.", new
            {
                AssetPairId = orderBook.Asset,
                OrderBookTimestamp = orderBook.Timestamp,
                Now = now,
                Latency = (now - orderBook.Timestamp).TotalMilliseconds
            });

            var sellLimitOrders = orderBook.Asks.Select(o => new Domain.LimitOrder
            {
                Price = o.Price,
                Volume = o.Volume,
                Type = Domain.LimitOrderType.Sell
            });
            
            var buyLimitOrders = orderBook.Bids.Select(o => new Domain.LimitOrder
            {
                Price = o.Price,
                Volume = o.Volume,
                Type = Domain.LimitOrderType.Buy
            });

            var newOrderBook = new Domain.OrderBook
            {
                AssetPairId = internalAssetPair,
                Time = orderBook.Timestamp,
                LimitOrders = sellLimitOrders.Concat(buyLimitOrders).ToArray()
            };

            await _b2C2OrderBookService.SetAsync(newOrderBook);

            await _marketMakerService.UpdateOrderBooksAsync(newOrderBook.AssetPairId);
        }
    }
}
