using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;
using Lykke.Service.LiquidityEngine.Rabbit;
using Lykke.Service.LiquidityEngine.Rabbit.Publishers;
using Lykke.Service.LiquidityEngine.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Managers
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly LykkeBalancesTimer _lykkeBalancesTimer;
        private readonly ExternalBalancesTimer _externalBalancesTimer;
        private readonly MarketMakerTimer _marketMakerTimer;
        private readonly HedgingTimer _hedgingTimer;
        private readonly SettlementsTimer _settlementsTimer;
        private readonly InternalTraderTimer _internalTraderTimer;
        private readonly PnLStopLossEngineTimer _pnLStopLossEngineTimer;
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;
        private readonly B2C2QuoteSubscriber _b2C2QuoteSubscriber;
        private readonly B2C2OrderBooksSubscriber _b2C2OrderBooksSubscriber;
        private readonly QuoteSubscriber[] _quoteSubscribers;
        private readonly OrderBooksUpdatesReportSubscriber _orderBooksUpdatesReportSubscriber;
        private readonly InternalQuotePublisher _internalQuotePublisher;
        private readonly InternalOrderBookPublisher _internalOrderBookPublisher;
        private readonly OrderBooksUpdatesReportPublisher _orderBooksUpdatesReportPublisher;
        private readonly LykkeTradeSubscriberMonitor _lykkeTradeSubscriberMonitor;

        public ShutdownManager(
            LykkeBalancesTimer lykkeBalancesTimer,
            ExternalBalancesTimer externalBalancesTimer,
            MarketMakerTimer marketMakerTimer,
            HedgingTimer hedgingTimer,
            SettlementsTimer settlementsTimer,
            InternalTraderTimer internalTraderTimer,
            PnLStopLossEngineTimer pnLStopLossEngineTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber,
            B2C2QuoteSubscriber b2C2QuoteSubscriber,
            B2C2OrderBooksSubscriber b2C2OrderBooksSubscriber,
            QuoteSubscriber[] quoteSubscribers,
            OrderBooksUpdatesReportSubscriber orderBooksUpdatesReportSubscriber,
            InternalQuotePublisher internalQuotePublisher,
            InternalOrderBookPublisher internalOrderBookPublisher,
            OrderBooksUpdatesReportPublisher orderBooksUpdatesReportPublisher,
            LykkeTradeSubscriberMonitor lykkeTradeSubscriberMonitor)
        {
            _lykkeBalancesTimer = lykkeBalancesTimer;
            _externalBalancesTimer = externalBalancesTimer;
            _marketMakerTimer = marketMakerTimer;
            _hedgingTimer = hedgingTimer;
            _settlementsTimer = settlementsTimer;
            _internalTraderTimer = internalTraderTimer;
            _pnLStopLossEngineTimer = pnLStopLossEngineTimer;
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
            _b2C2QuoteSubscriber = b2C2QuoteSubscriber;
            _b2C2OrderBooksSubscriber = b2C2OrderBooksSubscriber;
            _quoteSubscribers = quoteSubscribers;
            _orderBooksUpdatesReportSubscriber = orderBooksUpdatesReportSubscriber;
            _internalQuotePublisher = internalQuotePublisher;
            _internalOrderBookPublisher = internalOrderBookPublisher;
            _orderBooksUpdatesReportPublisher = orderBooksUpdatesReportPublisher;
            _lykkeTradeSubscriberMonitor = lykkeTradeSubscriberMonitor;
        }

        public Task StopAsync()
        {
            _internalTraderTimer.Stop();

            _settlementsTimer.Stop();

            _lykkeTradeSubscriberMonitor.Stop();

            _b2C2QuoteSubscriber.Stop();

            _b2C2OrderBooksSubscriber.Stop();

            foreach (QuoteSubscriber quoteSubscriber in _quoteSubscribers)
                quoteSubscriber.Stop();

            _lykkeTradeSubscriber.Stop();

            _orderBooksUpdatesReportSubscriber.Stop();

            _marketMakerTimer.Stop();

            _lykkeBalancesTimer.Stop();

            _externalBalancesTimer.Stop();

            _hedgingTimer.Stop();

            _pnLStopLossEngineTimer.Stop();
            
            _internalQuotePublisher.Stop();

            _internalOrderBookPublisher.Stop();

            _orderBooksUpdatesReportPublisher.Stop();

            return Task.CompletedTask;
        }
    }
}
