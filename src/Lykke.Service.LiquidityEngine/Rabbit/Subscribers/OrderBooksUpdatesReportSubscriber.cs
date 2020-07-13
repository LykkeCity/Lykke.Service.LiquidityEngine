using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Subscribers
{
    public class OrderBooksUpdatesReportSubscriber : IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly IOrderBookUpdateReportRepository _orderBookUpdateReportRepository;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private RabbitMqSubscriber<OrderBookUpdateReport> _subscriber;
        
        public OrderBooksUpdatesReportSubscriber(
            SubscriberSettings settings,
            IOrderBookUpdateReportRepository orderBookUpdateReportRepository,
            ILogFactory logFactory)
        {
            _settings = settings;
            _orderBookUpdateReportRepository = orderBookUpdateReportRepository;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }
        
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.Queue);

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<OrderBookUpdateReport>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OrderBookUpdateReport>())
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

        private async Task ProcessMessageAsync(OrderBookUpdateReport orderBookUpdateReport)
        {
            _log.InfoWithDetails("OrderBookUpdateReport is about to be saved...", orderBookUpdateReport);

            await _orderBookUpdateReportRepository.InsertAsync(orderBookUpdateReport);

            _log.InfoWithDetails("OrderBookUpdateReport was saved.", orderBookUpdateReport);
        }
    }
}
