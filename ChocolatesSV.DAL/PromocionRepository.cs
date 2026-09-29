using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.DAL
{
    public class PromocionRepository(IDatabaseRepository databaseRepository) : IPromocionRepository
    {
        private static class Queries
        {
            public const string GetActive = @"SELECT * FROM Promociones
                                              WHERE Activo = 1
                                              AND FechaInicio <= GETDATE()
                                              AND FechaFin >= CAST(GETDATE() AS DATE)
                                              ORDER BY FechaFin";

            public const string GetById = "SELECT * FROM Promociones WHERE PromocionID = @PromocionID";

            public const string GetByCoupon = "SELECT * FROM Promociones WHERE CodigoCupon = @CodigoCupon";

            public const string CouponExists = @"SELECT COUNT(1) FROM Promociones
                                                 WHERE CodigoCupon = @CodigoCupon
                                                 AND (@ExcluirID IS NULL OR PromocionID <> @ExcluirID)";

            public const string GetProducts = @"SELECT pp.PromocionID, pp.ProductoID, pp.Cantidad, p.Nombre, p.Precio
                                                FROM PromocionProductos pp
                                                INNER JOIN Productos p ON p.ProductoID = pp.ProductoID
                                                WHERE pp.PromocionID IN @Ids";

            public const string Insert = @"INSERT INTO Promociones (Nombre, Descripcion, TipoPromocion, TipoDescuento, ValorDescuento, PrecioCombo, CodigoCupon, MontoMinimoCompra, UsosMaximos, CategoriaID, FechaInicio, FechaFin, Activo)
                                           VALUES (@Nombre, @Descripcion, @TipoPromocion, @TipoDescuento, @ValorDescuento, @PrecioCombo, @CodigoCupon, @MontoMinimoCompra, @UsosMaximos, @CategoriaID, @FechaInicio, @FechaFin, @Activo);
                                           SELECT CAST(SCOPE_IDENTITY() AS int);";

            public const string Update = @"UPDATE Promociones SET Nombre = @Nombre, Descripcion = @Descripcion, TipoPromocion = @TipoPromocion,
                                           TipoDescuento = @TipoDescuento, ValorDescuento = @ValorDescuento, PrecioCombo = @PrecioCombo,
                                           CodigoCupon = @CodigoCupon, MontoMinimoCompra = @MontoMinimoCompra, UsosMaximos = @UsosMaximos,
                                           CategoriaID = @CategoriaID, FechaInicio = @FechaInicio, FechaFin = @FechaFin, Activo = @Activo
                                           WHERE PromocionID = @PromocionID";

            public const string Delete = "DELETE FROM Promociones WHERE PromocionID = @PromocionID";

            public const string DeleteProducts = "DELETE FROM PromocionProductos WHERE PromocionID = @PromocionID";

            public const string InsertProduct = @"INSERT INTO PromocionProductos (PromocionID, ProductoID, Cantidad)
                                                  VALUES (@PromocionID, @ProductoID, @Cantidad)";
        }

        public async Task<List<Promocion>> GetActivePromotionsAsync()
        {
            List<Promocion> promociones = [.. await databaseRepository.QueryAsync<Promocion>(Queries.GetActive)];
            await LoadProductsAsync(promociones);
            return promociones;
        }

        public async Task<Promocion?> GetPromotionByIdAsync(int id)
        {
            var promocion = await databaseRepository.QueryFirstOrDefaultAsync<Promocion>(Queries.GetById, new { PromocionID = id });
            if (promocion != null)
            {
                await LoadProductsAsync([promocion]);
            }
            return promocion;
        }

        public async Task<Promocion?> GetPromotionByCouponAsync(string codigo)
        {
            return await databaseRepository.QueryFirstOrDefaultAsync<Promocion>(Queries.GetByCoupon, new { CodigoCupon = codigo });
        }

        public async Task<bool> CouponExistsAsync(string codigo, int? excluirId = null)
        {
            return await databaseRepository.ExecuteScalarAsync<int>(Queries.CouponExists, new { CodigoCupon = codigo, ExcluirID = excluirId }) > 0;
        }

        public async Task<int> InsertPromotionAsync(Promocion promocion)
        {
            // Transacción: si falla guardar los productos, tampoco se guarda la promoción
            using var transaction = await databaseRepository.BeginTransactionAsync();
            using var connection = transaction.Connection!;
            try
            {
                var newId = await databaseRepository.ExecuteScalarAsync<int>(Queries.Insert, GetParameters(promocion), transaction);
                await SaveProductsAsync(newId, promocion.Productos, transaction);
                transaction.Commit();
                return newId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdatePromotionAsync(Promocion promocion)
        {
            using var transaction = await databaseRepository.BeginTransactionAsync();
            using var connection = transaction.Connection!;
            try
            {
                var rowsAffected = await databaseRepository.ExecuteAsync(Queries.Update, GetParameters(promocion), transaction);
                if (rowsAffected == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                // Se reemplaza la lista de productos por la nueva
                await databaseRepository.ExecuteAsync(Queries.DeleteProducts, new { promocion.PromocionID }, transaction);
                await SaveProductsAsync(promocion.PromocionID, promocion.Productos, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            // ON DELETE CASCADE borra también sus filas en PromocionProductos
            return await databaseRepository.ExecuteAsync(Queries.Delete, new { PromocionID = id }) > 0;
        }

        // ---------- Métodos privados de apoyo ----------

        private static object GetParameters(Promocion promocion) => new
        {
            promocion.PromocionID,
            promocion.Nombre,
            promocion.Descripcion,
            promocion.TipoPromocion,
            promocion.TipoDescuento,
            promocion.ValorDescuento,
            promocion.PrecioCombo,
            promocion.CodigoCupon,
            promocion.MontoMinimoCompra,
            promocion.UsosMaximos,
            promocion.CategoriaID,
            promocion.FechaInicio,
            promocion.FechaFin,
            promocion.Activo
        };

        private async Task LoadProductsAsync(List<Promocion> promociones)
        {
            if (promociones.Count == 0) return;

            var ids = promociones.Select(p => p.PromocionID).ToList();
            var productos = await databaseRepository.QueryAsync<PromocionProducto>(Queries.GetProducts, new { Ids = ids });

            foreach (var promocion in promociones)
            {
                promocion.Productos = [.. productos.Where(pp => pp.PromocionID == promocion.PromocionID)];
            }
        }

        private async Task SaveProductsAsync(int promocionId, List<PromocionProducto> productos, IDbTransaction transaction)
        {
            if (productos.Count == 0) return;

            
            await databaseRepository.ExecuteAsync(
                Queries.InsertProduct,
                productos.Select(p => new { PromocionID = promocionId, p.ProductoID, p.Cantidad }),
                transaction);
        }
    }
}
