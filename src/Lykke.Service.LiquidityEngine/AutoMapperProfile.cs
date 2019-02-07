using System.ComponentModel;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Common.InternalExchange.Client.Models;
using Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks;
using Lykke.Service.LiquidityEngine.Client.Models.AssetSettings;
using Lykke.Service.LiquidityEngine.Client.Models.Audit;
using Lykke.Service.LiquidityEngine.Client.Models.CrossRateInstruments;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;
using Lykke.Service.LiquidityEngine.Client.Models.InternalOrders;
using Lykke.Service.LiquidityEngine.Client.Models.MarketMaker;
using Lykke.Service.LiquidityEngine.Client.Models.OrderBooks;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossSettings;
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

            CreateMap<AssetSettings, AssetSettingsModel>(MemberList.Source);
            CreateMap<AssetSettingsModel, AssetSettings>(MemberList.Destination);

            CreateMap<Instrument, InstrumentModel>(MemberList.Source);
            CreateMap<InstrumentEditModel, Instrument>(MemberList.Destination)
                .ForMember(dest => dest.Levels, opt => opt.Ignore())
                .ForMember(dest => dest.CrossInstruments, opt => opt.Ignore());

            CreateMap<InstrumentLevel, InstrumentLevelModel>(MemberList.Source);
            CreateMap<InstrumentLevelModel, InstrumentLevel>(MemberList.Destination);
            CreateMap<InstrumentLevelAddModel, InstrumentLevel>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<InternalOrder, InternalOrderModel>(MemberList.Source);

            CreateMap<InternalOrder, OrderModel>(MemberList.Destination)
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AssetPair, opt => opt.MapFrom(src => src.AssetPairId))
                .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.OriginalVolume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ToModel(src.Status)));

            CreateMap<InternalOrder, Common.ExchangeAdapter.SpotController.Records.OrderModel>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.AssetPairId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.OriginalVolume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.AvgExecutionPrice, opt => opt.MapFrom(src => src.ExecutedPrice ?? 0))
                .ForMember(dest => dest.ExecutedVolume, opt => opt.MapFrom(src => src.ExecutedVolume ?? 0))
                .ForMember(dest => dest.RemainingAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TradeType, opt => opt.MapFrom(src => ToSpotModel(src.Type)))
                .ForMember(dest => dest.ExecutionStatus, opt => opt.MapFrom(src => ToSpotModel(src.Status)));

            CreateMap<BalanceOperation, BalanceOperationModel>(MemberList.Source);
            CreateMap<BalanceOperationModel, BalanceOperation>(MemberList.Destination);

            CreateMap<ExternalTrade, ExternalTradeModel>(MemberList.Source);
            CreateMap<InternalTrade, InternalTradeModel>(MemberList.Source);
            CreateMap<SettlementTrade, SettlementTradeModel>(MemberList.Source);

            CreateMap<Position, PositionModel>(MemberList.Source)
                .ForMember(dest => dest.Trades, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.TradeId)
                    ? new string[0]
                    : new[] {src.TradeId}));

            CreateMap<RemainingVolume, RemainingVolumeModel>(MemberList.Source);

            CreateMap<PositionSummaryReport, SummaryReportModel>(MemberList.Source);

            CreateMap<BalanceIndicatorsReport, BalanceIndicatorsReportModel>(MemberList.Source);

            CreateMap<BalanceReport, BalanceReportModel>(MemberList.Source);

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
                .ForSourceMember(src => src.Mid, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.Spread, opt => opt.DoNotValidate());

            CreateMap<CrossRateInstrument, CrossRateInstrumentModel>(MemberList.Source);
            CreateMap<CrossRateInstrumentModel, CrossRateInstrument>(MemberList.Destination);

            CreateMap<PnLStopLossSettings, PnLStopLossSettingsModel>(MemberList.Source);
            CreateMap<PnLStopLossSettingsModel, PnLStopLossSettings>(MemberList.Destination);

            CreateMap<PnLStopLossEngine, PnLStopLossEngineModel>(MemberList.Source);
            CreateMap<PnLStopLossEngineModel, PnLStopLossEngine>(MemberList.Destination);

            CreateMap<AssetPairMarkup, AssetPairMarkupModel>(MemberList.Source);
            CreateMap<AssetPairMarkupModel, AssetPairMarkup>(MemberList.Destination);
        }

        private static OrderStatus ToModel(Domain.InternalOrderStatus internalOrderStatus)
        {
            switch (internalOrderStatus)
            {
                case Domain.InternalOrderStatus.New:
                case Domain.InternalOrderStatus.Reserved:
                case Domain.InternalOrderStatus.Executed:
                case Domain.InternalOrderStatus.Transferred:
                case Domain.InternalOrderStatus.Failed:
                    return OrderStatus.InProgress;
                case Domain.InternalOrderStatus.Rejected:
                case Domain.InternalOrderStatus.Cancelled:
                    return OrderStatus.Reject;
                case Domain.InternalOrderStatus.Completed:
                    return OrderStatus.Done;
                default:
                    throw new InvalidEnumArgumentException(nameof(internalOrderStatus), (int) internalOrderStatus,
                        typeof(Domain.InternalOrderStatus));
            }
        }

        private static Common.ExchangeAdapter.SpotController.Records.OrderStatus ToSpotModel(
            Domain.InternalOrderStatus internalOrderStatus)
        {
            switch (internalOrderStatus)
            {
                case Domain.InternalOrderStatus.New:
                case Domain.InternalOrderStatus.Reserved:
                case Domain.InternalOrderStatus.Executed:
                case Domain.InternalOrderStatus.Transferred:
                case Domain.InternalOrderStatus.Failed:
                    return Common.ExchangeAdapter.SpotController.Records.OrderStatus.Active;
                case Domain.InternalOrderStatus.Rejected:
                case Domain.InternalOrderStatus.Cancelled:
                    return Common.ExchangeAdapter.SpotController.Records.OrderStatus.Canceled;
                case Domain.InternalOrderStatus.Completed:
                    return Common.ExchangeAdapter.SpotController.Records.OrderStatus.Fill;
                default:
                    throw new InvalidEnumArgumentException(nameof(internalOrderStatus), (int) internalOrderStatus,
                        typeof(Domain.InternalOrderStatus));
            }
        }

        private static Common.ExchangeAdapter.Contracts.TradeType ToSpotModel(Domain.LimitOrderType limitOrderType)
        {
            switch (limitOrderType)
            {
                case Domain.LimitOrderType.Sell:
                    return Common.ExchangeAdapter.Contracts.TradeType.Sell;
                case Domain.LimitOrderType.Buy:
                    return Common.ExchangeAdapter.Contracts.TradeType.Buy;
                default:
                    throw new InvalidEnumArgumentException(nameof(limitOrderType), (int) limitOrderType,
                        typeof(Domain.LimitOrderType));
            }
        }
    }
}
