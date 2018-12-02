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
    public class InternalTradeRepository : IInternalTradeRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public InternalTradeRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyCollection<InternalTrade>> GetAsync(DateTime startDate, DateTime endDate, int limit)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                IQueryable<InternalTradeEntity> query = context.InternalTrades
                    .Where(o => o.Timestamp >= startDate && o.Timestamp <= endDate);

                InternalTradeEntity[] entities = await query
                    .OrderByDescending(o => o.Timestamp)
                    .Take(limit)
                    .ToArrayAsync();

                return Mapper.Map<InternalTrade[]>(entities);
            }
        }

        public async Task<InternalTrade> GetByIdAsync(string internalTradeId)
        {
            if (!Guid.TryParse(internalTradeId, out Guid id))
                throw new ArgumentException("Invalid identifier", nameof(internalTradeId));

            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                InternalTradeEntity entity = await context.InternalTrades.FirstOrDefaultAsync(o => o.Id == id);

                return Mapper.Map<InternalTrade>(entity);
            }
        }

        public async Task InsertAsync(InternalTrade internalTrade)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                InternalTradeEntity entity = Mapper.Map<InternalTradeEntity>(internalTrade);

                context.Add(entity);

                await context.SaveChangesAsync();
            }
        }
    }
}
