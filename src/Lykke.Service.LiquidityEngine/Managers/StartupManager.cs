using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;
using Lykke.Service.LiquidityEngine.Migration;
using Lykke.Service.LiquidityEngine.Rabbit;
using Lykke.Service.LiquidityEngine.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Managers
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
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
        private readonly StorageMigrationService _storageMigrationService;
        private readonly ITradeService _tradeService;
        private readonly IPnLStopLossService _pnLStopLossService;

        public StartupManager(
            LykkeBalancesTimer lykkeBalancesTimer,
            ExternalBalancesTimer externalBalancesTimer,
            MarketMakerTimer marketMakerTimer,
            HedgingTimer hedgingTimer,
            SettlementsTimer settlementsTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber,
            B2C2QuoteSubscriber b2C2QuoteSubscriber,
            B2C2OrderBooksSubscriber b2C2OrderBooksSubscriber,
            QuoteSubscriber[] quoteSubscribers,
            LykkeTradeSubscriberMonitor lykkeTradeSubscriberMonitor,
            StorageMigrationService storageMigrationService,
            ITradeService tradeService,
            IPnLStopLossService pnLStopLossService)
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
            _storageMigrationService = storageMigrationService;
            _tradeService = tradeService;
            _pnLStopLossService = pnLStopLossService;
        }

        public async Task StartAsync()
        {
            _tradeService.Initialize();

            await _storageMigrationService.MigrateStorageAsync();

            _b2C2QuoteSubscriber.Start();

            _b2C2OrderBooksSubscriber.Start();

            foreach (QuoteSubscriber quoteSubscriber in _quoteSubscribers)
                quoteSubscriber.Start();

            _lykkeTradeSubscriber.Start();

            _lykkeBalancesTimer.Start();

            _externalBalancesTimer.Start();

            _marketMakerTimer.Start();

            _hedgingTimer.Start();
            
            _settlementsTimer.Start();
            
            _lykkeTradeSubscriberMonitor.Start();
        }
    }
}
