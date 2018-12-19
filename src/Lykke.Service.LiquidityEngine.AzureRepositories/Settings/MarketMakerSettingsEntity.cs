using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class MarketMakerSettingsEntity : AzureTableEntity
    {
        private decimal _limitOrderPriceMaxDeviation;
        private decimal _limitOrderPriceMarkup;

        public MarketMakerSettingsEntity()
        {
        }

        public MarketMakerSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public decimal LimitOrderPriceMaxDeviation
        {
            get => _limitOrderPriceMaxDeviation;
            set
            {
                if (_limitOrderPriceMaxDeviation != value)
                {
                    _limitOrderPriceMaxDeviation = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal LimitOrderPriceMarkup
        {
            get => _limitOrderPriceMarkup;
            set
            {
                if (_limitOrderPriceMarkup != value)
                {
                    _limitOrderPriceMarkup = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
