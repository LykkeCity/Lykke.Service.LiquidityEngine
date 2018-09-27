namespace Lykke.Service.LiquidityEngine.Domain
{
    public enum LimitOrderError
    {
        None,

        Unknown,
        
        Idle,

        Arbitrage,

        EmptyOrderBook,

        LowBalance,

        NoLiquidity,

        NotEnoughFunds,

        ReservedVolumeHigherThanBalance,

        BalanceLowerThanReserved ,

        LeadToNegativeSpread,

        TooSmallVolume,

        InvalidPrice
    }
}
