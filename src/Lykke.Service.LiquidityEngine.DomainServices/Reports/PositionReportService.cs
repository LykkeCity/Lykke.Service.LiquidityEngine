using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Reports
{
    public class PositionReportService : IPositionReportService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ICrossRateInstrumentService _crossRateInstrumentService;

        public PositionReportService(
            IPositionRepository positionRepository,
            ICrossRateInstrumentService crossRateInstrumentService)
        {
            _positionRepository = positionRepository;
            _crossRateInstrumentService = crossRateInstrumentService;
        }

        public async Task<IReadOnlyCollection<PositionReport>> GetByPeriodAsync(DateTime startDate, DateTime endDate,
            int limit)
        {
            IReadOnlyCollection<Position> positions =
                await _positionRepository.GetAsync(startDate, endDate, limit, null, null);

            var positionReports = new List<PositionReport>();

            foreach (Position position in positions)
            {
                bool isClosed = !string.IsNullOrEmpty(position.CloseTradeId);

                decimal? pnlUsd = await _crossRateInstrumentService
                    .ConvertPriceAsync(position.AssetPairId, position.PnL);

                positionReports.Add(new PositionReport
                {
                    Id = position.Id,
                    AssetPairId = position.AssetPairId,
                    Type = position.Type,
                    Timestamp = position.Date,
                    Price = position.Price,
                    Volume = position.Volume,
                    IsClosed = isClosed,
                    CloseDate = isClosed ? position.CloseDate : default(DateTime?),
                    ClosePrice = isClosed ? position.ClosePrice : default(decimal?),
                    PnL = isClosed ? position.PnL : default(decimal?),
                    PnLUsd = pnlUsd,
                    CrossAsk = position.CrossAsk,
                    CrossBid = position.CrossBid,
                    CrossAssetPairId = position.CrossAssetPairId,
                    TradeAssetPairId = position.TradeAssetPairId,
                    InternalTradesId = position.Trades?.FirstOrDefault(),
                    ExternalTradeId = position.CloseTradeId
                });
            }

            return positionReports;
        }
    }
}
