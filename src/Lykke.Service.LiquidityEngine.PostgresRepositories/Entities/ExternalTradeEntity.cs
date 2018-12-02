using System;
using System.ComponentModel.DataAnnotations.Schema;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories.Entities
{
    [Table("external_trades")]
    public class ExternalTradeEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("limit_order_id")]
        public Guid LimitOrderId { get; set; }

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

        [Column("request_id")]
        public Guid RequestId { get; set; }
    }
}
