using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Db;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LiquidityEngineSettings
    {
        public string Name { get; set; }

        public string WalletId { get; set; }
        
        public DbSettings Db { get; set; }
    }
}
