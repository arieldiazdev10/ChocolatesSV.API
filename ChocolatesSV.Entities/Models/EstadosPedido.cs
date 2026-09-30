namespace ChocolatesSV.Entities.Models
{
    public static class EstadosPedido
    {
        public const string Pendiente = "Pendiente";
        public const string Confirmado = "Confirmado";
        public const string EnPreparacion = "En Preparacion";
        public const string Enviado = "Enviado";
        public const string Entregado = "Entregado";
        public const string Cancelado = "Cancelado";

        public static readonly IReadOnlyList<string> Todos =
            [Pendiente, Confirmado, EnPreparacion, Enviado, Entregado, Cancelado];

        private static readonly Dictionary<string, string[]> Transiciones = new()
        {
            [Pendiente] = [Confirmado, Cancelado],
            [Confirmado] = [EnPreparacion, Enviado, Cancelado],
            [EnPreparacion] = [Enviado, Cancelado],
            [Enviado] = [Entregado],
            [Entregado] = [],
            [Cancelado] = []
        };

        public static string? Normalizar(string? estado)
        {
            var valor = estado?.Trim();
            return Todos.FirstOrDefault(e => string.Equals(e, valor, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EsTransicionValida(string actual, string nuevo)
        {
            return Transiciones.TryGetValue(actual, out var permitidos) && permitidos.Contains(nuevo);
        }

        public static List<string> ObtenerSiguientes(string actual)
        {
            if (Transiciones.TryGetValue(actual, out var permitidos))
            {
                return [.. permitidos];
            }

            return [];
        }
    }
}