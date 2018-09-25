using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitSubscribers
    {
        public SubscriberSettings LykkeTrades { get; set; }
    }
}
