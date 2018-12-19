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
        /// Creates limit orders using price levels of an instrument.
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
        /// Creates dynamically distributed limit orders using price levels of an instrument.
        /// </summary>
        /// <param name="quote1">Best prices of instrument on first level.</param>
        /// <param name="quote2">Best prices of instrument on second level.</param>
        /// <param name="levels">A collection of price levels.</param>
        /// <param name="baseAssetBalance">The amount of the base asset on a hedge exchange.</param>
        /// <param name="quoteAssetBalance">The amount of the quote asset on a hedge exchange.</param>
        /// <param name="timeSinceLastTrade">The total seconds from last trade in order book.</param>
        /// <param name="halfLifePeriod">The half life period in seconds.</param>
        /// <param name="allowSmartMarkup">If <c>true</c> then smart markup for first level will be applied; otherwise default markup.</param>
        /// <param name="markup">Common markup.</param>
        /// <param name="priceAccuracy">The accuracy of price.</param>
        /// <param name="volumeAccuracy">The accuracy of volume.</param>
        /// <returns>A collection of limit orders.</returns>
        public static IReadOnlyCollection<LimitOrder> CalculateLimitOrders(Quote quote1, Quote quote2,
            InstrumentLevel[] levels, decimal baseAssetBalance, decimal quoteAssetBalance, int timeSinceLastTrade,
            int halfLifePeriod, bool allowSmartMarkup, decimal markup, int priceAccuracy, int volumeAccuracy)
        {
            var limitOrders = new List<LimitOrder>();

            if (levels.Length == 0)
                return limitOrders;

            decimal sumSideVolume = levels.Sum(o => o.Volume);

            decimal deltaVolume = sumSideVolume - levels[0].Volume;

            decimal sellDeltaPrice = (quote2.Ask - quote1.Ask) / quote1.Ask;

            decimal buyDeltaPrice = (quote1.Bid - quote2.Bid) / quote1.Bid;

            decimal sellMarketPrice = quote1.Ask;

            decimal sellRawPrice = quote1.Ask;

            decimal buyMarketPrice = quote1.Bid;

            decimal buyRawPrice = quote1.Bid;

            decimal sumVolume = levels[0].Volume;

            decimal sellFirstLevelMarkup = levels[0].Markup;

            decimal buyFirstLevelMarkup = levels[0].Markup;

            if (allowSmartMarkup)
            {
                double alpha = Math.Log(2) / halfLifePeriod;

                decimal relativeSpread = (quote1.Ask - quote1.Bid) / quote1.Mid;

                decimal smartMarkup = levels[0].Markup - relativeSpread / 2m +
                                      relativeSpread * (decimal) Math.Exp(-alpha * timeSinceLastTrade);

                if (-baseAssetBalance * quote1.Mid >= quoteAssetBalance)
                    sellFirstLevelMarkup = smartMarkup;
                else
                    buyFirstLevelMarkup = smartMarkup;
            }

            decimal sellFirstLevelPrice =
                (sellRawPrice * (1 + sellFirstLevelMarkup + markup)).TruncateDecimalPlaces(priceAccuracy, true);

            decimal buyFirstLevelPrice =
                (buyRawPrice * (1 - (buyFirstLevelMarkup + markup))).TruncateDecimalPlaces(priceAccuracy);

            limitOrders.Add(LimitOrder.CreateSell(sellFirstLevelPrice, Math.Round(levels[0].Volume, volumeAccuracy)));
            limitOrders.Add(LimitOrder.CreateBuy(buyFirstLevelPrice, Math.Round(levels[0].Volume, volumeAccuracy)));

            for (int i = 1; i < levels.Length; i++)
            {
                decimal prevSellMarketPrice = sellMarketPrice;

                decimal prevBuyMarketPrice = buyMarketPrice;

                decimal prevSumVolume = sumVolume;

                sumVolume += levels[i].Volume;

                sellMarketPrice = (1 + (sumVolume - levels[0].Volume) / deltaVolume * sellDeltaPrice) * quote1.Ask;

                buyMarketPrice = (1 - (sumVolume - levels[0].Volume) / deltaVolume * buyDeltaPrice) * quote1.Bid;

                sellRawPrice = (sellMarketPrice * sumVolume - prevSellMarketPrice * prevSumVolume) / levels[i].Volume;

                buyRawPrice = (buyMarketPrice * sumVolume - prevBuyMarketPrice * prevSumVolume) / levels[i].Volume;

                decimal sellPrice = (sellRawPrice * (1 + levels[i].Markup + markup))
                    .TruncateDecimalPlaces(priceAccuracy, true);

                decimal buyPrice = (buyRawPrice * (1 - (levels[i].Markup + markup)))
                    .TruncateDecimalPlaces(priceAccuracy);

                limitOrders.Add(LimitOrder.CreateSell(Math.Max(sellFirstLevelPrice, sellPrice),
                    Math.Round(levels[i].Volume, volumeAccuracy)));

                limitOrders.Add(LimitOrder.CreateBuy(Math.Min(buyFirstLevelPrice, buyPrice),
                    Math.Round(levels[i].Volume, volumeAccuracy)));
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

        /// <summary>
        /// Calculates mid price of direct limit order using cross instrument.
        /// </summary>
        /// <param name="price">The price of cross limit order.</param>
        /// <param name="quote">Mid price of cross instrument.</param>
        /// <param name="inverse">Indicates that the quote is inverse.</param>
        /// <returns>The direct limit order price and used cross rate.</returns>
        public static (decimal, decimal) CalculateCrossMidPrice(decimal price, Quote quote, bool inverse)
            => (inverse ? price / quote.Mid : price * quote.Mid,
                inverse ? 1 / quote.Mid : quote.Mid);
    }
}
