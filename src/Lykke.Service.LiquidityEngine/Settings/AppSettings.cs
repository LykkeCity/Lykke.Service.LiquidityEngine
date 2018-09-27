using JetBrains.Annotations;
using Lykke.B2c2Client.Settings;
using Lykke.Sdk.Settings;
using Lykke.Service.Balances.Client;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Service.LiquidityEngine.Settings.Clients.MatchingEngine;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings;

namespace Lykke.Service.LiquidityEngine.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public LiquidityEngineSettings LiquidityEngineService { get; set; }
        
        public BalancesServiceClientSettings BalancesServiceClient { get; set; }
        
        public ExchangeOperationsServiceClientSettings ExchangeOperationsServiceClient { get; set; }

        public B2C2ClientSettings B2C2Client { get; set; }
        
        public MatchingEngineClientSettings MatchingEngineClient { get; set; }
    }
}
