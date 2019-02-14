using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class MarketMakerSettingsEntity : AzureTableEntity
    {
        private decimal _limitOrderPriceMaxDeviation;
        private decimal _limitOrderPriceMarkup;
        private decimal _fiatEquityThresholdFrom;
        private decimal _fiatEquityThresholdTo;
        private decimal _fiatEquityMarkupFrom;
        private decimal _fiatEquityMarkupTo;
        private TimeSpan _noFreshQuotesInterval;
        private decimal _noFreshQuotesMarkup;

        public MarketMakerSettingsEntity()
        {
        }

        public MarketMakerSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public decimal LimitOrderPriceMaxDeviation
        {
            get => _limitOrderPriceMaxDeviation;
            set
            {
                if (_limitOrderPriceMaxDeviation != value)
                {
                    _limitOrderPriceMaxDeviation = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal LimitOrderPriceMarkup
        {
            get => _limitOrderPriceMarkup;
            set
            {
                if (_limitOrderPriceMarkup != value)
                {
                    _limitOrderPriceMarkup = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal FiatEquityThresholdFrom
        {
            get => _fiatEquityThresholdFrom;
            set
            {
                if (_fiatEquityThresholdFrom != value)
                {
                    _fiatEquityThresholdFrom = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal FiatEquityThresholdTo
        {
            get => _fiatEquityThresholdTo;
            set
            {
                if (_fiatEquityThresholdTo != value)
                {
                    _fiatEquityThresholdTo = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal FiatEquityMarkupFrom
        {
            get => _fiatEquityMarkupFrom;
            set
            {
                if (_fiatEquityMarkupFrom != value)
                {
                    _fiatEquityMarkupFrom = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal FiatEquityMarkupTo
        {
            get => _fiatEquityMarkupTo;
            set
            {
                if (_fiatEquityMarkupTo != value)
                {
                    _fiatEquityMarkupTo = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public TimeSpan  NoFreshQuotesInterval
        {
            get => _noFreshQuotesInterval;
            set
            {
                if (_noFreshQuotesInterval != value)
                {
                    _noFreshQuotesInterval = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal NoFreshQuotesMarkup
        {
            get => _noFreshQuotesMarkup;
            set
            {
                if (_noFreshQuotesMarkup != value)
                {
                    _noFreshQuotesMarkup = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
