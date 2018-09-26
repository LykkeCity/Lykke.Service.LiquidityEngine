using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks;
using Lykke.Service.LiquidityEngine.Client.Models.Audit;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.LiquidityEngine.Client.Models.Reports;
using Lykke.Service.LiquidityEngine.Client.Models.Trades;
using Lykke.Service.LiquidityEngine.Domain;

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
            
            CreateMap<LevelVolume, LevelVolumeModel>(MemberList.Source);
            CreateMap<LevelVolumeModel, LevelVolume>(MemberList.Destination);
            
            CreateMap<BalanceOperation, BalanceOperationModel>(MemberList.Source);
            CreateMap<BalanceOperationModel, BalanceOperation>(MemberList.Destination);
            
            CreateMap<ExternalTrade, ExternalTradeModel>(MemberList.Source);
            CreateMap<InternalTrade, InternalTradeModel>(MemberList.Source);
            
            CreateMap<Position, PositionModel>(MemberList.Source);
            
            CreateMap<SummaryReport, SummaryReportModel>(MemberList.Source);
        }
    }
}
