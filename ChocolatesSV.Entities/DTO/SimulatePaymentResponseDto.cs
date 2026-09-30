namespace ChocolatesSV.Entities.DTO
{
    public class SimulatePaymentResponseDto
    {
        public bool Aprobado { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public decimal Monto { get; set; }
    }
}
