using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Db
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DbSettings
    {
        public string PostgresConnectionString { get; set; }

        [AzureTableCheck]
        public string DataConnectionString { get; set; }

        [AzureTableCheck]
        public string LogsConnectionString { get; set; }
        
        [AzureTableCheck]
        public string LykkeTradesMeQueuesDeduplicatorConnectionString { get; set; }
    }
}
