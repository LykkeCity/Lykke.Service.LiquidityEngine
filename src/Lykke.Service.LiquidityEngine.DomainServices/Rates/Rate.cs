namespace Lykke.Service.LiquidityEngine.DomainServices.Rates
{
    internal class AssetRate
    {
        public string AssetIdFrom { get; set; }

        public string AssetIdTo { get; set; }

        public decimal Rate { get; set; }
    }
}
