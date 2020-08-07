﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IInstrumentService
    {
        Task<IReadOnlyCollection<Instrument>> GetAllAsync();

        Task<Instrument> GetByAssetPairIdAsync(string assetPairId);

        Task<Instrument> TryGetByAssetPairIdAsync(string assetPairId);

        Task<Instrument> FindAsync(string assetPairId);

        Task AddAsync(Instrument instrument);

        Task UpdateAsync(Instrument instrument);

        Task DeleteAsync(string assetPairId);

        Task AddLevelAsync(string assetPairId, InstrumentLevel instrumentLevel);

        Task UpdateLevelAsync(string assetPairId, InstrumentLevel instrumentLevel);

        Task RemoveLevelAsync(string assetPairId, string levelId);

        Task RemoveLevelByNumberAsync(string assetPairId, int levelNumber);

        Task<decimal> ApplyVolumeAsync(string assetPairId, decimal volume, TradeType side);

        Task AddCrossInstrumentAsync(string assetPairId, CrossInstrument crossInstrument);

        Task UpdateCrossInstrumentAsync(string assetPairId, CrossInstrument crossInstrument);

        Task RemoveCrossInstrumentAsync(string assetPairId, string crossAssetPairId);

        Task ResetTradingVolumeAsync();
    }
}
