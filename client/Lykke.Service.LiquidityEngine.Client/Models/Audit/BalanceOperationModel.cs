using System;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Audit
{
    /// <summary>
    /// Represent a balance operation details.
    /// </summary>
    [PublicAPI]
    public class BalanceOperationModel
    {
        /// <summary>
        /// The timestamp of the operation.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The identifier of the asset.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The operation type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The amount than was changed by operation.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The comment that user set while created operation.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The identifier of the user which instantiated operation.
        /// </summary>
        public string UserId { get; set; }
    }
}
