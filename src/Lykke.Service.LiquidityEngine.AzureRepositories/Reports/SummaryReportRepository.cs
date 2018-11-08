using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Reports
{
    public class SummaryReportRepository : ISummaryReportRepository
    {
        private readonly INoSQLTableStorage<SummaryReportEntity> _storage;

        public SummaryReportRepository(INoSQLTableStorage<SummaryReportEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<SummaryReport>> GetAllAsync()
        {
            IList<SummaryReportEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<SummaryReport[]>(entities);
        }

        public async Task<SummaryReport> GetByAssetPairAsync(string assetPairId, string tradeAssetPairId)
        {
            SummaryReportEntity entity = await _storage.GetDataAsync(GetPartitionKey(assetPairId), GetRowKey(tradeAssetPairId));

            return Mapper.Map<SummaryReport>(entity);
        }

        public async Task InsertAsync(SummaryReport summaryReport)
        {
            var entity = new SummaryReportEntity(GetPartitionKey(summaryReport.AssetPairId), GetRowKey(summaryReport.TradeAssetPairId));

            Mapper.Map(summaryReport, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task UpdateAsync(SummaryReport summaryReport)
        {
            await _storage.MergeAsync(GetPartitionKey(summaryReport.AssetPairId), GetRowKey(summaryReport.TradeAssetPairId), entity =>
            {
                Mapper.Map(summaryReport, entity);
                return entity;
            });
        }

        public async Task DeleteAsync()
        {
            IList<SummaryReportEntity> entities = await _storage.GetDataAsync();

            await _storage.DeleteAsync(entities);
        }

        public Task DeleteAsync(string assetPairId, string tradeAssetPairId)
        {
            return _storage.DeleteAsync(GetPartitionKey(assetPairId), GetRowKey(tradeAssetPairId));
        }

        private static string GetPartitionKey(string assetPairId)
            => assetPairId;

        private static string GetRowKey(string tradeAssetPairId)
            => tradeAssetPairId;
    }
}
