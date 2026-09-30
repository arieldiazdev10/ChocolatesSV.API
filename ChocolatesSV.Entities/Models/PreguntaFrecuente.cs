using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.Models
{
    public class PreguntaFrecuente
    {
        [Key]
        public int PreguntaID { get; set; }
        public string Pregunta { get; set; } = string.Empty;
        public string Respuesta { get; set; } = string.Empty;
        public int Orden { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}