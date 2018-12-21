using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Settings.ServiceSettings.Dwh
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DwhSettings
    {
        public string DatabaseName { get; set; }

        public StoredProcedures StoredProcedures { get; set; }
    }
}
