using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings;

namespace Lykke.Service.LiquidityEngine.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public LiquidityEngineSettings LiquidityEngineService { get; set; }
    }
}
