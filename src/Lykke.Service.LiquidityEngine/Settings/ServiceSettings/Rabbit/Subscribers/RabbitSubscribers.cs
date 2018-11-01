using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers.Quotes;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitSubscribers
    {
        public SubscriberSettings LykkeTrades { get; set; }

        public SubscriberSettings B2C2Quotes { get; set; }
        
        public SubscriberSettings B2C2OrderBooks { get; set; }

        public QuotesSettings Quotes { get; set; }
    }
}
