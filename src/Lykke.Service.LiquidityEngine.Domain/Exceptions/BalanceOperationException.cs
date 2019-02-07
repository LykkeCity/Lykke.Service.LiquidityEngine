using System;

namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class BalanceOperationException : Exception
    {
        public const int NotEnoughFundsCode = 401;

        public BalanceOperationException(string message, int code)
            : base(message)
        {
            Code = code;
        }

        public int Code { get; }
    }
}
