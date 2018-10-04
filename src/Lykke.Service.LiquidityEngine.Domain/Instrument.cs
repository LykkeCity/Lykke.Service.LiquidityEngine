using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents an instrument which used to create limit orders.
    /// </summary>
    public class Instrument
    {
        /// <summary>
        /// The identifier of an internal asset pair. 
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The mode of the instrument.  
        /// </summary>
        public InstrumentMode Mode { get; set; }
        
        /// <summary>
        /// The threshold of the instrument realised profit and loss.
        /// </summary>
        public decimal PnLThreshold { get; set; }

        /// <summary>
        /// The threshold of the instrument absolute inventory.
        /// </summary>
        public decimal InventoryThreshold { get; set; }
        
        /// <summary>
        /// A collection of order book levels.
        /// </summary>
        public IReadOnlyCollection<InstrumentLevel> Levels { get; set; }

        public void Update(Instrument instrument)
        {
            Mode = instrument.Mode;
            PnLThreshold = instrument.PnLThreshold;
            InventoryThreshold = instrument.InventoryThreshold;
        }

        public void AddLevel(InstrumentLevel instrumentLevel)
        {
            if (Levels?.Any(o => o.Number == instrumentLevel.Number) == true)
                throw new InvalidOperationException("The level already exists");

            Levels = (Levels ?? new List<InstrumentLevel>())
                .Union(new[] {instrumentLevel})
                .OrderBy(o => o.Number)
                .ToArray();
        }

        public void RemoveLevel(int levelNumber)
        {
            Levels = (Levels ?? new List<InstrumentLevel>())
                .Where(o => o.Number != levelNumber)
                .OrderBy(o => o.Number)
                .Select((levelVolume, index) => new InstrumentLevel
                {
                    Number = index + 1,
                    Volume = levelVolume.Volume
                })
                .ToArray();
        }

        public void UpdateLevel(InstrumentLevel instrumentLevel)
        {
            InstrumentLevel currentLevelVolume = (Levels ?? new List<InstrumentLevel>())
                .FirstOrDefault(o => o.Number == instrumentLevel.Number);
            
            if(currentLevelVolume == null)
                throw new InvalidOperationException("The level does not exists");

            currentLevelVolume.Volume = instrumentLevel.Volume;
            currentLevelVolume.Markup = instrumentLevel.Markup;
        }
    }
}
