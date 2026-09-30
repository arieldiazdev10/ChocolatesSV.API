using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class TrackingRequestDto
    {
        [Required]
        public int PedidoId { get; set; }

        [Required]
        public string Correo { get; set; } = string.Empty;
    }
}