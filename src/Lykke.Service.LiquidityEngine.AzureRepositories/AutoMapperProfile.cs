using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.AzureRepositories.AssetPairLinks;
using Lykke.Service.LiquidityEngine.AzureRepositories.Instruments;
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
        }
    }
}
