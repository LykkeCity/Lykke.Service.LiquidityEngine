namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class NotEnoughFundsException : BalanceOperationException
    {
        public NotEnoughFundsException()
            : base("Not enough funds")
        {
        }
    }
}
