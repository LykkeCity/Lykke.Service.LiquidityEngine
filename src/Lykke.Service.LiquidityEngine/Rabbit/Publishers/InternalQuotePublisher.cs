using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Publishers;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Publishers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Publishers
{
    [UsedImplicitly]
    public class InternalQuotePublisher : IInternalQuotePublisher, IDisposable
    {
        private readonly ILogFactory _logFactory;
        private readonly PublisherSettings _settings;
        private readonly string _instanceName;
        private RabbitMqPublisher<TickPrice> _publisher;

        public InternalQuotePublisher(
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

            _publisher = new RabbitMqPublisher<TickPrice>(_logFactory, settings)
                .SetSerializer(new JsonMessageSerializer<TickPrice>())
                .DisableInMemoryQueuePersistence()
                .Start();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public Task PublishAsync(Quote quote)
        {
            return _publisher.ProduceAsync(new TickPrice
            {
                Asset = quote.AssetPair,
                Ask = quote.Ask,
                Bid = quote.Bid,
                Timestamp = quote.Time,
                Source = $"LiquidityEngine{_instanceName}"
            });
        }

        public void Dispose()
        {
            _publisher?.Stop();
        }
    }
}
