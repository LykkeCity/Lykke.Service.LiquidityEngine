using System;

namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class BalanceOperationException : Exception
    {
        public BalanceOperationException(string message)
            : base(message)
        {
        }
    }
}
