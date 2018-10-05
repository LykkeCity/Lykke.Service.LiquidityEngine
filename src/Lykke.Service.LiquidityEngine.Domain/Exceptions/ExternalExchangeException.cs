using System;

namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class ExternalExchangeException : Exception
    {
        public ExternalExchangeException(string message) 
            : base(message)
        {
        }

        public ExternalExchangeException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
