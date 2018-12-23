using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a position details.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// The identifier of the position.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The type of position.
        /// </summary>
        public PositionType Type { get; set; }

        /// <summary>
        /// The date when position was opened. 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The price of the trade that opened the position.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of the trade that opened the position.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// The date of the trade that closed the position.
        /// </summary>
        public DateTime CloseDate { get; set; }

        /// <summary>
        /// The price of the trade that closed the position.
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// The realised profit and loss.
        /// </summary>
        public decimal PnL { get; set; }

        /// <summary>
        /// The identifier of asset pair that used to convert trade price.
        /// </summary>
        public string CrossAssetPairId { get; set; }

        /// <summary>
        /// The best sell price on trade time.
        /// </summary>
        public decimal? CrossAsk { get; set; }

        /// <summary>
        /// The best buy price on trade time.
        /// </summary>
        public decimal? CrossBid { get; set; }

        /// <summary>
        /// The identifier of asset pair that used by trade.
        /// </summary>
        public string TradeAssetPairId { get; set; }

        /// <summary>
        /// The average price of executed limit orders.
        /// </summary>
        public decimal TradeAvgPrice { get; set; }

        /// <summary>
        /// A identifier of the trade that opened position.
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// The identifier of the trade that closed position.
        /// </summary>
        public string CloseTradeId { get; set; }

        public void Close(ExternalTrade externalTrade)
        {
            CloseDate = DateTime.UtcNow;
            ClosePrice = externalTrade.Price;

            int volumeSign = Type == PositionType.Long ? 1 : -1;

            PnL = (ClosePrice - Price) * Volume * volumeSign;

            CloseTradeId = externalTrade.Id;
        }

        public static Position Create(string assetPairId, ExternalTrade externalTrade)
        {
            PositionType positionType = externalTrade.Type == TradeType.Sell
                ? PositionType.Long
                : PositionType.Short;

            return new Position
            {
                Id = Guid.NewGuid().ToString("D"),
                AssetPairId = assetPairId,
                Type = positionType,
                Date = DateTime.UtcNow,
                Price = externalTrade.Price,
                Volume = externalTrade.Volume,
                CloseDate = DateTime.UtcNow,
                ClosePrice = externalTrade.Price,
                PnL = decimal.Zero,
                TradeAssetPairId = assetPairId,
                TradeAvgPrice = externalTrade.Price,
                CloseTradeId = externalTrade.Id
            };
        }

        public static Position Open(string assetPairId, decimal price, decimal avgPrice, decimal volume, Quote quote,
            string tradeAssetPairId, TradeType tradeType, string tradeId)
            => Open(assetPairId, price, avgPrice, volume, quote.AssetPair, quote.Ask, quote.Bid, tradeAssetPairId,
                tradeType, tradeId);

        public static Position Open(string assetPairId, decimal price, decimal volume, TradeType tradeType,
            string tradeId)
            => Open(assetPairId, price, price, volume, null, null, null, assetPairId, tradeType, tradeId);

        private static Position Open(string assetPairId, decimal price, decimal avgPrice, decimal volume,
            string crossAssetPairId, decimal? crossAsk, decimal? crossBid, string tradeAssetPairId, TradeType tradeType,
            string tradeId)
        {
            PositionType positionType = tradeType == TradeType.Sell
                ? PositionType.Short
                : PositionType.Long;

            return new Position
            {
                Id = Guid.NewGuid().ToString("D"),
                AssetPairId = assetPairId,
                Type = positionType,
                Date = DateTime.UtcNow,
                Price = price,
                Volume = volume,
                TradeId = tradeId,
                CloseDate = new DateTime(1900, 1, 1),

                CrossAssetPairId = crossAssetPairId,
                CrossAsk = crossAsk,
                CrossBid = crossBid,
                TradeAssetPairId = tradeAssetPairId,
                TradeAvgPrice = avgPrice
            };
        }
    }
}
