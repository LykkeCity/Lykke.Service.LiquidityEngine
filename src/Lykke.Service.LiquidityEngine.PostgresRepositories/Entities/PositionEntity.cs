using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories.Entities
{
    [Table("positions")]
    public class PositionEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("asset_pair_id")]
        public string AssetPairId { get; set; }

        [Column("type")]
        public PositionType Type { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("volume")]
        public decimal Volume { get; set; }

        [Column("close_date")]
        public DateTime CloseDate { get; set; }

        [Column("close_price")]
        public decimal ClosePrice { get; set; }

        [Column("pnl")]
        public decimal PnL { get; set; }

        [Column("cross_asset_pair_id")]
        public string CrossAssetPairId { get; set; }

        [Column("cross_ask")]
        public decimal? CrossAsk { get; set; }

        [Column("cross_bid")]
        public decimal? CrossBid { get; set; }

        [Column("trade_asset_pair_id")]
        public string TradeAssetPairId { get; set; }

        [Column("trade_avg_price")]
        public decimal TradeAvgPrice { get; set; }

        [Column("internal_trade_id")]
        public Guid? InternalTradeId { get; set; }

        [Column("external_trade_id")]
        public Guid? ExternalTradeId { get; set; }
    }
}
