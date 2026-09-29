using AutoMapper;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL.Profiles
{
    public class PromocionProfile : Profile
    {
        public PromocionProfile()
        {
            CreateMap<Promocion, PromocionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PromocionID))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.TipoPromocion))
                .ForMember(dest => dest.Descuento, opt => opt.MapFrom(src => src.ValorDescuento))
                .ForMember(dest => dest.Cupon, opt => opt.MapFrom(src => src.CodigoCupon))
                .ForMember(dest => dest.CompraMinima, opt => opt.MapFrom(src => src.MontoMinimoCompra))
                .ForMember(dest => dest.LimiteUsos, opt => opt.MapFrom(src => src.UsosMaximos))
                .ForMember(dest => dest.UsosRealizados, opt => opt.MapFrom(src => src.UsosActuales))
                .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.CategoriaID))
                .ForMember(dest => dest.Activa, opt => opt.MapFrom(src => src.Activo))
                .ReverseMap();

            CreateMap<PromocionProducto, PromocionProductoDto>()
                .ForMember(dest => dest.IdProducto, opt => opt.MapFrom(src => src.ProductoID))
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.PrecioProducto, opt => opt.MapFrom(src => src.Precio))
                .ReverseMap();
        }
    }
}
