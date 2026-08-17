using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.Models
{
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }
        public int CategoriaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? URLImagen { get; set; }
        public bool Destacado { get; set; }
        public int Existencias { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacionID { get; set; }
        public int? UsuarioModificacionID { get; set; }
    }
}

