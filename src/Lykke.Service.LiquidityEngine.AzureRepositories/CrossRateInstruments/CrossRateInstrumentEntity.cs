using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.CrossRateInstruments
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class CrossRateInstrumentEntity : AzureTableEntity
    {
        private string _quoteSource;
        private string _externalAssetPairId;
        private bool _isInverse;

        public CrossRateInstrumentEntity()
        {
        }

        public CrossRateInstrumentEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string AssetPairId { get; set; }

        public string QuoteSource
        {
            get => _quoteSource;
            set
            {
                if (_quoteSource != value)
                {
                    _quoteSource = value;
                    MarkValueTypePropertyAsDirty("QuoteSource");
                }
            }
        }

        public string ExternalAssetPairId
        {
            get => _externalAssetPairId;
            set
            {
                if (_externalAssetPairId != value)
                {
                    _externalAssetPairId = value;
                    MarkValueTypePropertyAsDirty("ExternalAssetPairId");
                }
            }
        }

        public bool IsInverse
        {
            get => _isInverse;
            set
            {
                if (_isInverse != value)
                {
                    _isInverse = value;
                    MarkValueTypePropertyAsDirty("IsInverse");
                }
            }
        }
    }
}
