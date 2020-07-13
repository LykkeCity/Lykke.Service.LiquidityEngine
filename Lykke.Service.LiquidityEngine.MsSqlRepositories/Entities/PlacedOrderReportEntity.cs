using System;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.MsSqlRepositories.Entities
{
    public class PlacedOrderReportEntity
    {
        public Guid Id { get; set; }

        public long OrderBookUpdateReportId { get; set; }

        public LimitOrderType Type { get; set; }

        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public decimal LevelMarkup { get; set; }
    }
}
