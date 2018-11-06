using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Instruments
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class CrossInstrumentEntity : AzureTableEntity
    {
        private bool _isInverse;
        private string _externalAssetPairId;

        public CrossInstrumentEntity()
        {
        }

        public CrossInstrumentEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string AssetPairId { get; set; }

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

        public string QuoteSource { get; set; }

        public string ExternalAssetPairId
        {
            get => _externalAssetPairId;
            set
            {
                if (_externalAssetPairId != value)
                {
                    _externalAssetPairId = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
