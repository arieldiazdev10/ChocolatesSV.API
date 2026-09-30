-- =========================================================
-- Base de datos: ChocolatesSV
-- Script completo de creación y datos de prueba
-- Compatible con el módulo de Checkout del Programador 3
-- =========================================================

IF DB_ID(N'ChocolatesSV') IS NOT NULL
BEGIN
    THROW 50000, 'La base de datos ChocolatesSV ya existe. Use una base de datos nueva para ejecutar este script.', 1;
END
GO

CREATE DATABASE ChocolatesSV;
GO

USE ChocolatesSV;
GO

-- =========================================================
-- TABLAS INDEPENDIENTES
-- =========================================================

CREATE TABLE Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) NOT NULL UNIQUE,
    ContrasenaHash NVARCHAR(255) NOT NULL,
    Rol NVARCHAR(50) NOT NULL CONSTRAINT DF_Usuarios_Rol DEFAULT 'Administrador',
    FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT SYSDATETIME(),
    Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT 1
);
GO

CREATE TABLE Categorias (
    CategoriaID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Categorias_Activo DEFAULT 1
);
GO

-- =========================================================
-- CATÁLOGO
-- =========================================================

CREATE TABLE Productos (
    ProductoID INT IDENTITY(1,1) PRIMARY KEY,
    CategoriaID INT NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    URLImagen NVARCHAR(255) NULL,
    Destacado BIT NOT NULL CONSTRAINT DF_Productos_Destacado DEFAULT 0,
    Existencias INT NOT NULL CONSTRAINT DF_Productos_Existencias DEFAULT 0,
    Activo BIT NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Productos_FechaCreacion DEFAULT SYSDATETIME(),
    UsuarioCreacionID INT NULL,
    UsuarioModificacionID INT NULL,

    CONSTRAINT CK_Productos_Precio CHECK (Precio >= 0),
    CONSTRAINT CK_Productos_Existencias CHECK (Existencias >= 0),
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaID)
        REFERENCES Categorias(CategoriaID),
    CONSTRAINT FK_Productos_UsuarioCreacion FOREIGN KEY (UsuarioCreacionID)
        REFERENCES Usuarios(UsuarioID),
    CONSTRAINT FK_Productos_UsuarioModificacion FOREIGN KEY (UsuarioModificacionID)
        REFERENCES Usuarios(UsuarioID)
);
GO

-- =========================================================
-- PROMOCIONES
-- =========================================================

CREATE TABLE Promociones (
    PromocionID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    TipoPromocion NVARCHAR(20) NOT NULL,
    TipoDescuento NVARCHAR(20) NOT NULL,
    ValorDescuento DECIMAL(10,2) NOT NULL,
    PrecioCombo DECIMAL(10,2) NULL,
    CodigoCupon NVARCHAR(30) NULL,
    MontoMinimoCompra DECIMAL(10,2) NULL,
    UsosMaximos INT NULL,
    UsosActuales INT NOT NULL CONSTRAINT DF_Promociones_UsosActuales DEFAULT 0,
    CategoriaID INT NULL,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NOT NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Promociones_Activo DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Promociones_FechaCreacion DEFAULT SYSDATETIME(),

    CONSTRAINT CK_Promociones_Tipo CHECK (TipoPromocion IN ('Temporada', 'Producto', 'Categoria', 'Combo', 'Cupon')),
    CONSTRAINT CK_Promociones_TipoDescuento CHECK (TipoDescuento IN ('Porcentaje', 'MontoFijo')),
    CONSTRAINT CK_Promociones_Valor CHECK (ValorDescuento >= 0),
    CONSTRAINT CK_Promociones_Fechas CHECK (FechaFin >= FechaInicio),
    CONSTRAINT CK_Promociones_Usos CHECK (UsosMaximos IS NULL OR UsosMaximos > 0),
    CONSTRAINT FK_Promociones_Categorias FOREIGN KEY (CategoriaID)
        REFERENCES Categorias(CategoriaID)
);
GO

CREATE UNIQUE INDEX UX_Promociones_CodigoCupon
    ON Promociones(CodigoCupon)
    WHERE CodigoCupon IS NOT NULL;
GO

CREATE TABLE PromocionProductos (
    PromocionID INT NOT NULL,
    ProductoID INT NOT NULL,
    Cantidad INT NOT NULL CONSTRAINT DF_PromocionProductos_Cantidad DEFAULT 1,

    CONSTRAINT PK_PromocionProductos PRIMARY KEY (PromocionID, ProductoID),
    CONSTRAINT CK_PromocionProductos_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT FK_PromProd_Promociones FOREIGN KEY (PromocionID)
        REFERENCES Promociones(PromocionID) ON DELETE CASCADE,
    CONSTRAINT FK_PromProd_Productos FOREIGN KEY (ProductoID)
        REFERENCES Productos(ProductoID)
);
GO

-- =========================================================
-- PEDIDOS Y CHECKOUT GUEST
-- =========================================================

