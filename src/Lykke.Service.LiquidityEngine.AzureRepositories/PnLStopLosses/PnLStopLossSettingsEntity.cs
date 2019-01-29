using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.PnLStopLosses
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class PnLStopLossSettingsEntity : AzureTableEntity
    {
        private string _assetPairId;
        private TimeSpan _interval;
        private decimal _threshold;
        private decimal _markup;

        public PnLStopLossSettingsEntity()
        {
        }

        public PnLStopLossSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Id { get; set; }

        public string AssetPairId
        {
            get => _assetPairId;
            set
            {
                if (_assetPairId != value)
                {
                    _assetPairId = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public TimeSpan Interval
        {
            get => _interval;
            set
            {
                if (_interval != value)
                {
                    _interval = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Threshold
        {
            get => _threshold;
            set
            {
                if (_threshold != value)
                {
                    _threshold = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Markup
        {
            get => _markup;
            set
            {
                if (_markup != value)
                {
                    _markup = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
