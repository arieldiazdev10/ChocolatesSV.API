using AutoMapper;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL.Profiles
{
    public class DashboardProfile : Profile
    {
        public DashboardProfile()
        {
            CreateMap<ResumenVentas, DashboardSummaryDto>()
                .ForMember(dest => dest.VentasDelDia, opt => opt.MapFrom(src => src.PedidosHoy))
                .ForMember(dest => dest.IngresosDelDia, opt => opt.MapFrom(src => src.IngresosHoy))
                .ForMember(dest => dest.VentasDelMes, opt => opt.MapFrom(src => src.PedidosMes))
                .ForMember(dest => dest.IngresosDelMes, opt => opt.MapFrom(src => src.IngresosMes))
                .ForMember(dest => dest.TicketPromedioMes, opt => opt.Ignore())
                .ForMember(dest => dest.PedidosPorEstado, opt => opt.Ignore());

            CreateMap<PedidosPorEstado, EstadoPedidoDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.EstadoPedido));
        }
    }
}
