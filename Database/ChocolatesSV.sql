-- =========================================================
-- Base de Datos: Chocolates SV
-- =========================================================

CREATE DATABASE ChocolatesSV;
GO

USE ChocolatesSV;
GO

-- =========================================================
-- TABLAS INDEPENDIENTES (Sin llaves foráneas)
-- =========================================================

-- Tabla de Usuarios (EXCLUSIVA PARA ADMINISTRADORES)
CREATE TABLE Usuarios (
    UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) UNIQUE NOT NULL,
    ContrasenaHash NVARCHAR(255) NOT NULL, 
    Rol NVARCHAR(50) DEFAULT 'Administrador',   
    FechaCreacion DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);

-- Tabla de Categorías de Productos
CREATE TABLE Categorias (
    CategoriaID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Activo BIT DEFAULT 1
);

-- =========================================================
-- TABLAS DEPENDIENTES (Con llaves foráneas)
-- =========================================================

-- Tabla de Promociones y Ofertas
CREATE TABLE Promociones (
    PromocionID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    TipoPromocion NVARCHAR(50) NOT NULL CHECK (TipoPromocion IN ('Temporada', 'DescuentoCategoria', 'Combo', 'Cupon')),
    TipoDescuento NVARCHAR(50) NOT NULL CHECK (TipoDescuento IN ('Porcentaje', 'MontoFijo')),
    ValorDescuento DECIMAL(10,2) NOT NULL,
    CodigoCupon NVARCHAR(50) UNIQUE, 
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    
    -- Campos de Auditoría
    UsuarioCreacionID INT,
    UsuarioModificacionID INT,
    FOREIGN KEY (UsuarioCreacionID) REFERENCES Usuarios(UsuarioID),
    FOREIGN KEY (UsuarioModificacionID) REFERENCES Usuarios(UsuarioID)
);

-- Tabla de Productos (Catálogo)
CREATE TABLE Productos (
    ProductoID INT IDENTITY(1,1) PRIMARY KEY,
    CategoriaID INT NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    URLImagen NVARCHAR(255),
    Destacado BIT DEFAULT 0, 
    Existencias INT DEFAULT 0,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    
    -- Campos de Auditoría
    UsuarioCreacionID INT,
    UsuarioModificacionID INT,
    FOREIGN KEY (CategoriaID) REFERENCES Categorias(CategoriaID),
    FOREIGN KEY (UsuarioCreacionID) REFERENCES Usuarios(UsuarioID),
    FOREIGN KEY (UsuarioModificacionID) REFERENCES Usuarios(UsuarioID)
);

-- Tabla para aplicar descuentos específicos a productos
CREATE TABLE PromocionesProductos (
    PromocionProductoID INT IDENTITY(1,1) PRIMARY KEY,
    ProductoID INT NOT NULL,
    PromocionID INT NOT NULL,
    FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID),
    FOREIGN KEY (PromocionID) REFERENCES Promociones(PromocionID)
);

-- Tabla de Pedidos (Checkout Guest)
CREATE TABLE Pedidos (
    PedidoID INT IDENTITY(1,1) PRIMARY KEY,
    -- Datos directos del cliente invitado
    NombreCliente NVARCHAR(150) NOT NULL,
    CorreoCliente NVARCHAR(150) NOT NULL,
    TelefonoCliente NVARCHAR(20), 
    
    -- Datos de la entrega
    FechaEntrega DATE NOT NULL,
    Comentarios NVARCHAR(MAX),
    
    -- Cálculos financieros
    SubTotal DECIMAL(10,2) NOT NULL,
    DescuentoAplicado DECIMAL(10,2) DEFAULT 0.00,
    Total DECIMAL(10,2) NOT NULL,
    
    -- Estado y Pagos
    EstadoPedido NVARCHAR(50) DEFAULT 'Confirmado' CHECK (EstadoPedido IN ('Pendiente', 'Confirmado', 'En Preparacion', 'Enviado', 'Entregado', 'Cancelado')),
    MetodoPago NVARCHAR(50) NOT NULL CHECK (MetodoPago IN ('Simulado', 'ContraEntrega', 'TransferenciaBancaria')),
    ReferenciaPago NVARCHAR(100), 
    
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Tabla de Detalles del Pedido
CREATE TABLE DetallesPedido (
    DetallePedidoID INT IDENTITY(1,1) PRIMARY KEY,
    PedidoID INT NOT NULL,
    ProductoID INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(10,2) NOT NULL, -- Precio congelado al momento de la compra
    SubTotal DECIMAL(10,2) NOT NULL,       -- Cantidad * PrecioUnitario
    FOREIGN KEY (PedidoID) REFERENCES Pedidos(PedidoID),
    FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID)
);

