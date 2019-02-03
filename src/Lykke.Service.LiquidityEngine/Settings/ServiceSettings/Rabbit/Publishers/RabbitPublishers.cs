using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Publishers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitPublishers
    {
        public PublisherSettings InternalQuotes { get; set; }

        public PublisherSettings InternalOrderBooks { get; set; }
    }
}
