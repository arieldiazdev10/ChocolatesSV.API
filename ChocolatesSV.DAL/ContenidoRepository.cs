using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL
{
    public class ContenidoRepository(IDatabaseRepository databaseRepository) : IContenidoRepository
    {
        private static class Queries
        {
            public const string GetCompanyInfo = "SELECT TOP 1 * FROM InformacionEmpresa WHERE Activo = 1 ORDER BY FechaActualizacion DESC, InformacionEmpresaID DESC";
            public const string GetActiveFaq = "SELECT * FROM PreguntasFrecuentes WHERE Activo = 1 ORDER BY Orden, PreguntaID";
        }

        public async Task<InformacionEmpresa?> GetCompanyInfoAsync()
        {
            return await databaseRepository.QueryFirstOrDefaultAsync<InformacionEmpresa>(Queries.GetCompanyInfo);
        }

        public async Task<List<PreguntaFrecuente>> GetActiveFaqAsync()
        {
            return [.. (await databaseRepository.QueryAsync<PreguntaFrecuente>(Queries.GetActiveFaq))];
        }
    }
}