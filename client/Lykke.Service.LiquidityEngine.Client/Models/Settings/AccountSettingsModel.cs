using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settings
{
    /// <summary>
    /// Represent an account settings.
    /// </summary>
    [PublicAPI]
    public class AccountSettingsModel
    {
        /// <summary>
        /// The identifier of the wallet that used by service.
        /// </summary>
        public string WalletId { get; set; }
    }
}
