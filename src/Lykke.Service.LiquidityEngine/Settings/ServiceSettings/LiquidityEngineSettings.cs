using System;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Db;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Dwh;
using Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LiquidityEngineSettings
    {
        public string Name { get; set; }

        public string WalletId { get; set; }

        public TimeSpan AssetsCacheExpirationPeriod { get; set; }

        public DbSettings Db { get; set; }

        public RabbitSettings Rabbit { get; set; }

        public DwhSettings Dwh { get; set; }
    }
}
