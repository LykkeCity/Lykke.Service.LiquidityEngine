using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;
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
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;
        private readonly B2C2QuoteSubscriber _b2C2QuoteSubscriber;
        private readonly QuoteSubscriber[] _quoteSubscribers;

        public ShutdownManager(
            LykkeBalancesTimer lykkeBalancesTimer,
            ExternalBalancesTimer externalBalancesTimer,
            MarketMakerTimer marketMakerTimer,
            HedgingTimer hedgingTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber,
            B2C2QuoteSubscriber b2C2QuoteSubscriber,
            QuoteSubscriber[] quoteSubscribers)
        {
            _lykkeBalancesTimer = lykkeBalancesTimer;
            _externalBalancesTimer = externalBalancesTimer;
            _marketMakerTimer = marketMakerTimer;
            _hedgingTimer = hedgingTimer;
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
            _b2C2QuoteSubscriber = b2C2QuoteSubscriber;
            _quoteSubscribers = quoteSubscribers;
        }

        public Task StopAsync()
        {
            _b2C2QuoteSubscriber.Stop();

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
