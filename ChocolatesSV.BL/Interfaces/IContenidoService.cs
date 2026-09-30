using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IContenidoService
    {
        public Task<InformacionEmpresaDto?> GetCompanyInfoAsync();
        public Task<List<PreguntaFrecuenteDto>> GetFaqAsync();
    }
}