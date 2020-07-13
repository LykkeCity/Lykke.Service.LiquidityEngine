using AutoMapper;
using Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates;
using Lykke.Service.LiquidityEngine.MsSqlRepositories.Entities;

namespace Lykke.Service.LiquidityEngine.MsSqlRepositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<OrderBookUpdateReport, OrderBookUpdateReportEntity>(MemberList.Destination);
            CreateMap<OrderBookUpdateReportEntity, OrderBookUpdateReport>(MemberList.Destination);
        }
    }
}
