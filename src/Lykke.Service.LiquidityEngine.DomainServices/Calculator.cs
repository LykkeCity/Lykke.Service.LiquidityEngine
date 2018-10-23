using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    public static class Calculator
    {
        /// <summary>
        /// Creates limit orders using price levels of instrument.
        /// </summary>
        /// <param name="quote">Best prices of instrument.</param>
        /// <param name="levels">A collection of price levels.</param>
        /// <param name="priceAccuracy">The accuracy of price.</param>
        /// <param name="volumeAccuracy">The accuracy of volume.</param>
        /// <returns>A collection of limit orders.</returns>
        public static IReadOnlyCollection<LimitOrder> CalculateLimitOrders(Quote quote,
            IReadOnlyCollection<InstrumentLevel> levels, int priceAccuracy, int volumeAccuracy)
        {
            var limitOrders = new List<LimitOrder>();

            foreach (InstrumentLevel instrumentLevel in levels.OrderBy(o => o.Number))
            {
                decimal sellPrice = (quote.Ask * (1 + instrumentLevel.Markup))
                    .TruncateDecimalPlaces(priceAccuracy, true);
                decimal buyPrice = (quote.Bid * (1 - instrumentLevel.Markup))
                    .TruncateDecimalPlaces(priceAccuracy);

                decimal volume = Math.Round(instrumentLevel.Volume, volumeAccuracy);

                limitOrders.Add(LimitOrder.CreateSell(sellPrice, volume));
                limitOrders.Add(LimitOrder.CreateBuy(buyPrice, volume));
            }

            return limitOrders;
        }

        /// <summary>
        /// Calculates cross limit orders by original limit orders using cross price.
        /// </summary>
        /// <param name="quote">Best prices of cross instrument.</param>
        /// <param name="limitOrders">The collection of original limit orders.</param>
        /// <param name="inverse">Indicates that the quote is inverse.</param>
        /// <param name="priceAccuracy">The accuracy of price.</param>
        /// <param name="volumeAccuracy">The accuracy of volume.</param>
        /// <returns>A collection of limit orders.</returns>
        public static IReadOnlyCollection<LimitOrder> CalculateCrossLimitOrders(Quote quote,
            IReadOnlyCollection<LimitOrder> limitOrders, bool inverse, int priceAccuracy, int volumeAccuracy)
        {
            return limitOrders
                .Select(o => o.Type == LimitOrderType.Sell
                    ? LimitOrder.CreateSell(CalculateCrossSellPrice(quote, o.Price, inverse)
                        .TruncateDecimalPlaces(priceAccuracy, true), Math.Round(o.Volume, volumeAccuracy))
                    : LimitOrder.CreateBuy(CalculateCrossBuyPrice(quote, o.Price, inverse)
                        .TruncateDecimalPlaces(priceAccuracy), Math.Round(o.Volume, volumeAccuracy)))
                .ToArray();
        }

        /// <summary>
        /// Calculates a sell price using cross rate.
        /// </summary>
        /// <param name="quote">Best prices of cross instrument.</param>
        /// <param name="price">The direct price.</param>
        /// <param name="inverse">Indicates that the quote is inverse.</param>
        /// <returns>The price calculated by cross rate.</returns>
        public static decimal CalculateCrossSellPrice(Quote quote, decimal price, bool inverse)
            => inverse ? price / quote.Bid : price * quote.Ask;

        /// <summary>
        /// Calculates a buy price using cross rate.
        /// </summary>
        /// <param name="quote">Best prices of cross instrument.</param>
        /// <param name="price">The direct price.</param>
        /// <param name="inverse">Indicates that the quote is inverse.</param>
        /// <returns>The price calculated by cross rate.</returns>
        public static decimal CalculateCrossBuyPrice(Quote quote, decimal price, bool inverse)
            => inverse ? price / quote.Ask : price * quote.Bid;

        /// <summary>
        /// Calculates sell price of direct limit order using cross instrument.
        /// </summary>
        /// <param name="price">The price of cross limit order.</param>
        /// <param name="quote">Best prices of cross instrument.</param>
        /// <param name="inverse">Indicates that the quote is inverse.</param>
        /// <returns>The direct limit order price.</returns>
        public static decimal CalculateDirectSellPrice(decimal price, Quote quote, bool inverse)
            => inverse ? price * quote.Bid : price / quote.Ask;

        /// <summary>
        /// Calculates buy price of direct limit order using cross instrument.
        /// </summary>
        /// <param name="price">The price of cross limit order.</param>
        /// <param name="quote">Best prices of cross instrument.</param>
        /// <param name="inverse">Indicates that the quote is inverse.</param>
        /// <returns>The direct limit order price.</returns>
        public static decimal CalculateDirectBuyPrice(decimal price, Quote quote, bool inverse)
            => inverse ? price * quote.Ask : price / quote.Bid;
    }
}
