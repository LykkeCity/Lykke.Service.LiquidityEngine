using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain.Publishers;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Publishers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Publishers
{
    [UsedImplicitly]
    public class InternalOrderBookPublisher : IInternalOrderBookPublisher, IDisposable
    {
        private readonly ILogFactory _logFactory;
        private readonly PublisherSettings _settings;
        private readonly string _instanceName;
        private RabbitMqPublisher<OrderBook> _publisher;

        public InternalOrderBookPublisher(
            PublisherSettings settings,
            string instanceName,
            ILogFactory logFactory)
        {
            _logFactory = logFactory;
            _settings = settings;
            _instanceName = instanceName;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_settings.ConnectionString, _settings.Exchange);

            _publisher = new RabbitMqPublisher<OrderBook>(_logFactory, settings)
                .SetSerializer(new JsonMessageSerializer<OrderBook>())
                .DisableInMemoryQueuePersistence()
                .Start();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public Task PublishAsync(Domain.OrderBook orderBook)
        {
            IEnumerable<OrderBookItem> sellOrderBookItems = orderBook.LimitOrders
                .Where(o => o.Type == Domain.LimitOrderType.Sell)
                .OrderBy(o => o.Price)
                .Select(o => new OrderBookItem(o.Price, o.Volume));

            IEnumerable<OrderBookItem> buyOrderBookItems = orderBook.LimitOrders
                .Where(o => o.Type == Domain.LimitOrderType.Buy)
                .OrderByDescending(o => o.Price)
                .Select(o => new OrderBookItem(o.Price, o.Volume));

            return _publisher.ProduceAsync(new OrderBook($"LiquidityEngine{_instanceName}", orderBook.AssetPairId,
                orderBook.Time, sellOrderBookItems, buyOrderBookItems));
        }

        public void Dispose()
        {
            _publisher?.Stop();
        }
    }
}
