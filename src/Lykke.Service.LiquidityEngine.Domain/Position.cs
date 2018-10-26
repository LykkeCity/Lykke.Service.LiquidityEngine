using System;
using System.Collections.Generic;
using System.Linq;

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
        /// The price of the trade that opened the position in USD.
        /// </summary>
        public decimal PriceUsd { get; set; }

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
        /// The price of the trade that closed the position in USD.
        /// </summary>
        public decimal ClosePriceUsd { get; set; }

        /// <summary>
        /// The realised profit and loss.
        /// </summary>
        public decimal PnL { get; set; }

        /// <summary>
        /// The realised profit and loss converted into USD.
        /// </summary>
        public decimal PnLUsd { get; set; }

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
        /// A collection of identifiers of the trades that opened position.
        /// </summary>
        public IReadOnlyCollection<string> Trades { get; set; }

        /// <summary>
        /// The identifier of the trade that closed position.
        /// </summary>
        public string CloseTradeId { get; set; }

        public void Close(ExternalTrade externalTrade)
        {
            CloseDate = DateTime.UtcNow;
            ClosePrice = externalTrade.Price;
            ClosePriceUsd = externalTrade.PriceUsd;

            int volumeSign = Type == PositionType.Long ? 1 : -1;

            PnL = (ClosePrice - Price) * Volume * volumeSign;
            PnLUsd = (ClosePriceUsd - PriceUsd) * Volume * volumeSign;

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
                PriceUsd = externalTrade.PriceUsd,
                Volume = externalTrade.Volume,
                Trades = new string[0],
                CloseDate = DateTime.UtcNow,
                ClosePrice = externalTrade.Price,
                ClosePriceUsd = externalTrade.PriceUsd,
                PnL = decimal.Zero,
                CloseTradeId = externalTrade.Id
            };
        }

        public static Position Open(string assetPairId, decimal price, decimal avgPrice, decimal volume, Quote quote,
            string tradeAssetPairId, TradeType tradeType, string[] trades)
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
                Trades = trades,
                CloseDate = new DateTime(1900, 1, 1),
                
                CrossAssetPairId = quote.AssetPair,
                CrossAsk = quote.Ask,
                CrossBid = quote.Bid,
                TradeAssetPairId = tradeAssetPairId,
                TradeAvgPrice = avgPrice
            };
        }
        
        public static Position Open(string assetPairId, decimal price, decimal volume, TradeType tradeType,
            string[] trades)
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
                Trades = trades,
                CloseDate = new DateTime(1900, 1, 1),
                
                CrossAssetPairId = null,
                CrossAsk = null,
                CrossBid = null,
                TradeAssetPairId = assetPairId,
                TradeAvgPrice = price
            };
        }

        public static Position Open(IReadOnlyCollection<InternalTrade> internalTrades)
        {
            string assetPairId = internalTrades.First().AssetPairId;

            PositionType positionType = internalTrades.First().Type == TradeType.Sell
                ? PositionType.Short
                : PositionType.Long;

            decimal avgPrice = internalTrades.Sum(o => o.Price) / internalTrades.Count;

            decimal avgPriceUsd = internalTrades.Sum(o => o.PriceUsd) / internalTrades.Count;

            decimal volume = internalTrades.Sum(o => o.Volume);

            string[] trades = internalTrades.Select(o => o.Id).ToArray();

            return new Position
            {
                Id = Guid.NewGuid().ToString("D"),
                AssetPairId = assetPairId,
                Type = positionType,
                Date = DateTime.UtcNow,
                Price = avgPrice,
                PriceUsd = avgPriceUsd,
                Volume = volume,
                Trades = trades,
                CloseDate = new DateTime(1900, 1, 1)
            };
        }
    }
}
