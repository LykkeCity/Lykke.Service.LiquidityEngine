using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.AssetSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class AssetSettingsEntity : AzureTableEntity
    {
        private bool _isInverse;
        private bool _isCrypto;
        private int _displayAccuracy;

        public AssetSettingsEntity()
        {
        }

        public AssetSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string AssetId { get; set; }

        public string LykkeAssetId { get; set; }

        public string QuoteSource { get; set; }

        public string ExternalAssetPairId { get; set; }

        public bool IsInverse
        {
            get => _isInverse;
            set
            {
                if (_isInverse != value)
                {
                    _isInverse = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public bool IsCrypto
        {
            get => _isCrypto;
            set
            {
                if (_isCrypto != value)
                {
                    _isCrypto = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public int DisplayAccuracy
        {
            get => _displayAccuracy;
            set
            {
                if (_displayAccuracy != value)
                {
                    _displayAccuracy = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
