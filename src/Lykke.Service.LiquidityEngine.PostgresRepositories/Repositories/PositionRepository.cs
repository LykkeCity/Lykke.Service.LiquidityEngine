using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.PostgresRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public PositionRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyCollection<Position>> GetAsync(DateTime startDate, DateTime endDate, int limit,
            string assetPairId, string tradeAssetPairId)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                IQueryable<PositionEntity> query = context.Positions
                    .Where(o => o.Timestamp >= startDate && o.Timestamp <= endDate);

                if (!string.IsNullOrEmpty(assetPairId))
                    query = query.Where(o => o.AssetPairId == assetPairId);

                if (!string.IsNullOrEmpty(tradeAssetPairId))
                    query = query.Where(o => o.TradeAssetPairId == tradeAssetPairId);

                PositionEntity[] entities = await query
                    .OrderByDescending(o => o.Timestamp)
                    .Take(limit)
                    .ToArrayAsync();

                return Mapper.Map<Position[]>(entities);
            }
        }

        public async Task<IReadOnlyCollection<SummaryReport>> GetReportAsync(DateTime startDate, DateTime endDate)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                return await context.Positions
                    .Where(o => o.Timestamp >= startDate && o.Timestamp <= endDate)
                    .Join(context.InternalTrades,
                        position => position.InternalTradeId,
                        internalTrade => internalTrade.Id,
                        (position, internalTrade) =>
                            new
                            {
                                PositionId = position.Id,
                                position.AssetPairId,
                                position.TradeAssetPairId,
                                position.Type,
                                position.Timestamp,
                                position.Price,
                                position.Volume,
                                position.PnL,
                                OpenPosition = position.ExternalTradeId == null
                                    ? 1
                                    : 0,
                                ClosedPosition = position.ExternalTradeId != null
                                    ? 1
                                    : 0,
                                BuyTrade = internalTrade != null && internalTrade.Type == TradeType.Buy
                                    ? 1
                                    : 0,
                                SellTrade = internalTrade != null && internalTrade.Type == TradeType.Sell
                                    ? 1
                                    : 0,
                                BaseAssetVolume = internalTrade != null
                                    ? internalTrade.Type == TradeType.Buy
                                        ? internalTrade.Volume
                                        : -internalTrade.Volume
                                    : 0,
                                QuoteAssetVolume = internalTrade != null
                                    ? internalTrade.Type == TradeType.Buy
                                        ? -internalTrade.OppositeVolume
                                        : internalTrade.OppositeVolume
                                    : 0,
                                BaseAssetSellVolume = internalTrade != null
                                    ? internalTrade.Type == TradeType.Sell
                                        ? internalTrade.Volume
                                        : 0
                                    : 0,
                                BaseAssetBuyVolume = internalTrade != null
                                    ? internalTrade.Type == TradeType.Buy
                                        ? internalTrade.Volume
                                        : 0
                                    : 0,
                                QuoteAssetSellVolume = internalTrade != null
                                    ? internalTrade.Type == TradeType.Buy
                                        ? internalTrade.OppositeVolume
                                        : 0
                                    : 0,
                                QuoteAssetBuyVolume = internalTrade != null
                                    ? internalTrade.Type == TradeType.Sell
                                        ? internalTrade.OppositeVolume
                                        : 0
                                    : 0
                            })
                    .GroupBy(o => new {o.TradeAssetPairId, o.AssetPairId})
                    .Select(o => new SummaryReport
                    {
                        AssetPairId = o.Key.AssetPairId,
                        TradeAssetPairId = o.Key.TradeAssetPairId,
                        OpenPositionsCount = o.Sum(p => p.OpenPosition),
                        ClosedPositionsCount = o.Sum(p => p.ClosedPosition),
                        PnL = o.Sum(p => p.PnL),
                        BaseAssetVolume = o.Sum(p => p.BaseAssetVolume),
                        QuoteAssetVolume = o.Sum(p => p.QuoteAssetVolume),
                        TotalSellBaseAssetVolume = o.Sum(p => p.BaseAssetSellVolume),
                        TotalBuyBaseAssetVolume = o.Sum(p => p.BaseAssetBuyVolume),
                        TotalSellQuoteAssetVolume = o.Sum(p => p.QuoteAssetSellVolume),
                        TotalBuyQuoteAssetVolume = o.Sum(p => p.QuoteAssetBuyVolume),
                        SellTradesCount = o.Sum(p => p.SellTrade),
                        BuyTradesCount = o.Sum(p => p.BuyTrade)
                    })
                    .ToArrayAsync();
            }
        }

        public async Task<Position> GetByIdAsync(string positionId)
        {
            if (!Guid.TryParse(positionId, out Guid id))
                throw new ArgumentException("Invalid identifier", nameof(positionId));

            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                PositionEntity entity = await context.Positions.FirstOrDefaultAsync(o => o.Id == id);

                return Mapper.Map<Position>(entity);
            }
        }

        public async Task InsertAsync(Position position)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                PositionEntity entity = Mapper.Map<PositionEntity>(position);

                context.Add(entity);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Position position)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                Guid id = Guid.Parse(position.Id);

                PositionEntity entity = await context.Positions
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (entity != null)
                {
                    Mapper.Map(position, entity);

                    context.Positions.Update(entity);

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteAsync()
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                await context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE [TableName]");
            }
        }

        public async Task DeleteAsync(string positionId)
        {
            if (!Guid.TryParse(positionId, out Guid id))
                throw new ArgumentException("Invalid identifier", nameof(positionId));

            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                context.Positions.Remove(new PositionEntity {Id = id});

                await context.SaveChangesAsync();
            }
        }
    }
}