CREATE TABLE Pedidos (
    PedidoID INT IDENTITY(1,1) PRIMARY KEY,
    CodigoOrden NVARCHAR(32) NOT NULL UNIQUE,
    NombreCliente NVARCHAR(150) NOT NULL,
    CorreoCliente NVARCHAR(150) NOT NULL,
    TelefonoCliente NVARCHAR(30) NULL,
    FechaEntrega DATE NOT NULL,
    Comentarios NVARCHAR(MAX) NULL,
    SubTotal DECIMAL(10,2) NOT NULL,
    DescuentoAplicado DECIMAL(10,2) NOT NULL CONSTRAINT DF_Pedidos_Descuento DEFAULT 0,
    Total DECIMAL(10,2) NOT NULL,
    EstadoPedido NVARCHAR(50) NOT NULL CONSTRAINT DF_Pedidos_Estado DEFAULT 'Pendiente',
    MetodoPago NVARCHAR(50) NOT NULL,
    ReferenciaPago NVARCHAR(100) NULL,
    FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Pedidos_FechaCreacion DEFAULT SYSDATETIME(),

    CONSTRAINT CK_Pedidos_SubTotal CHECK (SubTotal >= 0),
    CONSTRAINT CK_Pedidos_Descuento CHECK (DescuentoAplicado >= 0),
    CONSTRAINT CK_Pedidos_Total CHECK (Total >= 0),
    CONSTRAINT CK_Pedidos_Estado CHECK (EstadoPedido IN ('Pendiente', 'Confirmado', 'En Preparacion', 'Enviado', 'Entregado', 'Cancelado')),
    CONSTRAINT CK_Pedidos_MetodoPago CHECK (MetodoPago IN ('Simulado', 'ContraEntrega', 'TransferenciaBancaria'))
);
GO

CREATE TABLE PedidoDetalles (
    PedidoDetalleID INT IDENTITY(1,1) PRIMARY KEY,
    PedidoID INT NOT NULL,
    ProductoID INT NOT NULL,
    NombreProducto NVARCHAR(150) NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Cantidad INT NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,

    CONSTRAINT CK_PedidoDetalles_PrecioUnitario CHECK (PrecioUnitario >= 0),
    CONSTRAINT CK_PedidoDetalles_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_PedidoDetalles_Subtotal CHECK (Subtotal >= 0),
    CONSTRAINT FK_PedidoDetalles_Pedidos FOREIGN KEY (PedidoID)
        REFERENCES Pedidos(PedidoID) ON DELETE CASCADE,
    CONSTRAINT FK_PedidoDetalles_Productos FOREIGN KEY (ProductoID)
        REFERENCES Productos(ProductoID)
);
GO

CREATE INDEX IX_PedidoDetalles_PedidoID
    ON PedidoDetalles(PedidoID);
GO

-- =========================================================
-- DATOS DE PRUEBA
-- =========================================================

INSERT INTO Usuarios (Nombre, Apellido, Correo, ContrasenaHash, Rol)
VALUES ('Admin', 'Sistema', 'admin@chocolatessv.com', 'hash_seguro_aqui_123', 'Administrador');
GO

INSERT INTO Categorias (Nombre, Descripcion)
VALUES
    ('Trufas', 'Deliciosas trufas rellenas artesanales'),
    ('Tabletas', 'Tabletas de chocolate oscuro, de leche y blanco'),
    ('Combos Especiales', 'Cajas de regalo y arreglos');
GO

INSERT INTO Productos
    (CategoriaID, Nombre, Descripcion, Precio, URLImagen, Destacado, Existencias, Activo, UsuarioCreacionID)
VALUES
    (1, 'Trufas de Maracuyá', 'Caja de 6 trufas rellenas de ganache de maracuyá.', 5.50, '/images/trufa_maracuya.jpg', 1, 50, 1, 1),
    (2, 'Tableta 70% Cacao', 'Tableta artesanal de 100g con 70% cacao puro.', 3.75, '/images/tableta_70.jpg', 1, 100, 1, 1),
    (3, 'Caja San Valentín', 'Surtido de 12 bombones en caja en forma de corazón.', 12.00, '/images/caja_sanvalentin.jpg', 0, 20, 1, 1);
GO

INSERT INTO Promociones
    (Nombre, Descripcion, TipoPromocion, TipoDescuento, ValorDescuento, CodigoCupon,
     MontoMinimoCompra, UsosMaximos, CategoriaID, FechaInicio, FechaFin, Activo)
VALUES
    ('Cupón Bienvenida', 'Descuento del 10% en tu primera compra', 'Cupon', 'Porcentaje', 10.00,
     'DULCE10', 5.00, 100, NULL, DATEFROMPARTS(YEAR(GETDATE()), 1, 1), DATEFROMPARTS(YEAR(GETDATE()), 12, 31), 1),
    ('Descuento de Temporada', 'Descuento de $2 en compras de temporada', 'Temporada', 'MontoFijo', 2.00,
     NULL, NULL, NULL, NULL, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1),
    ('Descuento Tabletas', 'Descuento del 5% en tabletas', 'Categoria', 'Porcentaje', 5.00,
     NULL, NULL, NULL, 2, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1);
GO

INSERT INTO PromocionProductos (PromocionID, ProductoID, Cantidad)
VALUES
    (3, 2, 1);
GO

INSERT INTO Pedidos
    (CodigoOrden, NombreCliente, CorreoCliente, TelefonoCliente, FechaEntrega,
     Comentarios, SubTotal, DescuentoAplicado, Total, EstadoPedido, MetodoPago, ReferenciaPago)
VALUES
    ('ORD-DEMO-00000000000000000001', 'Juan Pérez', 'juan.perez@email.com', '7777-8888',
     DATEADD(DAY, 2, CAST(GETDATE() AS DATE)), 'Pedido de prueba', 17.50, 0.00, 17.50,
     'Confirmado', 'Simulado', 'SIM-987654321');
GO

INSERT INTO PedidoDetalles
    (PedidoID, ProductoID, NombreProducto, PrecioUnitario, Cantidad, Subtotal)
VALUES
    (1, 3, 'Caja San Valentín', 12.00, 1, 12.00),
    (1, 1, 'Trufas de Maracuyá', 5.50, 1, 5.50);
GO