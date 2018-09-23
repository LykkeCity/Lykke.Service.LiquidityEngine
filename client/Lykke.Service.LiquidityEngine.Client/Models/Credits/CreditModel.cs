using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Credits
{
    /// <summary>
    /// Represent a credit details.
    /// </summary>
    [PublicAPI]
    public class CreditModel
    {
        /// <summary>
        /// The asset id.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The amount of a credit.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
