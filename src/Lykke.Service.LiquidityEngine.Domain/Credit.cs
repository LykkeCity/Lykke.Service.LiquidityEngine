using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represent a credit details.
    /// </summary>
    public class Credit
    {
        /// <summary>
        /// The asset id.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The amount of a credit.
        /// </summary>
        public decimal Amount { get; set; }

        public void Add(decimal amount)
        {
            if (Amount + amount < 0)
                throw new InvalidOperationException("Credit amount can not be less than zero");

            Amount += amount;
        }
    }
}
