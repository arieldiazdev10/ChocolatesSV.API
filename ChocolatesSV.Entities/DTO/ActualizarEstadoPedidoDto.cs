using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class ActualizarEstadoPedidoDto
    {
        [Required]
        public string Estado { get; set; } = string.Empty;
    }
}