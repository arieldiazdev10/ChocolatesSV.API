using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.Models
{
    public class Categoria
    {
        [Key]
        public int CategoriaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
