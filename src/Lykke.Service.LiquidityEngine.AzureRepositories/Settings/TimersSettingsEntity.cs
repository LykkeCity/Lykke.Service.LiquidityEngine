using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class TimersSettingsEntity : AzureTableEntity
    {
        private TimeSpan _marketMaker;
        private TimeSpan _balances;

        public TimersSettingsEntity()
        {
        }

        public TimersSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public TimeSpan MarketMaker
        {
            get => _marketMaker;
            set
            {
                if (_marketMaker != value)
                {
                    _marketMaker = value;
                    MarkValueTypePropertyAsDirty("MarketMaker");
                }
            }
        }

        public TimeSpan Balances
        {
            get => _balances;
            set
            {
                if (_balances != value)
                {
                    _balances = value;
                    MarkValueTypePropertyAsDirty("Balances");
                }
            }
        }
    }
}
