using System;

namespace Lykke.Service.LiquidityEngine.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
            : base("Entity not found")
        {
        }
    }
}
