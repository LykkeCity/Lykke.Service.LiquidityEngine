﻿using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.AzureRepositories.AssetPairLinks;
using Lykke.Service.LiquidityEngine.AzureRepositories.BalanceOperations;
using Lykke.Service.LiquidityEngine.AzureRepositories.Credits;
using Lykke.Service.LiquidityEngine.AzureRepositories.CrossRateInstruments;
using Lykke.Service.LiquidityEngine.AzureRepositories.Instruments;
using Lykke.Service.LiquidityEngine.AzureRepositories.MarketMaker;
using Lykke.Service.LiquidityEngine.AzureRepositories.Positions;
using Lykke.Service.LiquidityEngine.AzureRepositories.Reports;
using Lykke.Service.LiquidityEngine.AzureRepositories.Settings;
using Lykke.Service.LiquidityEngine.AzureRepositories.Trades;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;

namespace Lykke.Service.LiquidityEngine.AzureRepositories
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AssetPairLink, AssetPairLinkEntity>(MemberList.Source);
            CreateMap<AssetPairLinkEntity, AssetPairLink>(MemberList.Destination);

            CreateMap<CrossRateInstrument, CrossRateInstrumentEntity>(MemberList.Source);
            CreateMap<CrossRateInstrumentEntity, CrossRateInstrument>(MemberList.Destination);

            CreateMap<Instrument, InstrumentEntity>(MemberList.Source)
                .ForSourceMember(src => src.CrossInstruments, opt => opt.Ignore());
            CreateMap<InstrumentEntity, Instrument>(MemberList.Destination)
                .ForMember(src => src.CrossInstruments, opt => opt.Ignore());

            CreateMap<TimersSettings, TimersSettingsEntity>(MemberList.Source);
            CreateMap<TimersSettingsEntity, TimersSettings>(MemberList.Destination);

            CreateMap<BalanceOperation, BalanceOperationEntity>(MemberList.Source);
            CreateMap<BalanceOperationEntity, BalanceOperation>(MemberList.Destination);

            CreateMap<Credit, CreditEntity>(MemberList.Source);
            CreateMap<CreditEntity, Credit>(MemberList.Destination);

            CreateMap<InternalTrade, InternalTradeEntity>(MemberList.Source);
            CreateMap<InternalTradeEntity, InternalTrade>(MemberList.Destination);

            CreateMap<ExternalTrade, ExternalTradeEntity>(MemberList.Source);
            CreateMap<ExternalTradeEntity, ExternalTrade>(MemberList.Destination);

            CreateMap<Position, PositionEntity>(MemberList.Source);
            CreateMap<PositionEntity, Position>(MemberList.Destination);

            CreateMap<RemainingVolume, RemainingVolumeEntity>(MemberList.Source);
            CreateMap<RemainingVolumeEntity, RemainingVolume>(MemberList.Destination);

            CreateMap<SummaryReport, SummaryReportEntity>(MemberList.Source);
            CreateMap<SummaryReportEntity, SummaryReport>(MemberList.Destination);

            CreateMap<MarketMakerSettings, MarketMakerSettingsEntity>(MemberList.Source);
            CreateMap<MarketMakerSettingsEntity, MarketMakerSettings>(MemberList.Destination);
            
            CreateMap<QuoteTimeoutSettings, QuoteTimeoutSettingsEntity>(MemberList.Source);
            CreateMap<QuoteTimeoutSettingsEntity, QuoteTimeoutSettings>(MemberList.Destination);

            CreateMap<QuoteThresholdSettings, QuoteThresholdSettingsEntity>(MemberList.Source);
            CreateMap<QuoteThresholdSettingsEntity, QuoteThresholdSettings>(MemberList.Destination);

            CreateMap<MarketMakerState, MarketMakerStateEntity>(MemberList.Source);
            CreateMap<MarketMakerStateEntity, MarketMakerState>(MemberList.Destination);
            
            CreateMap<CrossInstrument, CrossInstrumentEntity>(MemberList.Source);
            CreateMap<CrossInstrumentEntity, CrossInstrument>(MemberList.Destination);
        }
    }
}
