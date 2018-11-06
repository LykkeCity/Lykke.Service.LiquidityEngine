using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Trades
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class ExternalTradeEntity : AzureTableEntity
    {
        private TradeType _type;
        private DateTime _time;
        private decimal _price;
        private decimal _volume;
        
        public ExternalTradeEntity()
        {
        }

        public ExternalTradeEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        
        public string Id { get; set; }

        public string LimitOrderId { get; set; }

        public string AssetPairId { get; set; }

        public TradeType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public DateTime Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public string RequestId { get; set; }
    }
}
