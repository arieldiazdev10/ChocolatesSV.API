using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.Models
{
    public class InformacionEmpresa
    {
        [Key]
        public int InformacionEmpresaID { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Mision { get; set; }
        public string? Vision { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? HorarioAtencion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}