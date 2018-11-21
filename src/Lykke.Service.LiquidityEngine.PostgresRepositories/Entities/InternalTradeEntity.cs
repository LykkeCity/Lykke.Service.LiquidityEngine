using System;
using System.ComponentModel.DataAnnotations.Schema;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories.Entities
{
    [Table("internal_trades")]
    public class InternalTradeEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("limit_order_id")]
        public Guid LimitOrderId { get; set; }

        [Column("exchange_order_id")]
        public Guid ExchangeOrderId { get; set; }

        [Column("asset_pair_id")]
        public string AssetPairId { get; set; }

        [Column("type")]
        public TradeType Type { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("volume")]
        public decimal Volume { get; set; }

        [Column("remaining_volume")]
        public decimal RemainingVolume { get; set; }

        [Column("status")]
        public TradeStatus Status { get; set; }

        [Column("opposite_volume")]
        public decimal OppositeVolume { get; set; }

        [Column("opposite_client_id")]
        public Guid OppositeClientId { get; set; }

        [Column("opposite_limit_order_id")]
        public Guid OppositeLimitOrderId { get; set; }
    }
}
