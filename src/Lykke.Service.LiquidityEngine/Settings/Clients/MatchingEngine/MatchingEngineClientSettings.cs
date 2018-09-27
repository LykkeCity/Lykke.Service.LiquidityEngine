using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Settings.Clients.MatchingEngine
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class MatchingEngineClientSettings
    {
        public IpEndpointSettings IpEndpoint { get; set; }
    }
}
