using System;
using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.MsSqlRepositories.Entities
{
    public class OrderBookUpdateReportEntity
    {
        public long Id { get; set; }

        public double IterationId { get; set; } // unix date time in seconds

        public DateTime IterationDateTime { get; set; }

        public string AssetPair { get; set; }

        public decimal FirstQuoteAsk { get; set; }

        public decimal FirstQuoteBid { get; set; }

        public decimal SecondQuoteAsk { get; set; }

        public decimal SecondQuoteBid { get; set; }

        public DateTime QuoteDateTime { get; set; }

        public decimal GlobalMarkup { get; set; }

        public decimal NoFreshQuoteMarkup { get; set; }

        public decimal PnLStopLossMarkup { get; set; }

        public decimal FiatEquityMarkup { get; set; }

        public IList<PlacedOrderReportEntity> Orders { get; set; } = new List<PlacedOrderReportEntity>();
    }
}
