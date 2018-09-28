using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    public class Quote
    {
        public Quote(string assetPair, DateTime time, decimal ask, decimal bid, string source)
        {
            AssetPair = assetPair;
            Time = time;
            Ask = ask;
            Bid = bid;
            Mid = (ask + bid) / 2m;
            Spread = Ask - Bid;
            Source = source;
        }

        public string AssetPair { get; }

        public DateTime Time { get; }

        public decimal Ask { get; }

        public decimal Bid { get; }

        public decimal Mid { get; }

        public decimal Spread { get; }

        public string Source { get; }
    }
}
