using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.DAL
{
    public class ProductoRepository(IDatabaseRepository databaseRepository) : IProductoRepository
    {
        private static class Queries
        {
            public const string GetAllActive = "SELECT * FROM Productos WHERE Activo = 1";
            public const string GetFeatured = "SELECT * FROM Productos WHERE Activo = 1 AND Destacado = 1";
            public const string GetById = "SELECT * FROM Productos WHERE ProductoID = @ProductoID";
            public const string GetByIds = "SELECT * FROM Productos WHERE ProductoID IN @Ids AND Activo = 1";
            public const string DecreaseStock = @"UPDATE Productos
                                                 SET Existencias = Existencias - @Cantidad
                                                 WHERE ProductoID = @ProductoID
                                                   AND Activo = 1
                                                   AND Existencias >= @Cantidad";

            public const string Insert = "INSERT INTO Productos (CategoriaID, Nombre, Descripcion, Precio, URLImagen, Destacado, Existencias, Activo, UsuarioCreacionID) VALUES (@CategoriaID, @Nombre, @Descripcion, @Precio, @URLImagen, @Destacado, @Existencias, 1, @UsuarioCreacionID);SELECT CAST(SCOPE_IDENTITY() as int);";
            public const string Update = "UPDATE Productos SET CategoriaID = @CategoriaID, Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, URLImagen = @URLImagen, Destacado = @Destacado, Existencias = @Existencias, UsuarioModificacionID = @UsuarioModificacionID WHERE ProductoID = @ProductoID AND Activo = 1";
            public const string SoftDelete = "UPDATE Productos SET Activo = 0 WHERE ProductoID = @ProductoID AND Activo = 1";
        }

        public async Task<List<Producto>> GetAllActiveProductsAsync()
        {
            return [.. (await databaseRepository.QueryAsync<Producto>(Queries.GetAllActive))];
        }

        public async Task<List<Producto>> GetFeaturedProductsAsync()
        {
            return [.. (await databaseRepository.QueryAsync<Producto>(Queries.GetFeatured))];
        }

        public async Task<Producto?> GetProductByIdAsync(int id)
        {
            return await databaseRepository.QueryFirstOrDefaultAsync<Producto>(Queries.GetById, new { ProductoID = id });
        }

        public async Task<List<Producto>> GetProductsByIdsAsync(IEnumerable<int> ids, IDbTransaction? transaction = null)
        {
            return [.. await databaseRepository.QueryAsync<Producto>(Queries.GetByIds, new { Ids = ids }, transaction)];
        }

        public async Task<bool> DecreaseStockAsync(int id, int quantity, IDbTransaction transaction)
        {
            return await databaseRepository.ExecuteAsync(
                Queries.DecreaseStock,
                new { ProductoID = id, Cantidad = quantity },
                transaction) > 0;
        }

        public async Task<int> InsertProductAsync(Producto producto)
        {
            return await databaseRepository.ExecuteScalarAsync<int>(Queries.Insert, new { producto.CategoriaID, producto.Nombre, producto.Descripcion, producto.Precio, producto.URLImagen, producto.Destacado, producto.Existencias, producto.UsuarioCreacionID });
        }

        public async Task<bool> UpdateProductAsync(Producto producto)
        {
            var rowsAffected = await databaseRepository.ExecuteAsync(
                Queries.Update,
                new {
                    producto.ProductoID,
                    producto.CategoriaID,
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Precio,
                    producto.URLImagen,
                    producto.Destacado,
                    producto.Existencias,
                    producto.UsuarioModificacionID
                }
            );
            return rowsAffected > 0;
        }

        public async Task<bool> SoftDeleteProductAsync(int id)
        {
            return await databaseRepository.ExecuteAsync(Queries.SoftDelete, new { ProductoID = id }) > 0;
        }
    }
}


