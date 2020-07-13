using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain.Publishers;
using Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Publishers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Publishers
{
    [UsedImplicitly]
    public class OrderBooksUpdatesReportPublisher : IOrderBooksUpdatesReportPublisher, IDisposable
    {
        private readonly ILogFactory _logFactory;
        private readonly PublisherSettings _settings;
        private RabbitMqPublisher<OrderBookUpdateReport> _publisher;

        public OrderBooksUpdatesReportPublisher(
            PublisherSettings settings,
            ILogFactory logFactory)
        {
            _logFactory = logFactory;
            _settings = settings;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_settings.ConnectionString, _settings.Exchange);

            _publisher = new RabbitMqPublisher<OrderBookUpdateReport>(_logFactory, settings)
                .SetSerializer(new JsonMessageSerializer<OrderBookUpdateReport>())
                .DisableInMemoryQueuePersistence()
                .Start();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public Task PublishAsync(OrderBookUpdateReport orderBookUpdateReport)
        {
            return _publisher.ProduceAsync(orderBookUpdateReport);
        }

        public void Dispose()
        {
            _publisher?.Stop();
        }
    }
}
