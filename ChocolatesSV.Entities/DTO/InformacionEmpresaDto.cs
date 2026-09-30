namespace ChocolatesSV.Entities.DTO
{
    public class InformacionEmpresaDto
    {
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Mision { get; set; }
        public string? Vision { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? HorarioAtencion { get; set; }
    }
}