using System;

namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class MaxCreditExposureReachedException : ExternalExchangeException
    {
        public MaxCreditExposureReachedException(string message) 
            : base( message)
        {
        }

        public MaxCreditExposureReachedException(string message, Exception exception) 
            : base(message, exception)
        {
        }
    }
}
