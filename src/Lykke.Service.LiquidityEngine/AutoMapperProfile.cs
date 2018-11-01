using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks;
using Lykke.Service.LiquidityEngine.Client.Models.Audit;
using Lykke.Service.LiquidityEngine.Client.Models.CrossRateInstruments;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;
using Lykke.Service.LiquidityEngine.Client.Models.MarketMaker;
using Lykke.Service.LiquidityEngine.Client.Models.OrderBooks;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.LiquidityEngine.Client.Models.Quotes;
using Lykke.Service.LiquidityEngine.Client.Models.Reports;
using Lykke.Service.LiquidityEngine.Client.Models.Settings;
using Lykke.Service.LiquidityEngine.Client.Models.Trades;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;

namespace Lykke.Service.LiquidityEngine
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AssetPairLink, AssetPairLinkModel>(MemberList.Source);
            CreateMap<AssetPairLinkModel, AssetPairLink>(MemberList.Destination);

            CreateMap<Instrument, InstrumentModel>(MemberList.Source);
            CreateMap<InstrumentModel, Instrument>(MemberList.Destination)
                .ForMember(dest => dest.Levels, opt => opt.Ignore());

            CreateMap<InstrumentLevel, InstrumentLevelModel>(MemberList.Source);
            CreateMap<InstrumentLevelModel, InstrumentLevel>(MemberList.Destination);

            CreateMap<BalanceOperation, BalanceOperationModel>(MemberList.Source);
            CreateMap<BalanceOperationModel, BalanceOperation>(MemberList.Destination);

            CreateMap<ExternalTrade, ExternalTradeModel>(MemberList.Source);
            CreateMap<InternalTrade, InternalTradeModel>(MemberList.Source);

            CreateMap<Position, PositionModel>(MemberList.Source);

            CreateMap<RemainingVolume, RemainingVolumeModel>(MemberList.Source);

            CreateMap<SummaryReport, SummaryReportModel>(MemberList.Source);

            CreateMap<OrderBook, OrderBookModel>(MemberList.Source);
            CreateMap<LimitOrder, LimitOrderModel>(MemberList.Source);

            CreateMap<MarketMakerSettings, MarketMakerSettingsModel>(MemberList.Source);
            CreateMap<MarketMakerSettingsModel, MarketMakerSettings>(MemberList.Destination);
            
            CreateMap<QuoteThresholdSettings, QuoteThresholdSettingsModel>(MemberList.Source);
            CreateMap<QuoteThresholdSettingsModel, QuoteThresholdSettings>(MemberList.Destination);

            CreateMap<QuoteTimeoutSettings, QuoteTimeoutSettingsModel>(MemberList.Source);
            CreateMap<QuoteTimeoutSettingsModel, QuoteTimeoutSettings>(MemberList.Destination);

            CreateMap<TimersSettings, TimersSettingsModel>(MemberList.Source);
            CreateMap<TimersSettingsModel, TimersSettings>(MemberList.Destination);

            CreateMap<MarketMakerState, MarketMakerStateModel>(MemberList.Source);

            CreateMap<Quote, QuoteModel>(MemberList.Source)
                .ForSourceMember(src => src.Mid, opt => opt.Ignore())
                .ForSourceMember(src => src.Spread, opt => opt.Ignore());

            CreateMap<CrossRateInstrument, CrossRateInstrumentModel>(MemberList.Source);
            CreateMap<CrossRateInstrumentModel, CrossRateInstrument>(MemberList.Destination);

        }
    }
}
