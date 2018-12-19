using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;
using Lykke.Service.LiquidityEngine.Rabbit;
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
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;
        private readonly B2C2QuoteSubscriber _b2C2QuoteSubscriber;
        private readonly B2C2OrderBooksSubscriber _b2C2OrderBooksSubscriber;
        private readonly QuoteSubscriber[] _quoteSubscribers;
        private readonly LykkeTradeSubscriberMonitor _lykkeTradeSubscriberMonitor;

        public ShutdownManager(
            LykkeBalancesTimer lykkeBalancesTimer,
            ExternalBalancesTimer externalBalancesTimer,
            MarketMakerTimer marketMakerTimer,
            HedgingTimer hedgingTimer,
            SettlementsTimer settlementsTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber,
            B2C2QuoteSubscriber b2C2QuoteSubscriber,
            B2C2OrderBooksSubscriber b2C2OrderBooksSubscriber,
            QuoteSubscriber[] quoteSubscribers,
            LykkeTradeSubscriberMonitor lykkeTradeSubscriberMonitor)
        {
            _lykkeBalancesTimer = lykkeBalancesTimer;
            _externalBalancesTimer = externalBalancesTimer;
            _marketMakerTimer = marketMakerTimer;
            _hedgingTimer = hedgingTimer;
            _settlementsTimer = settlementsTimer;
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
            _b2C2QuoteSubscriber = b2C2QuoteSubscriber;
            _b2C2OrderBooksSubscriber = b2C2OrderBooksSubscriber;
            _quoteSubscribers = quoteSubscribers;
            _lykkeTradeSubscriberMonitor = lykkeTradeSubscriberMonitor;
        }

        public Task StopAsync()
        {
            _settlementsTimer.Stop();
            
            _lykkeTradeSubscriberMonitor.Stop();
            
            _b2C2QuoteSubscriber.Stop();

            _b2C2OrderBooksSubscriber.Stop();
            
            foreach (QuoteSubscriber quoteSubscriber in _quoteSubscribers)
                quoteSubscriber.Stop();
            
            _lykkeTradeSubscriber.Stop();

            _marketMakerTimer.Stop();

            _lykkeBalancesTimer.Stop();

            _externalBalancesTimer.Stop();

            _hedgingTimer.Stop();

            return Task.CompletedTask;
        }
    }
}
