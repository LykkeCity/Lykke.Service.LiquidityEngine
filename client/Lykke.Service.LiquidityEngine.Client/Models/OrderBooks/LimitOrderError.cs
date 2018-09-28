using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.OrderBooks
{
    [PublicAPI]
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
