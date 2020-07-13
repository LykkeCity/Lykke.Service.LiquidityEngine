using System;

namespace Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates
{
    public class PlacedOrderReport
    {
        public Guid Id { get; set; }

        public long OrderBookUpdateReportId { get; set; }

        public LimitOrderType Type { get; set; }

        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public decimal LevelMarkup { get; set; }

        public PlacedOrderReport(Guid id, long orderBookUpdateReportId, LimitOrderType type, decimal price, decimal volume, decimal levelMarkup)
        {
            Id = id;
            OrderBookUpdateReportId = orderBookUpdateReportId;
            Type = type;
            Price = price;
            Volume = volume;
            LevelMarkup = levelMarkup;
        }

        public PlacedOrderReport(LimitOrder orderLimit, decimal markup, long orderBookUpdateReportId)
        {
            Id = Guid.Parse(orderLimit.Id);
            OrderBookUpdateReportId = orderBookUpdateReportId;
            Type = orderLimit.Type;
            Price = orderLimit.Price;
            Volume = orderLimit.Volume;
            LevelMarkup = markup;
        }
    }
}
