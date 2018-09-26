using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Publishers;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitSettings
    {
        //public RabbitPublishers Publishers { get; set; }

        public RabbitSubscribers Subscribers { get; set; }
    }
}
