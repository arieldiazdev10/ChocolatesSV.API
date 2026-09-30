using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.Models
{
    public class Pedido
    {
        [Key]
        public int PedidoID { get; set; }
        public string NombreCliente { get; set; } = string.Empty;

        public string CorreoCliente { get; set; } = string.Empty;

        public string TelefonoCliente { get; set; }

        public DateTime FechaEntrega { get; set; }

        public string? Comentarios { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DescuentoAplicado { get; set; }

        public decimal Total { get; set; }

        public string EstadoPedido { get; set; } = string.Empty;

        public string MetodoPago { get; set; } = string.Empty;

        public string? ReferenciaPago { get; set; }

        public DateTime FechaCreacion { get; set; }

    }
}
