using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.Domain
{
    public class InstrumentMessages
    {
        public string AssetPairId { get; set; }

        public IReadOnlyCollection<string> Messages { get; set; }
    }
}
