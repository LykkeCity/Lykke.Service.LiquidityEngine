using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Rabbit.Subscribers
{
    public class B2C2QuoteSubscriber : IDisposable
    {
        private readonly SubscriberSettings _settings;
        private readonly IQuoteService _quoteService;
        private readonly ILogFactory _logFactory;

        private RabbitMqSubscriber<TickPrice> _subscriber;
        
        public B2C2QuoteSubscriber(
            SubscriberSettings settings,
            IQuoteService quoteService,
            ILogFactory logFactory)
        {
            _settings = settings;
            _quoteService = quoteService;
            _logFactory = logFactory;
        }
        
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_settings.ConnectionString, _settings.Exchange, _settings.Queue);

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<TickPrice>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<TickPrice>())
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

        private Task ProcessMessageAsync(TickPrice tickPrice)
        {
            // workaround for Lykke production
            var internalAssetPair = tickPrice.Asset.Replace("EOS", "EOScoin");

            return _quoteService.SetAsync(new Quote(internalAssetPair, tickPrice.Timestamp, tickPrice.Ask, tickPrice.Bid,
                tickPrice.Source));
        }
    }
}
