using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.AzureRepositories.AssetPairLinks;
using Lykke.Service.LiquidityEngine.AzureRepositories.BalanceOperations;
using Lykke.Service.LiquidityEngine.AzureRepositories.Credits;
using Lykke.Service.LiquidityEngine.AzureRepositories.Instruments;
using Lykke.Service.LiquidityEngine.AzureRepositories.Positions;
using Lykke.Service.LiquidityEngine.AzureRepositories.Reports;
using Lykke.Service.LiquidityEngine.AzureRepositories.Settings;
using Lykke.Service.LiquidityEngine.AzureRepositories.Trades;
using Lykke.Service.LiquidityEngine.Domain;

namespace Lykke.Service.LiquidityEngine.AzureRepositories
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AssetPairLink, AssetPairLinkEntity>(MemberList.Source);
            CreateMap<AssetPairLinkEntity, AssetPairLink>(MemberList.Destination);

            CreateMap<Instrument, InstrumentEntity>(MemberList.Source)
                .ForSourceMember(src => src.Levels, opt => opt.Ignore())
                .ForMember(dest => dest.Levels,
                    opt => opt.MapFrom(src => (src.Levels ?? new LevelVolume[0]).OrderBy(o => o.Number)
                        .Select(o => o.Volume)));
            
            CreateMap<InstrumentEntity, Instrument>(MemberList.Destination)
                .ForMember(dest => dest.Levels, opt => opt.MapFrom(src => (src.Levels ?? new decimal[0])
                    .Select((volume, index) => new LevelVolume
                    {
                        Number = index + 1,
                        Volume = volume
                    })));
            
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
            
            CreateMap<SummaryReport, SummaryReportEntity>(MemberList.Source);
            CreateMap<SummaryReportEntity, SummaryReport>(MemberList.Destination);
        }
    }
}
