using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface IContenidoRepository
    {
        public Task<InformacionEmpresa?> GetCompanyInfoAsync();
        public Task<List<PreguntaFrecuente>> GetActiveFaqAsync();
    }
}