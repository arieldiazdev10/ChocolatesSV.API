using System;
using System.Collections.Generic;
using System.Text;

namespace ChocolatesSV.Entities.DTO
{
    public class TrackingResponseDto
    {
        public int PedidoId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public DateTime FechaEntrega { get; set; }
    }
}