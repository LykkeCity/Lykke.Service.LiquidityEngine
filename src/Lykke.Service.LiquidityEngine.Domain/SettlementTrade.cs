using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a B2C2 fiat settlement trade.
    /// </summary>
    public class SettlementTrade
    {
        /// <summary>
        /// The identifier of trade.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The type of trade.
        /// </summary>
        public TradeType Type { get; set; }

        /// <summary>
        /// The trade asset pair.
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The base asset of asset pair.
        /// </summary>
        public string BaseAsset { get; set; }

        /// <summary>
        /// The quote asset of asset pair.
        /// </summary>
        public string QuoteAsset { get; set; }

        /// <summary>
        /// The trade price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The trade volume.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// The trade opposite volume.
        /// </summary>
        public decimal OppositeVolume { get; set; }

        /// <summary>
        /// The trade date.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Indicated that the trade was settled.
        /// </summary>
        public bool IsCompleted { get; set; }

        public void Complete()
        {
            if (IsCompleted)
                throw new InvalidOperationException("The trade already completed");

            IsCompleted = true;
        }

        public SettlementTrade Copy()
        {
            return new SettlementTrade
            {
                Id = Id,
                Type = Type,
                AssetPair = AssetPair,
                BaseAsset = BaseAsset,
                QuoteAsset = QuoteAsset,
                Price = Price,
                Volume = Volume,
                OppositeVolume = OppositeVolume,
                Timestamp = Timestamp,
                IsCompleted = IsCompleted
            };
        }
    }
}
