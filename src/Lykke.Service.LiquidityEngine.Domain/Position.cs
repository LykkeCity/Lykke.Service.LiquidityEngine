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

            PnL = (ClosePrice - Price) * Volume;

            CloseTradeId = externalTrade.Id;
        }

        public static Position Open(IReadOnlyCollection<InternalTrade> internalTrades)
        {
            string assetPairId = internalTrades.First().AssetPairId;
            
            PositionType positionType = internalTrades.First().Type == TradeType.Sell
                ? PositionType.Short
                : PositionType.Long;
            
            decimal avgPrice = internalTrades.Sum(o => o.Price) / internalTrades.Count;
            
            decimal volume = internalTrades.Sum(o => o.Volume);

            string[] trades = internalTrades.Select(o => o.Id).ToArray();
            
            return new Position
            {
                Id = Guid.NewGuid().ToString("D"),
                AssetPairId = assetPairId,
                Type = positionType,
                Date = DateTime.UtcNow,
                Price = avgPrice,
                Volume = volume,
                Trades = trades
            };
        }
    }
}
