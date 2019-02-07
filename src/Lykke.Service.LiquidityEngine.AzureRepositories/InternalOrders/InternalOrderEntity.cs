using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.InternalOrders
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class InternalOrderEntity : AzureTableEntity
    {
        private decimal _price;
        private decimal _volume;
        private bool _fullExecution;
        private InternalOrderStatus _status;
        private DateTime _createdDate;
        private LimitOrderType _type;

        public InternalOrderEntity()
        {
        }

        public InternalOrderEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Id { set; get; }

        public string WalletId { set; get; }

        public string AssetPairId { set; get; }

        public LimitOrderType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal? ExecutedPrice { set; get; }

        public decimal? ExecutedVolume { set; get; }

        public bool FullExecution
        {
            get => _fullExecution;
            set
            {
                if (_fullExecution != value)
                {
                    _fullExecution = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public InternalOrderStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public string RejectReason { set; get; }

        public string TradeId { get; set; }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                if (_createdDate != value)
                {
                    _createdDate = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
