using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers.Quotes
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class QuotesSettings
    {
        [AmqpCheck] public string ConnectionString { get; set; }

        public Exchange[] Exchanges { get; set; }

        public string Queue { get; set; }
    }
}
