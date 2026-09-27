using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL
{
    public class ContentService : IContentService
    {
        public Task<AboutDto> GetAboutAsync()
        {
            return Task.FromResult(
                new AboutDto
                {
                    NombreEmpresa = "Chocolates SV",

                    Descripcion =
                        "Emprendimiento salvadoreño dedicado a la elaboración y venta de chocolates artesanales personalizados.",

                    Vision =
                        "Ofrecer productos artesanales de alta calidad para ocasiones especiales.",

                    Valores =
                        "Calidad, compromiso, creatividad y satisfacción al cliente."
                });
        }

        public Task<List<FaqDto>> GetFaqAsync()
        {
            var preguntas = new List<FaqDto>
            {
                new()
                {
                    Pregunta = "¿Cuánto tarda una entrega?",
                    Respuesta = "Entre 24 y 72 horas dependiendo de la ubicación."
                },

                new()
                {
                    Pregunta = "¿Puedo personalizar mis chocolates?",
                    Respuesta = "Sí, ofrecemos opciones personalizadas para eventos especiales."
                },

                new()
                {
                    Pregunta = "¿Qué métodos de pago aceptan?",
                    Respuesta = "Pago simulado, contra entrega y transferencia bancaria."
                }
            };

            return Task.FromResult(preguntas);
        }
    }
}