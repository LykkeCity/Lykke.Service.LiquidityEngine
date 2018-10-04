using System.ComponentModel;
using Lykke.MatchingEngine.Connector.Models.Api;
using Lykke.MatchingEngine.Connector.Models.Common;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.DomainServices.Extensions
{
    public static class MatchingEngineMappingExtensions
    {
        public static OrderAction ToOrderAction(this LimitOrderType limitOrderType)
        {
            if (limitOrderType == LimitOrderType.Buy)
                return OrderAction.Buy;

            if (limitOrderType == LimitOrderType.Sell)
                return OrderAction.Sell;

            throw new InvalidEnumArgumentException(nameof(limitOrderType), (int)limitOrderType, typeof(TradeType));
        }
        
        public static LimitOrderError ToOrderError(this MeStatusCodes meStatusCode)
        {
            switch (meStatusCode)
            {
                case MeStatusCodes.Ok:
                    return LimitOrderError.None;
                case MeStatusCodes.LowBalance:
                    return LimitOrderError.LowBalance;
                case MeStatusCodes.NoLiquidity:
                    return LimitOrderError.NoLiquidity;
                case MeStatusCodes.NotEnoughFunds:
                    return LimitOrderError.NotEnoughFunds;
                case MeStatusCodes.ReservedVolumeHigherThanBalance:
                    return LimitOrderError.ReservedVolumeHigherThanBalance;
                case MeStatusCodes.BalanceLowerThanReserved:
                    return LimitOrderError.BalanceLowerThanReserved;
                case MeStatusCodes.LeadToNegativeSpread:
                    return LimitOrderError.LeadToNegativeSpread;
                case MeStatusCodes.TooSmallVolume:
                    return LimitOrderError.TooSmallVolume;
                case MeStatusCodes.InvalidPrice:
                    return LimitOrderError.InvalidPrice;
                default:
                    return LimitOrderError.Unknown;
            }
        }
    }
}
