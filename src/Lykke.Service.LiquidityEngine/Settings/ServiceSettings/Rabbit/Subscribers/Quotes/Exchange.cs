using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit.Subscribers.Quotes
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Exchange
    {
        public string Name { get; set; }

        public string Endpoint { get; set; }
    }
}
