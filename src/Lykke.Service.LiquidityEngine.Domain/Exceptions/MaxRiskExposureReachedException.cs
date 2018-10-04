using System;

namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class MaxRiskExposureReachedException : ExternalExchangeException
    {
        public MaxRiskExposureReachedException(string message) 
            : base( message)
        {
        }

        public MaxRiskExposureReachedException(string message, Exception exception) 
            : base(message, exception)
        {
        }
    }
}
