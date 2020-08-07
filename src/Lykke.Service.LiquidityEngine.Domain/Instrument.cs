using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents an instrument which used to create limit orders.
    /// </summary>
    public class Instrument
    {
        public Instrument()
        {
            Levels = new InstrumentLevel[0];
            CrossInstruments = new CrossInstrument[0];
        }

        /// <summary>
        /// The identifier of an internal asset pair. 
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The mode of the instrument.  
        /// </summary>
        public InstrumentMode Mode { get; set; }

        /// <summary>
        /// The threshold of the instrument realized profit and loss.
        /// </summary>
        public decimal PnLThreshold { get; set; }

        /// <summary>
        /// The threshold of the instrument absolute inventory.
        /// </summary>
        public decimal InventoryThreshold { get; set; }

        /// <summary>
        /// The min volume that can be used to create external limit order.
        /// </summary>
        public decimal MinVolume { get; set; }

        /// <summary>
        /// The accuracy of the hedge limit order volume that will be created on external exchange.
        /// </summary>
        public int VolumeAccuracy { get; set; }

        /// <summary>
        /// A collection of order book levels.
        /// </summary>
        public IReadOnlyCollection<InstrumentLevel> Levels { get; set; }

        /// <summary>
        /// A collection of cross instrument.
        /// </summary>
        public IReadOnlyCollection<CrossInstrument> CrossInstruments { get; set; }

        /// <summary>
        /// Indicates that the smart markup is allowed for instrument.
        /// </summary>
        public bool AllowSmartMarkup { get; set; }

        /// <summary>
        /// The half life period of asset pair in seconds.
        /// </summary>
        public int HalfLifePeriod { get; set; }

        public void Update(Instrument instrument)
        {
            Mode = instrument.Mode;
            PnLThreshold = instrument.PnLThreshold;
            InventoryThreshold = instrument.InventoryThreshold;
            MinVolume = instrument.MinVolume;
            VolumeAccuracy = instrument.VolumeAccuracy;
            AllowSmartMarkup = instrument.AllowSmartMarkup;
            HalfLifePeriod = instrument.HalfLifePeriod;
        }

        public void AddLevel(InstrumentLevel instrumentLevel)
        {
            if (Levels?.Any(o => o.Number == instrumentLevel.Number) == true)
            {
                throw new InvalidOperationException("The level already exists");
            }

            instrumentLevel.Id = Guid.NewGuid().ToString();

            Levels = (Levels ?? new InstrumentLevel[0])
                .Union(new[] {instrumentLevel})
                .OrderBy(o => o.Number)
                .ToArray();
        }

        public void UpdateLevel(InstrumentLevel instrumentLevel)
        {
            InstrumentLevel currentLevelVolume = (Levels ?? new InstrumentLevel[0])
                .FirstOrDefault(o => !string.IsNullOrEmpty(instrumentLevel.Id)
                    ? o.Id == instrumentLevel.Id
                    : o.Number == instrumentLevel.Number);

            if (currentLevelVolume == null)
                throw new InvalidOperationException("The level does not exists");

            if (!string.IsNullOrEmpty(instrumentLevel.Id) && Levels?
                .Any(o => o.Id != instrumentLevel.Id && o.Number == instrumentLevel.Number) == true)
            {
                throw new InvalidOperationException("The level already exists");
            }

            currentLevelVolume.Number = instrumentLevel.Number;
            currentLevelVolume.Volume = instrumentLevel.Volume;
            currentLevelVolume.Markup = instrumentLevel.Markup;

            // ReSharper disable once AssignNullToNotNullAttribute
            Levels = Levels
                .OrderBy(o => o.Number)
                .ToArray();
        }

        public void UpdateLevels(IReadOnlyCollection<InstrumentLevel> instrumentLevels)
        {
            Levels = instrumentLevels
                .OrderBy(o => o.Number)
                .ToArray();
        }

        public void RemoveLevel(string levelId)
        {
            Levels = (Levels ?? new InstrumentLevel[0])
                .Where(o => o.Id != levelId)
                .OrderBy(o => o.Number)
                .ToArray();
        }

        public void RemoveLevelByNumber(int levelNumber)
        {
            Levels = (Levels ?? new InstrumentLevel[0])
                .Where(o => o.Number != levelNumber)
                .OrderBy(o => o.Number)
                .ToArray();
        }

        public void AddCrossInstrument(CrossInstrument crossInstrument)
        {
            if (CrossInstruments?.Any(o => o.AssetPairId == crossInstrument.AssetPairId) == true)
                throw new InvalidOperationException("The cross instrument already exists");

            CrossInstruments = (CrossInstruments ?? new CrossInstrument[0])
                .Union(new[] {crossInstrument})
                .ToArray();
        }

        public void UpdateCrossInstrument(CrossInstrument crossInstrument)
        {
            CrossInstrument currentCrossInstrument = (CrossInstruments ?? new CrossInstrument[0])
                .FirstOrDefault(o => o.AssetPairId == crossInstrument.AssetPairId);

            if (currentCrossInstrument == null)
                throw new InvalidOperationException("The cross instrument does not exists");

            currentCrossInstrument.IsInverse = crossInstrument.IsInverse;
            currentCrossInstrument.QuoteSource = crossInstrument.QuoteSource;
            currentCrossInstrument.ExternalAssetPairId = crossInstrument.ExternalAssetPairId;
        }

        public void RemoveCrossInstrument(string crossAssetPairId)
        {
            CrossInstruments = (CrossInstruments ?? new CrossInstrument[0])
                .Where(o => o.AssetPairId != crossAssetPairId)
                .ToArray();
        }

        public decimal ApplyVolume(decimal volume, TradeType side)
        {
            foreach (var level in Levels.OrderBy(x => x.Number))
            {
                var volumeLevel = side == TradeType.Buy ? level.BuyVolume : level.SellVolume;

                var size = Math.Min(volumeLevel, volume);

                volumeLevel -= size;
                volume -= size;

                if (side == TradeType.Buy)
                    level.BuyVolume = volumeLevel;
                else
                    level.SellVolume = volumeLevel;

                if (volumeLevel <= 0)
                    return 0;
            }

            return volume;
        }

        public void ResetTradingVolume()
        {
            foreach (var level in Levels)
            {
                level.BuyVolume = level.Volume;

                level.SellVolume = level.Volume;
            }
        }
    }
}
