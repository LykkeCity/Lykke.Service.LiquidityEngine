using System;

namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class ExternalExchangeThrottlingException : ExternalExchangeException
    {
        public ExternalExchangeThrottlingException(string message) 
            : base(message)
        {
        }

        public ExternalExchangeThrottlingException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
