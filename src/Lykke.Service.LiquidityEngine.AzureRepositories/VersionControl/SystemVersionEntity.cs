using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.VersionControl
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class SystemVersionEntity : AzureTableEntity
    {
        private int _versionNumber;

        public SystemVersionEntity()
        {
        }

        public SystemVersionEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public int VersionNumber
        {
            get => _versionNumber;
            set
            {
                if (_versionNumber != value)
                {
                    _versionNumber = value;
                    MarkValueTypePropertyAsDirty("VersionNumber");
                }
            }
        }
    }
}
