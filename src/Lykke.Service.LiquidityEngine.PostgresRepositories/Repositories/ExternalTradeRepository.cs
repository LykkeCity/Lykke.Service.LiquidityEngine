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
    public class ExternalTradeRepository : IExternalTradeRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public ExternalTradeRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyCollection<ExternalTrade>> GetAsync(DateTime startDate, DateTime endDate, int limit)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                IQueryable<ExternalTradeEntity> query = context.ExternalTrades
                    .Where(o => o.Timestamp >= startDate && o.Timestamp <= endDate);

                ExternalTradeEntity[] entities = await query
                    .OrderByDescending(o => o.Timestamp)
                    .Take(limit)
                    .ToArrayAsync();

                return Mapper.Map<ExternalTrade[]>(entities);
            }
        }

        public async Task<ExternalTrade> GetByIdAsync(string externalTradeId)
        {
            if (!Guid.TryParse(externalTradeId, out Guid id))
                throw new ArgumentException("Invalid identifier", nameof(externalTradeId));

            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                ExternalTradeEntity entity = await context.ExternalTrades.FirstOrDefaultAsync(o => o.Id == id);

                return Mapper.Map<ExternalTrade>(entity);
            }
        }

        public async Task InsertAsync(ExternalTrade externalTrade)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                ExternalTradeEntity entity = Mapper.Map<ExternalTradeEntity>(externalTrade);

                context.Add(entity);

                await context.SaveChangesAsync();
            }
        }
    }
}
