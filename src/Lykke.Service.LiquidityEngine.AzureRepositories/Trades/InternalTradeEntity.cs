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
    public class InternalTradeEntity : AzureTableEntity
    {
        private TradeType _type;
        private DateTime _time;
        private decimal _price;
        private decimal _priceUsd;
        private decimal _volume;
        private decimal _remainingVolume;
        private TradeStatus _status;
        private decimal _oppositeSideVolume;

        public InternalTradeEntity()
        {
        }

        public InternalTradeEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        
        public string Id { get; set; }

        public string LimitOrderId { get; set; }

        public string ExchangeOrderId { get; set; }

        public string AssetPairId { get; set; }

        public TradeType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    MarkValueTypePropertyAsDirty("Type");
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
                    MarkValueTypePropertyAsDirty("Time");
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
                    MarkValueTypePropertyAsDirty("Price");
                }
            }
        }

        public decimal PriceUsd
        {
            get => _priceUsd;
            set
            {
                if (_priceUsd != value)
                {
                    _priceUsd = value;
                    MarkValueTypePropertyAsDirty("PriceUsd");
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
                    MarkValueTypePropertyAsDirty("Volume");
                }
            }
        }

        public decimal RemainingVolume
        {
            get => _remainingVolume;
            set
            {
                if (_remainingVolume != value)
                {
                    _remainingVolume = value;
                    MarkValueTypePropertyAsDirty("RemainingVolume");
                }
            }
        }

        public TradeStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    MarkValueTypePropertyAsDirty("Status");
                }
            }
        }

        public decimal OppositeSideVolume
        {
            get => _oppositeSideVolume;
            set
            {
                if (_oppositeSideVolume != value)
                {
                    _oppositeSideVolume = value;
                    MarkValueTypePropertyAsDirty("OppositeSideVolume");
                }
            }
        }

        public string OppositeClientId { get; set; }

        public string OppositeLimitOrderId { get; set; }
    }
}
