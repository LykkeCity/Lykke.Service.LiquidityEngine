namespace Lykke.Service.LiquidityEngine.DomainServices.Exchanges
{
    public class CashInContext
    {
        public CashInContext(string clientId, string assetId, decimal amount, string userId)
        {
            ClientId = clientId;
            AssetId = assetId;
            Amount = amount;
            UserId = userId;
        }
        
        public string ClientId { get; }
        
        public string AssetId { get; }
        
        public decimal Amount { get; }
        
        public string UserId { get; }
    }
}
