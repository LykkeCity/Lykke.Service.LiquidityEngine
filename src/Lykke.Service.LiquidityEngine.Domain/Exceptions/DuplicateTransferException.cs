namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class DuplicateTransferException : BalanceOperationException
    {
        public DuplicateTransferException()
            : base("Transfer already executed")
        {
        }
    }
}
