using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Instruments
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class InstrumentEntity : AzureTableEntity
    {
        private InstrumentMode _mode;
        private decimal _pnLThreshold;
        private decimal _inventoryThreshold;
        private int _volumeAccuracy;
        private decimal _minVolume;
        private int _halfLifePeriod;
        private bool _allowSmartMarkup;

        public InstrumentEntity()
        {
        }

        public InstrumentEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string AssetPairId { get; set; }

        public InstrumentMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal PnLThreshold
        {
            get => _pnLThreshold;
            set
            {
                if (_pnLThreshold != value)
                {
                    _pnLThreshold = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal InventoryThreshold
        {
            get => _inventoryThreshold;
            set
            {
                if (_inventoryThreshold != value)
                {
                    _inventoryThreshold = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public int VolumeAccuracy
        {
            get => _volumeAccuracy;
            set
            {
                if (_volumeAccuracy != value)
                {
                    _volumeAccuracy = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal MinVolume
        {
            get => _minVolume;
            set
            {
                if (_minVolume != value)
                {
                    _minVolume = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public bool AllowSmartMarkup
        {
            get => _allowSmartMarkup;
            set
            {
                if (_allowSmartMarkup != value)
                {
                    _allowSmartMarkup = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public int HalfLifePeriod
        {
            get => _halfLifePeriod;
            set
            {
                if (_halfLifePeriod != value)
                {
                    _halfLifePeriod = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        [JsonValueSerializer]
        public InstrumentLevel[] Levels { get; set; }
    }
}
