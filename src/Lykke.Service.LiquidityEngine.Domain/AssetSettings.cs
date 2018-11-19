namespace Lykke.Service.LiquidityEngine.Domain
{
    public class AssetSettings
    {
        public string AssetId { get; set; }

        public string LykkeAssetId { get; set; }

        public string QuoteSource { get; set; }

        public string ExternalAssetPairId { get; set; }

        public bool IsInverse { get; set; }

        public int DisplayAccuracy { get; set; }

        public bool IsCrypto { get; set; }

        public void Update(AssetSettings assetSettings)
        {
            LykkeAssetId = assetSettings.LykkeAssetId;
            QuoteSource = assetSettings.QuoteSource;
            ExternalAssetPairId = assetSettings.ExternalAssetPairId;
            IsInverse = assetSettings.IsInverse;
            DisplayAccuracy = assetSettings.DisplayAccuracy;
            IsCrypto = assetSettings.IsCrypto;
        }
    }
}
