using AutoMapper;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL
{
    public class ContenidoService(IContenidoRepository contenidoRepository, IMapper mapper) : IContenidoService
    {
        public async Task<InformacionEmpresaDto?> GetCompanyInfoAsync()
        {
            var info = await contenidoRepository.GetCompanyInfoAsync();
            return info is null ? null : mapper.Map<InformacionEmpresaDto>(info);
        }

        public async Task<List<PreguntaFrecuenteDto>> GetFaqAsync()
        {
            var preguntas = await contenidoRepository.GetActiveFaqAsync();
            return mapper.Map<List<PreguntaFrecuenteDto>>(preguntas);
        }
    }
}