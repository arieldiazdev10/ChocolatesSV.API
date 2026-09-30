using AutoMapper;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL.Profiles
{
    public class ContenidoProfile : Profile
    {
        public ContenidoProfile()
        {
            CreateMap<InformacionEmpresa, InformacionEmpresaDto>();

            CreateMap<PreguntaFrecuente, PreguntaFrecuenteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PreguntaID));
        }
    }
}