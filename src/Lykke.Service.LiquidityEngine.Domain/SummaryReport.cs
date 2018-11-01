﻿namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a summary report. 
    /// </summary>
    public class SummaryReport
    {
        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The number of open positions.
        /// </summary>
        public int OpenPositionsCount { get; set; }

        /// <summary>
        /// The number of closed positions.
        /// </summary>
        public int ClosedPositionsCount { get; set; }

        /// <summary>
        /// The cumulative profit and loss.
        /// </summary>
        public decimal PnL { get; set; }

        /// <summary>
        /// The cumulative profit and loss converted into USD.
        /// </summary>
        public decimal? PnLUsd { get; set; }

        /// <summary>
        /// The current volume of the base asset.
        /// </summary>
        public decimal BaseAssetVolume { get; set; }
        
        /// <summary>
        /// The current volume of the quote asset.
        /// </summary>
        public decimal QuoteAssetVolume {get; set; }
        
        /// <summary>
        /// The total volume of the base asset that was sold.
        /// </summary>
        public decimal TotalSellBaseAssetVolume { get; set; }
        
        /// <summary>
        /// The total volume of the base asset that was bought.
        /// </summary>
        public decimal TotalBuyBaseAssetVolume { get; set; }
        
        /// <summary>
        /// The total volume of the quote asset that was sold.
        /// </summary>
        public decimal TotalSellQuoteAssetVolume { get; set; }
        
        /// <summary>
        /// The total volume of the quote asset that was bought.
        /// </summary>
        public decimal TotalBuyQuoteAssetVolume { get; set; }
        
        /// <summary>
        /// The number of sell trades.
        /// </summary>
        public int SellTradesCount { get; set; }
        
        /// <summary>
        /// The number of buy trades.
        /// </summary>
        public int BuyTradesCount { get; set; }

        public void ApplyTrade(InternalTrade internalTrade)
        {
            if (internalTrade.Type == TradeType.Sell)
            {
                BaseAssetVolume -= internalTrade.Volume;
                QuoteAssetVolume += internalTrade.OppositeSideVolume;
                
                TotalSellBaseAssetVolume += internalTrade.Volume;
                
                TotalBuyQuoteAssetVolume += internalTrade.OppositeSideVolume;

                SellTradesCount++;
            }
            else
            {
                BaseAssetVolume += internalTrade.Volume;
                QuoteAssetVolume -= internalTrade.OppositeSideVolume;
                
                TotalBuyBaseAssetVolume += internalTrade.Volume;
                
                TotalSellQuoteAssetVolume += internalTrade.OppositeSideVolume;

                BuyTradesCount++;
            }
        }

        public void ApplyOpenPosition()
        {
            OpenPositionsCount++;
        }

        public void ApplyClosePosition(Position position)
        {
            OpenPositionsCount--;
            ClosedPositionsCount++;

            PnL += position.PnL;
            PnLUsd += position.PnLUsd ?? 0;
        }
    }
}
