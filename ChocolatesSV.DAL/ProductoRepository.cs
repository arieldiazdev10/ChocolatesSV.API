using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChocolatesSV.DAL
{
    public class ProductoRepository(IDatabaseRepository databaseRepository) : IProductoRepository
    {
        private static class Queries
        {
            public const string GetAllActive = "SELECT * FROM Productos WHERE Activo = 1";
            public const string GetFeatured = "SELECT * FROM Productos WHERE Activo = 1 AND Destacado = 1";
            public const string GetById = "SELECT * FROM Productos WHERE ProductoID = @ProductoID AND Activo = 1";
            public const string Insert = "INSERT INTO Productos (CategoriaID, Nombre, Descripcion, Precio, URLImagen, Destacado, Existencias, UsuarioCreacionID) VALUES (@CategoriaID, @Nombre, @Descripcion, @Precio, @URLImagen, @Destacado, @Existencias, @UsuarioCreacionID);SELECT CAST(SCOPE_IDENTITY() as int);";
            public const string Update = "UPDATE Productos SET CategoriaID = @CategoriaID, Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, URLImagen = @URLImagen, Destacado = @Destacado,Existencias = @Existencias, UsuarioModificacionID = @UsuarioModificacionID WHERE ProductoID = @ProductoID";
            public const string SoftDelete = "UPDATE Productos SET Activo = 0 WHERE ProductoID = @ProductoID";
        }

        public async Task<List<Producto>> GetAllActiveProductsAsync()
        {
            return [.. (await databaseRepository.QueryAsync<Producto>(Queries.GetAllActive))];
        }

        public async Task<List<Producto>> GetFeaturedProductsAsync()
        {
            return [.. (await databaseRepository.QueryAsync<Producto>(Queries.GetAllActive))];
        }

        public async Task<Producto?> GetProductByIdAsync(int id)
        {
            return await databaseRepository.QueryFirstOrDefaultAsync<Producto>(Queries.GetById, new { ProductoID = id });
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


