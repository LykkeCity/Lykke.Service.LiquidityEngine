using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Reports
{
    /// <summary>
    /// Represents a balance report for an asset.
    /// </summary>
    [PublicAPI]
    public class BalanceReportModel
    {
        /// <summary>
        /// The asset name.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// The Lykke identifier of an asset.
        /// </summary>
        public string LykkeAssetId { get; set; }

        /// <summary>
        /// The external identifier of an asset.
        /// </summary>
        public string ExternalAssetId { get; set; }

        /// <summary>
        /// The balance amount on Lykke exchange.
        /// </summary>
        public decimal? LykkeAmount { get; set; }

        /// <summary>
        /// The credit amount on Lykke exchange.
        /// </summary>
        public decimal? LykkeCreditAmount { get; set; }

        /// <summary>
        /// Indicates disbalance of current amount and credit amount on Lykke exchange.
        /// </summary>
        public decimal? LykkeDisbalance { get; set; }

        /// <summary>
        /// The balance amount on external exchange.
        /// </summary>
        public decimal? ExternalAmount { get; set; }

        /// <summary>
        /// The total amount.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The total amount converted into USD.
        /// </summary>
        public decimal? TotalAmountInUsd { get; set; }

        /// <summary>
        /// The rate used for conversion into USD.
        /// </summary>
        public decimal? CrossRate { get; set; }
    }
}
