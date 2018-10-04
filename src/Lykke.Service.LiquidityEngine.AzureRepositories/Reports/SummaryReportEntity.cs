using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Reports
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class SummaryReportEntity : AzureTableEntity
    {
        private int _openPositionsCount;
        private int _closedPositionsCount;
        private decimal _pnL;
        private decimal _baseAssetVolume;
        private decimal _quoteAssetVolume;
        private decimal _totalSellBaseAssetVolume;
        private decimal _totalBuyBaseAssetVolume;
        private decimal _totalSellQuoteAssetVolume;
        private decimal _totalBuyQuoteAssetVolume;
        private int _sellTradesCount;
        private int _buyTradesCount;

        public SummaryReportEntity()
        {
        }

        public SummaryReportEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        
        public string AssetPairId { get; set; }

        public int OpenPositionsCount
        {
            get => _openPositionsCount;
            set
            {
                if (_openPositionsCount != value)
                {
                    _openPositionsCount = value;
                    MarkValueTypePropertyAsDirty("OpenPositionsCount");
                }
            }
        }

        public int ClosedPositionsCount
        {
            get => _closedPositionsCount;
            set
            {
                if (_closedPositionsCount != value)
                {
                    _closedPositionsCount = value;
                    MarkValueTypePropertyAsDirty("ClosedPositionsCount");
                }
            }
        }

        public decimal PnL
        {
            get => _pnL;
            set
            {
                if (_pnL != value)
                {
                    _pnL = value;
                    MarkValueTypePropertyAsDirty("PnL");
                }
            }
        }

        public decimal BaseAssetVolume
        {
            get => _baseAssetVolume;
            set
            {
                if (_baseAssetVolume != value)
                {
                    _baseAssetVolume = value;
                    MarkValueTypePropertyAsDirty("BaseAssetVolume");
                }
            }
        }

        public decimal QuoteAssetVolume
        {
            get => _quoteAssetVolume;
            set
            {
                if (_quoteAssetVolume != value)
                {
                    _quoteAssetVolume = value;
                    MarkValueTypePropertyAsDirty("QuoteAssetVolume");
                }
            }
        }

        public decimal TotalSellBaseAssetVolume
        {
            get => _totalSellBaseAssetVolume;
            set
            {
                if (_totalSellBaseAssetVolume != value)
                {
                    _totalSellBaseAssetVolume = value;
                    MarkValueTypePropertyAsDirty("TotalSellBaseAssetVolume");
                }
            }
        }

        public decimal TotalBuyBaseAssetVolume
        {
            get => _totalBuyBaseAssetVolume;
            set
            {
                if (_totalBuyBaseAssetVolume != value)
                {
                    _totalBuyBaseAssetVolume = value;
                    MarkValueTypePropertyAsDirty("TotalBuyBaseAssetVolume");
                }
            }
        }

        public decimal TotalSellQuoteAssetVolume
        {
            get => _totalSellQuoteAssetVolume;
            set
            {
                if (_totalSellQuoteAssetVolume != value)
                {
                    _totalSellQuoteAssetVolume = value;
                    MarkValueTypePropertyAsDirty("TotalSellQuoteAssetVolume");
                }
            }
        }

        public decimal TotalBuyQuoteAssetVolume
        {
            get => _totalBuyQuoteAssetVolume;
            set
            {
                if (_totalBuyQuoteAssetVolume != value)
                {
                    _totalBuyQuoteAssetVolume = value;
                    MarkValueTypePropertyAsDirty("TotalBuyQuoteAssetVolume");
                }
            }
        }

        public int SellTradesCount
        {
            get => _sellTradesCount;
            set
            {
                if (_sellTradesCount != value)
                {
                    _sellTradesCount = value;
                    MarkValueTypePropertyAsDirty("SellTradesCount");
                }
            }
        }

        public int BuyTradesCount
        {
            get => _buyTradesCount;
            set
            {
                if (_buyTradesCount != value)
                {
                    _buyTradesCount = value;
                    MarkValueTypePropertyAsDirty("BuyTradesCount");
                }
            }
        }
    }
}
