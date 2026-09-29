namespace ChocolatesSV.Entities.DTO
{
    public class ValidarCuponResponseDto
    {
        public bool Valido { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Codigo { get; set; }
        public string? TipoDescuento { get; set; }
        public decimal Valor { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal TotalConDescuento { get; set; }
    }
}
