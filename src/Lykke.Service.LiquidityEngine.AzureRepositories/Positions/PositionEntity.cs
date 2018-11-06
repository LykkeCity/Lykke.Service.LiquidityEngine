using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Positions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class PositionEntity : AzureTableEntity
    {
        private PositionType _type;
        private DateTime _date;
        private decimal _price;
        private decimal? _priceUsd;
        private decimal _volume;
        private DateTime _closeDate;
        private decimal _closePrice;
        private decimal? _closePriceUsd;
        private decimal _pnL;
        private decimal? _pnLUsd;
        private decimal _tradeAvgPrice;

        public PositionEntity()
        {
        }

        public PositionEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Id { get; set; }

        public string AssetPairId { get; set; }

        public PositionType Type
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

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
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

        public decimal? PriceUsd
        {
            get => _priceUsd;
            set
            {
                if (_priceUsd != value)
                {
                    _priceUsd = value;
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

        public DateTime CloseDate
        {
            get => _closeDate;
            set
            {
                if (_closeDate != value)
                {
                    _closeDate = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal ClosePrice
        {
            get => _closePrice;
            set
            {
                if (_closePrice != value)
                {
                    _closePrice = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal? ClosePriceUsd
        {
            get => _closePriceUsd;
            set
            {
                if (_closePriceUsd != value)
                {
                    _closePriceUsd = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal PnL
        {
            get => _pnL;
            set
            {
                if (_pnL != value)
                {
                    _pnL = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal? PnLUsd
        {
            get => _pnLUsd;
            set
            {
                if (_pnLUsd != value)
                {
                    _pnLUsd = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public string CrossAssetPairId { get; set; }

        public decimal? CrossAsk { get; set; }

        public decimal? CrossBid { get; set; }

        public string TradeAssetPairId { get; set; }

        public decimal TradeAvgPrice
        {
            get => _tradeAvgPrice;
            set
            {
                if (_tradeAvgPrice != value)
                {
                    _tradeAvgPrice = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        [JsonValueSerializer]
        public IReadOnlyCollection<string> Trades { get; set; }

        public string CloseTradeId { get; set; }
    }
}
