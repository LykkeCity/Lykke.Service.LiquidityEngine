using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.BalanceOperations
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class BalanceOperationEntity : AzureTableEntity
    {
        private DateTime _time;
        private decimal _amount;

        public BalanceOperationEntity()
        {
        }

        public BalanceOperationEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public DateTime Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    MarkValueTypePropertyAsDirty("Time");
                }
            }
        }

        public string AssetId { get; set; }

        public string Type { get; set; }

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    MarkValueTypePropertyAsDirty("Amount");
                }
            }
        }

        public string Comment { get; set; }

        public string UserId { get; set; }
    }
}
