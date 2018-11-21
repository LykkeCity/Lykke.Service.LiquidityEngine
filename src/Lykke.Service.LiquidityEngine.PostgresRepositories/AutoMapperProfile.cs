using System;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.PostgresRepositories.Entities;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExternalTrade, ExternalTradeEntity>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => Guid.Parse(src.RequestId)));
            CreateMap<ExternalTradeEntity, ExternalTrade>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString("D")))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestId.ToString("D")));

            CreateMap<InternalTrade, InternalTradeEntity>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.OppositeVolume, opt => opt.MapFrom(src => src.OppositeSideVolume))
                .ForMember(dest => dest.LimitOrderId, opt => opt.MapFrom(src => Guid.Parse(src.LimitOrderId)))
                .ForMember(dest => dest.ExchangeOrderId, opt => opt.MapFrom(src => Guid.Parse(src.ExchangeOrderId)))
                .ForMember(dest => dest.OppositeClientId, opt => opt.MapFrom(src => Guid.Parse(src.OppositeClientId)))
                .ForMember(dest => dest.OppositeLimitOrderId,
                    opt => opt.MapFrom(src => Guid.Parse(src.OppositeLimitOrderId)));
            CreateMap<InternalTradeEntity, InternalTrade>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString("D")))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.OppositeSideVolume, opt => opt.MapFrom(src => src.OppositeVolume))
                .ForMember(dest => dest.LimitOrderId, opt => opt.MapFrom(src => src.LimitOrderId.ToString("D")))
                .ForMember(dest => dest.ExchangeOrderId, opt => opt.MapFrom(src => src.ExchangeOrderId.ToString("D")))
                .ForMember(dest => dest.OppositeClientId, opt => opt.MapFrom(src => src.OppositeClientId.ToString("D")))
                .ForMember(dest => dest.OppositeLimitOrderId,
                    opt => opt.MapFrom(src => src.OppositeLimitOrderId.ToString("D")));

            CreateMap<Position, PositionEntity>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.InternalTradeId,
                    opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.TradeId)
                        ? Guid.Parse(src.TradeId)
                        : default(Guid?)))
                .ForMember(dest => dest.ExternalTradeId,
                    opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.CloseTradeId)
                        ? Guid.Parse(src.CloseTradeId)
                        : default(Guid?)));
            CreateMap<PositionEntity, Position>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString("D")))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.TradeId,
                    opt => opt.MapFrom(src => src.InternalTradeId != null
                        ? src.InternalTradeId.Value.ToString("D")
                        : null))
                .ForMember(dest => dest.CloseTradeId,
                    opt => opt.MapFrom(src => src.ExternalTradeId != null
                        ? src.ExternalTradeId.Value.ToString("D")
                        : null));
        }
    }
}
