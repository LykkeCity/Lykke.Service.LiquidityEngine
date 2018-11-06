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
    }
}
