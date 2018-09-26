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
        /// The risk markup.
        /// </summary>
        public decimal Markup { get; set; }

        /// <summary>
        /// A collection of order book levels.
        /// </summary>
        public IReadOnlyCollection<LevelVolume> Levels { get; set; }

        public void Update(Instrument instrument)
        {
            Mode = instrument.Mode;
            Markup = instrument.Markup;
        }

        public void AddLevel(LevelVolume levelVolume)
        {
            if (Levels?.Any(o => o.Number == levelVolume.Number) == true)
                throw new InvalidOperationException("The level already exists");

            Levels = (Levels ?? new List<LevelVolume>())
                .Union(new[] {levelVolume})
                .OrderBy(o => o.Number)
                .ToArray();
        }

        public void RemoveLevel(int levelNumber)
        {
            Levels = (Levels ?? new List<LevelVolume>())
                .Where(o => o.Number != levelNumber)
                .OrderBy(o => o.Number)
                .Select((levelVolume, index) => new LevelVolume
                {
                    Number = index + 1,
                    Volume = levelVolume.Volume
                })
                .ToArray();
        }

        public void UpdateLevel(LevelVolume levelVolume)
        {
            LevelVolume currentLevelVolume = (Levels ?? new List<LevelVolume>())
                .FirstOrDefault(o => o.Number == levelVolume.Number);
            
            if(currentLevelVolume == null)
                throw new InvalidOperationException("The level does not exists");

            currentLevelVolume.Volume = levelVolume.Volume;
        }
    }
}
