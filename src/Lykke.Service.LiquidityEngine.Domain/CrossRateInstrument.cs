namespace Lykke.Service.LiquidityEngine.Domain
{
    public class CrossRateInstrument
    {
        public string AssetPairId { get; set; }

        public string QuoteSource { get; set; }

        public string ExternalAssetPairId { get; set; }

        public bool IsInverse { get; set; }

        public void Update(CrossRateInstrument crossInstrument)
        {
            QuoteSource = crossInstrument.QuoteSource;
            ExternalAssetPairId = crossInstrument.ExternalAssetPairId;
            IsInverse = crossInstrument.IsInverse;
        }
    }
}
