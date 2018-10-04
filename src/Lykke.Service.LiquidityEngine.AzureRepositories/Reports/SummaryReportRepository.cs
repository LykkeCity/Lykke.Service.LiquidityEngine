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

        public async Task<SummaryReport> GetByAssetPairAsync(string assetPairId)
        {
            SummaryReportEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey(assetPairId));

            return Mapper.Map<SummaryReport>(entity);
        }

        public async Task InsertAsync(SummaryReport summaryReport)
        {
            var entity = new SummaryReportEntity(GetPartitionKey(), GetRowKey(summaryReport.AssetPairId));

            Mapper.Map(summaryReport, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task UpdateAsync(SummaryReport summaryReport)
        {
            await _storage.MergeAsync(GetPartitionKey(), GetRowKey(summaryReport.AssetPairId), entity =>
            {
                Mapper.Map(summaryReport, entity);
                return entity;
            });
        }

        public Task DeleteAsync()
        {
            return _storage.DeleteAsync();
        }

        public Task DeleteAsync(string assetPairId)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(assetPairId));
        }
        
        private static string GetPartitionKey()
            => "SummaryReport";

        private static string GetRowKey(string assetPairId)
            => assetPairId;
    }
}
