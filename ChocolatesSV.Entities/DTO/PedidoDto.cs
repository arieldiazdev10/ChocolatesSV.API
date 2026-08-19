using System;
using System.Collections.Generic;
using System.Text;

namespace ChocolatesSV.Entities.DTO
{
    public class PedidoDto
    {
        public int Id { get; set; }

        public string Cliente { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Telefono { get; set; }

        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;

        public DateTime FechaEntrega { get; set; }
    }
}
