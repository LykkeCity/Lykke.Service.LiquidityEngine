namespace Lykke.Service.LiquidityEngine.Domain
{
    public enum LimitOrderError
    {
        None,

        Unknown,
        
        Idle,
        
        InvalidQuote,

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
