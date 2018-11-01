using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.AssetPairLinks
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class AssetPairLinkEntity : AzureTableEntity
    {
        public AssetPairLinkEntity()
        {
        }

        public AssetPairLinkEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string AssetPairId { get; set; }

        public string ExternalAssetPairId { get; set; }

        public string ExternalBaseAssetId { get; set; }

        public string ExternalQuoteAssetId { get; set; }
    }
}
