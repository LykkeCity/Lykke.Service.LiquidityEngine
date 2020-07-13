using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.MsSqlRepositories.Entities;

namespace Lykke.Service.LiquidityEngine.MsSqlRepositories.Repositories
{
    public class OrderBookUpdateReportRepository : IOrderBookUpdateReportRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public OrderBookUpdateReportRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InsertAsync(OrderBookUpdateReport orderBookUpdateReport)
        {
            OrderBookUpdateReportEntity entity = Mapper.Map<OrderBookUpdateReportEntity>(orderBookUpdateReport);

            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                await context.OrderBookUpdateReports.AddAsync(entity);

                await context.SaveChangesAsync();
            }
        }
    }
}