-- =========================================================
-- DATOS DE PRUEBA INICIALES
-- =========================================================

-- 1. Insertar un Administrador
INSERT INTO Usuarios (Nombre, Apellido, Correo, ContrasenaHash, Rol)
VALUES ('Admin', 'Sistema', 'admin@chocolatessv.com', 'hash_seguro_aqui_123', 'Administrador');

-- 2. Insertar Categorías
INSERT INTO Categorias (Nombre, Descripcion)
VALUES 
('Trufas', 'Deliciosas trufas rellenas artesanales'),
('Tabletas', 'Tabletas de chocolate oscuro, de leche y blanco'),
('Combos Especiales', 'Cajas de regalo y arreglos');

-- 3. Insertar Productos (Registrando que el Usuario 1 los creó)
INSERT INTO Productos (CategoriaID, Nombre, Descripcion, Precio, URLImagen, Destacado, Existencias, UsuarioCreacionID)
VALUES 
(1, 'Trufas de Maracuyá', 'Caja de 6 trufas rellenas de ganache de maracuyá.', 5.50, '/images/trufa_maracuya.jpg', 1, 50, 1),
(2, 'Tableta 70% Cacao', 'Tableta artesanal de 100g con 70% cacao puro.', 3.75, '/images/tableta_70.jpg', 1, 100, 1),
(3, 'Caja San Valentín', 'Surtido de 12 bombones en caja en forma de corazón.', 12.00, '/images/caja_sanvalentin.jpg', 0, 20, 1);

-- 4. Insertar Promociones (Registrando que el Usuario 1 las creó)
INSERT INTO Promociones (Nombre, Descripcion, TipoPromocion, TipoDescuento, ValorDescuento, CodigoCupon, FechaInicio, FechaFin, UsuarioCreacionID)
VALUES 
('Cupón Bienvenida', 'Descuento del 10% en tu primera compra', 'Cupon', 'Porcentaje', 10.00, 'CHOCO10', GETDATE(), DATEADD(year, 1, GETDATE()), 1),
('Descuento de Temporada', 'Descuento de $2 en todas las cajas de regalo', 'Temporada', 'MontoFijo', 2.00, NULL, GETDATE(), DATEADD(month, 1, GETDATE()), 1);

-- 5. Insertar un Pedido de Prueba
INSERT INTO Pedidos (NombreCliente, CorreoCliente, TelefonoCliente, FechaEntrega, SubTotal, DescuentoAplicado, Total, EstadoPedido, MetodoPago, ReferenciaPago)
VALUES ('Juan Pérez', 'juan.perez@email.com', '7777-8888', DATEADD(day, 2, GETDATE()), 17.50, 0.00, 17.50, 'Confirmado', 'Simulado', 'SIM-987654321');

-- 6. Insertar el Detalle del Pedido
INSERT INTO DetallesPedido (PedidoID, ProductoID, Cantidad, PrecioUnitario, SubTotal)
VALUES 
(1, 3, 1, 12.00, 12.00), -- Caja San Valentín
(1, 1, 1, 5.50, 5.50);   -- Trufas de Maracuyá
GO