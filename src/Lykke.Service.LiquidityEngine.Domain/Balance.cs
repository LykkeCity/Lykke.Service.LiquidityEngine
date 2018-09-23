namespace Lykke.Service.LiquidityEngine.Domain
{
    public class Balance
    {
        public Balance(string assetId, decimal amount)
        {
            AssetId = assetId;
            Amount = amount;
        }
        
        public string AssetId { get; }
        
        public decimal Amount { get; }
    }
}
