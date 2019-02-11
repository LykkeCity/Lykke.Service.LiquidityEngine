namespace Lykke.Service.LiquidityEngine.Client.Models.InstrumentMarkups
{
    /// <summary>
    /// Represents instrument markups.
    /// </summary>
    public class InstrumentMarkupModel
    {
        /// <summary>
        /// The identifier of asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Total markup for both asks and bids, without level markups.
        /// </summary>
        public decimal TotalMarkup { get; set; }

        /// <summary>
        /// Total ask markup, without level markups.
        /// </summary>
        public decimal TotalAskMarkup { get; set; }

        /// <summary>
        /// Total bid markup, without level markups.
        /// </summary>
        public decimal TotalBidMarkup { get; set; }
    }
}
