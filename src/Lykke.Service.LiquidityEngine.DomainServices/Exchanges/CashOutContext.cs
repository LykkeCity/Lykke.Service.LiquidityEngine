namespace Lykke.Service.LiquidityEngine.DomainServices.Exchanges
{
    public class CashOutContext
    {
        public CashOutContext(string clientId, string assetId, decimal amount)
        {
            ClientId = clientId;
            AssetId = assetId;
            Amount = amount;
        }
        
        public string ClientId { get; }
        
        public string AssetId { get; }
        
        public decimal Amount { get; }
    }
}
