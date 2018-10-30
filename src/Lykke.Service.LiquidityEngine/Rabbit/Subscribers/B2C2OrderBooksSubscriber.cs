using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ILogFactory _logFactory;

        private RabbitMqSubscriber<OrderBook> _subscriber;
        
        public B2C2OrderBooksSubscriber(
            SubscriberSettings settings,
            IB2C2OrderBookService b2C2OrderBookService,
            ILogFactory logFactory)
        {
            _settings = settings;
            _b2C2OrderBookService = b2C2OrderBookService;
            _logFactory = logFactory;
        }
        
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.Queue);

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

        private Task ProcessMessageAsync(OrderBook orderBook)
        {
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

            return _b2C2OrderBookService.SetAsync(new Domain.OrderBook
            {
                AssetPairId = orderBook.Asset,
                Time = orderBook.Timestamp,
                LimitOrders = sellLimitOrders.Concat(buyLimitOrders).ToArray()
            });
        }
    }
}
