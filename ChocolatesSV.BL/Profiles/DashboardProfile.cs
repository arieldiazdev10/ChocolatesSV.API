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

            CreateMap<ProductoMasVendido, TopProductoDto>()
    .ForMember(dest => dest.IdProducto, opt => opt.MapFrom(src => src.ProductoID))
    .ForMember(dest => dest.Producto, opt => opt.MapFrom(src => src.Nombre))
    .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.URLImagen))
    .ForMember(dest => dest.UnidadesVendidas, opt => opt.MapFrom(src => src.CantidadVendida))
    .ForMember(dest => dest.Ingresos, opt => opt.MapFrom(src => src.TotalVendido))
    .ForMember(dest => dest.Posicion, opt => opt.Ignore());
        }
    }
}
