using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.MarketMaker
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class MarketMakerStateEntity : AzureTableEntity
    {
        private MarketMakerStatus _status;
        private DateTime _time;
        private MarketMakerError _error;
        private string _errorMessage;

        public MarketMakerStateEntity()
        {
        }

        public MarketMakerStateEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public MarketMakerStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    MarkValueTypePropertyAsDirty("Status");
                }
            }
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

        public MarketMakerError Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    MarkValueTypePropertyAsDirty("Error");
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    MarkValueTypePropertyAsDirty("ErrorMessage");
                }
            }
        }
    }
}
