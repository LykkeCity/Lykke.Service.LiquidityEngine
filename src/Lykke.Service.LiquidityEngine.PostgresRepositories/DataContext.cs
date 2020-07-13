using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.PostgresRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories
{
    public class DataContext : DbContext
    {
        private readonly string _connectionString;

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal DbSet<PositionEntity> Positions { get; set; }

        internal DbSet<InternalTradeEntity> InternalTrades { get; set; }

        internal DbSet<ExternalTradeEntity> ExternalTrades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Positions

            modelBuilder.Entity<PositionEntity>()
                .Property(o => o.Type)
                .HasConversion(new EnumToNumberConverter<PositionType, short>());


            // InternalTrades

            modelBuilder.Entity<InternalTradeEntity>()
                .Property(o => o.Type)
                .HasConversion(new EnumToNumberConverter<TradeType, short>());

            modelBuilder.Entity<InternalTradeEntity>()
                .Property(o => o.Status)
                .HasConversion(new EnumToNumberConverter<TradeStatus, short>());


            // External Trades

            modelBuilder.Entity<ExternalTradeEntity>()
                .Property(o => o.Type)
                .HasConversion(new EnumToNumberConverter<TradeType, short>());
        }
    }
}
