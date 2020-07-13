using Lykke.Service.LiquidityEngine.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.LiquidityEngine.MsSqlRepositories
{
    public class DataContext : DbContext
    {
        private readonly string _connectionString;

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal DbSet<OrderBookUpdateReportEntity> OrderBookUpdateReports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildOrderBookUpdateReports(modelBuilder);

            BuildPlacedOrderReports(modelBuilder);
        }

        private void BuildOrderBookUpdateReports(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .ToTable("LEOrderBookUpdateReports")
                .HasKey(c => new { c.Id });

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.IterationId)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.IterationDateTime)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.AssetPair)
                .HasMaxLength(16)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.FirstQuoteAsk)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.FirstQuoteBid)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.SecondQuoteAsk)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.SecondQuoteBid)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.QuoteDateTime)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.GlobalMarkup)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.NoFreshQuoteMarkup)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.PnLStopLossMarkup)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .Property(x => x.FiatEquityMarkup)
                .IsRequired();

            modelBuilder.Entity<OrderBookUpdateReportEntity>()
                .HasMany(x => x.Orders)
                .WithOne()
                .HasForeignKey(x => x.OrderBookUpdateReportId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }

        private void BuildPlacedOrderReports(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlacedOrderReportEntity>()
                .ToTable("LEPlacedOrderReports")
                .HasKey(c => new { c.Id });

            modelBuilder.Entity<PlacedOrderReportEntity>()
                .Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(8)
                .IsRequired();

            modelBuilder.Entity<PlacedOrderReportEntity>()
                .Property(x => x.Price)
                .IsRequired();

            modelBuilder.Entity<PlacedOrderReportEntity>()
                .Property(x => x.Volume)
                .IsRequired();

            modelBuilder.Entity<PlacedOrderReportEntity>()
                .Property(x => x.LevelMarkup)
                .IsRequired();
        }
    }
}
