using System;

namespace Lykke.Service.LiquidityEngine.Domain.MarketMaker
{
    public class MarketMakerState
    {
        public MarketMakerStatus Status { get; set; }

        public DateTime Time { get; set; }

        public MarketMakerError Error { get; set; }

        public string ErrorMessage { get; set; }
    }
}
