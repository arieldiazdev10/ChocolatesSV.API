IF DB_ID(N'ChocolatesSV') IS NOT NULL
BEGIN
    THROW 50000, 'La base de datos ChocolatesSV ya existe. Use una base de datos nueva para ejecutar este script.', 1;
END
GO

CREATE DATABASE ChocolatesSV;
GO

USE ChocolatesSV;
GO


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


CREATE TABLE InformacionEmpresa (
    InformacionEmpresaID INT IDENTITY(1,1) PRIMARY KEY,
    NombreEmpresa NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    Mision NVARCHAR(500) NULL,
    Vision NVARCHAR(500) NULL,
    Direccion NVARCHAR(255) NULL,
    Telefono NVARCHAR(30) NULL,
    Correo NVARCHAR(150) NULL,
    HorarioAtencion NVARCHAR(200) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_InformacionEmpresa_Activo DEFAULT 1,
    FechaActualizacion DATETIME2 NOT NULL CONSTRAINT DF_InformacionEmpresa_Fecha DEFAULT SYSDATETIME()
);
GO

CREATE TABLE PreguntasFrecuentes (
    PreguntaID INT IDENTITY(1,1) PRIMARY KEY,
    Pregunta NVARCHAR(255) NOT NULL,
    Respuesta NVARCHAR(MAX) NOT NULL,
    Orden INT NOT NULL CONSTRAINT DF_PreguntasFrecuentes_Orden DEFAULT 0,
    Activo BIT NOT NULL CONSTRAINT DF_PreguntasFrecuentes_Activo DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_PreguntasFrecuentes_Fecha DEFAULT SYSDATETIME()
);
GO

INSERT INTO Usuarios (Nombre, Apellido, Correo, ContrasenaHash, Rol, Activo)
VALUES
    (N'Admin', N'Sistema', N'admin@chocolatessv.com', N'hash_seguro_aqui_123', N'Administrador', 1),
    (N'María', N'Hernández', N'maria.hernandez@chocolatessv.com', N'hash_seguro_aqui_123', N'Administrador', 1),
    (N'Carlos', N'Martínez', N'carlos.martinez@chocolatessv.com', N'hash_seguro_aqui_123', N'Editor', 1),
    (N'Ana', N'Rivas', N'ana.rivas@chocolatessv.com', N'hash_seguro_aqui_123', N'Editor', 1),
    (N'Luis', N'Ramírez', N'luis.ramirez@chocolatessv.com', N'hash_seguro_aqui_123', N'Editor', 0);
GO

INSERT INTO Categorias (Nombre, Descripcion, Activo)
VALUES
    (N'Trufas', N'Deliciosas trufas rellenas artesanales', 1),
    (N'Tabletas', N'Tabletas de chocolate oscuro, de leche y blanco', 1),
    (N'Combos Especiales', N'Cajas de regalo y arreglos', 1),
    (N'Bebidas de Cacao', N'Chocolate caliente y cacao en polvo', 1),
    (N'Bombones', N'Bombones rellenos de licor, frutas y cremas', 1),
    (N'Regalos Corporativos', N'Cajas y presentaciones para empresas', 1),
    (N'Temporada Anterior', N'Línea descontinuada', 0);
GO

INSERT INTO Productos
    (CategoriaID, Nombre, Descripcion, Precio, URLImagen, Destacado, Existencias, Activo, UsuarioCreacionID)
VALUES
    (1, N'Trufas de Maracuyá', N'Caja de 6 trufas rellenas de ganache de maracuyá.', 5.50, '/images/trufa_maracuya.jpg', 1, 50, 1, 1),
    (2, N'Tableta 70% Cacao', N'Tableta artesanal de 100g con 70% cacao puro.', 3.75, '/images/tableta_70.jpg', 1, 100, 1, 2),
    (3, N'Caja San Valentín', N'Surtido de 12 bombones en caja en forma de corazón.', 12.00, '/images/caja_sanvalentin.jpg', 0, 20, 1, 3),
    (1, N'Trufas de Café', N'Caja de 6 trufas de ganache de café salvadoreño.', 5.75, '/images/trufa_cafe.jpg', 1, 45, 1, 1),
    (1, N'Trufas de Coco', N'Caja de 6 trufas cubiertas de coco rallado.', 5.25, '/images/trufa_coco.jpg', 0, 60, 1, 2),
    (1, N'Trufas de Avellana', N'Caja de 6 trufas rellenas de crema de avellana.', 6.00, '/images/trufa_avellana.jpg', 1, 35, 1, 3),
    (2, N'Tableta Chocolate con Leche', N'Tableta de 100g de chocolate con leche cremoso.', 3.25, '/images/tableta_leche.jpg', 0, 120, 1, 1),
    (2, N'Tableta Chocolate Blanco', N'Tableta de 100g de chocolate blanco con vainilla.', 3.50, '/images/tableta_blanco.jpg', 0, 80, 1, 2),
    (2, N'Tableta 85% Cacao', N'Tableta intensa de 100g con 85% cacao.', 4.25, '/images/tableta_85.jpg', 0, 70, 1, 3),
    (2, N'Tableta con Almendras', N'Tableta de 100g de chocolate oscuro con almendras tostadas.', 4.50, '/images/tableta_almendras.jpg', 1, 90, 1, 1),
    (3, N'Caja Sorpresa Mamá', N'Caja de regalo con trufas, bombones y tableta.', 15.00, '/images/caja_mama.jpg', 1, 25, 1, 2),
    (3, N'Caja Degustación', N'Selección de 4 tabletas de origen en presentación de regalo.', 18.50, '/images/caja_degustacion.jpg', 0, 15, 1, 3),
    (3, N'Bombones Surtidos x24', N'Caja de 24 bombones surtidos.', 22.00, '/images/bombones_24.jpg', 1, 12, 1, 1),
    (4, N'Chocolate Caliente en Polvo', N'Bolsa de 300g de chocolate caliente artesanal.', 6.50, '/images/chocolate_caliente.jpg', 0, 55, 1, 2),
    (4, N'Cacao para Hornear', N'Bolsa de 250g de cacao puro sin azúcar.', 4.95, '/images/cacao_hornear.jpg', 0, 40, 1, 3),
    (5, N'Bombones de Licor', N'Caja de 8 bombones rellenos de licor de ron.', 9.00, '/images/bombones_licor.jpg', 0, 30, 1, 1),
    (5, N'Bombones de Fresa', N'Caja de 8 bombones rellenos de crema de fresa.', 8.00, '/images/bombones_fresa.jpg', 0, 0, 1, 2),
    (6, N'Caja Corporativa 50 piezas', N'Caja de 50 bombones surtidos para obsequios corporativos.', 45.00, '/images/caja_corporativa.jpg', 0, 10, 1, 3),
    (2, N'Tableta Edición Navidad', N'Tableta de edición limitada con especias navideñas.', 4.00, '/images/tableta_navidad.jpg', 0, 0, 0, 1);
GO

INSERT INTO Promociones
    (Nombre, Descripcion, TipoPromocion, TipoDescuento, ValorDescuento, PrecioCombo, CodigoCupon,
     MontoMinimoCompra, UsosMaximos, UsosActuales, CategoriaID, FechaInicio, FechaFin, Activo)
VALUES
    (N'Cupón Bienvenida', N'Descuento del 10% en tu primera compra', 'Cupon', 'Porcentaje', 10.00, NULL, N'DULCE10', 5.00, 100, 0, NULL, DATEFROMPARTS(YEAR(GETDATE()), 1, 1), DATEFROMPARTS(YEAR(GETDATE()), 12, 31), 1),
    (N'Descuento de Temporada', N'Descuento de $2 en compras de temporada', 'Temporada', 'MontoFijo', 2.00, NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1),
    (N'Descuento Tabletas', N'Descuento del 5% en tabletas', 'Categoria', 'Porcentaje', 5.00, NULL, NULL, NULL, NULL, 0, 2, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1),
    (N'Combo Dúo de Trufas', N'Trufas de Maracuyá y Trufas de Café a precio especial', 'Combo', 'MontoFijo', 0.75, 10.50, NULL, NULL, NULL, 0, NULL, GETDATE(), DATEADD(MONTH, 2, GETDATE()), 1),
    (N'Semana de las Trufas', N'Descuento del 10% en todas las trufas', 'Categoria', 'Porcentaje', 10.00, NULL, NULL, NULL, NULL, 0, 1, DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, 10, GETDATE()), 1),
    (N'Oferta Caja Sorpresa Mamá', N'Descuento del 15% en la Caja Sorpresa Mamá', 'Producto', 'Porcentaje', 15.00, NULL, NULL, NULL, NULL, 0, NULL, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1),
    (N'Cupón Compra Grande', N'Descuento de $5 en compras mayores a $25', 'Cupon', 'MontoFijo', 5.00, NULL, N'CHOCO5', 25.00, 50, 3, NULL, DATEADD(DAY, -10, GETDATE()), DATEADD(MONTH, 3, GETDATE()), 1),
    (N'Promoción San Valentín', N'Descuento del 20% en cajas de regalo', 'Categoria', 'Porcentaje', 20.00, NULL, NULL, NULL, NULL, 0, 3, DATEFROMPARTS(YEAR(GETDATE())-1, 2, 1), DATEFROMPARTS(YEAR(GETDATE())-1, 2, 14), 1),
    (N'Cupón Verano', N'Descuento del 20% pausado temporalmente', 'Cupon', 'Porcentaje', 20.00, NULL, N'VERANO20', 10.00, 30, 0, NULL, GETDATE(), DATEADD(MONTH, 2, GETDATE()), 0);
GO

INSERT INTO PromocionProductos (PromocionID, ProductoID, Cantidad)
VALUES
    (3, 2, 1),
    (4, 1, 1),
    (4, 4, 1),
    (6, 11, 1),
    (5, 1, 1),
    (5, 4, 1),
    (5, 5, 1),
    (5, 6, 1);
GO

INSERT INTO Pedidos
    (CodigoOrden, NombreCliente, CorreoCliente, TelefonoCliente, FechaEntrega,
     Comentarios, SubTotal, DescuentoAplicado, Total, EstadoPedido, MetodoPago, ReferenciaPago, FechaCreacion)
VALUES
    ('ORD-DEMO-00000000000000000001', N'Juan Pérez', N'juan.perez@email.com', N'7777-8888', DATEADD(DAY, 2, CAST(GETDATE() AS DATE)), N'Pedido de prueba', 17.50, 0.00, 17.50, 'Confirmado', 'Simulado', N'SIM-987654321', DATEADD(DAY, -1, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000002', N'Sofía Menjívar', N'sofia.menjivar@email.com', N'7123-4567', DATEADD(DAY, 3, CAST(GETDATE() AS DATE)), N'Entregar por la tarde', 10.75, 0.00, 10.75, 'Pendiente', 'ContraEntrega', NULL, DATEADD(DAY, 0, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000003', N'Roberto Alfaro', N'roberto.alfaro@email.com', N'7234-5678', DATEADD(DAY, 1, CAST(GETDATE() AS DATE)), NULL, 26.50, 2.25, 24.25, 'Confirmado', 'TransferenciaBancaria', N'TRF-20260001', DATEADD(DAY, -1, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000004', N'Daniela Guzmán', N'daniela.guzman@email.com', N'7345-6789', DATEADD(DAY, 4, CAST(GETDATE() AS DATE)), N'Es un regalo, sin factura visible', 22.00, 0.00, 22.00, 'En Preparacion', 'Simulado', N'SIM-100200300', DATEADD(DAY, -2, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000005', N'Miguel Ángel Cruz', N'miguel.cruz@email.com', N'7456-7890', DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), NULL, 27.50, 2.75, 24.75, 'Enviado', 'Simulado', N'SIM-100200301', DATEADD(DAY, -3, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000006', N'Karla Portillo', N'karla.portillo@email.com', N'7567-8901', DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), N'Tocar el timbre', 13.50, 1.35, 12.15, 'Entregado', 'ContraEntrega', NULL, DATEADD(DAY, -6, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000007', N'Fernando López', N'fernando.lopez@email.com', N'7678-9012', DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), NULL, 45.00, 0.00, 45.00, 'Entregado', 'TransferenciaBancaria', N'TRF-20260002', DATEADD(DAY, -9, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000008', N'Gabriela Ortiz', N'gabriela.ortiz@email.com', N'7789-0123', DATEADD(DAY, 5, CAST(GETDATE() AS DATE)), N'Sin nueces por alergia', 31.50, 0.00, 31.50, 'Pendiente', 'Simulado', N'SIM-100200302', DATEADD(DAY, 0, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000009', N'Ricardo Segovia', N'ricardo.segovia@email.com', NULL, DATEADD(DAY, 2, CAST(GETDATE() AS DATE)), NULL, 17.00, 1.70, 15.30, 'Confirmado', 'Simulado', N'SIM-100200303', DATEADD(DAY, -1, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000010', N'Lucía Barrera', N'lucia.barrera@email.com', N'7890-1234', DATEADD(DAY, -7, CAST(GETDATE() AS DATE)), N'Cancelado por el cliente', 17.00, 0.00, 17.00, 'Cancelado', 'Simulado', N'SIM-100200304', DATEADD(DAY, -12, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000011', N'Andrés Castillo', N'andres.castillo@email.com', N'7901-2345', DATEADD(DAY, -5, CAST(GETDATE() AS DATE)), NULL, 12.20, 0.00, 12.20, 'Entregado', 'ContraEntrega', NULL, DATEADD(DAY, -10, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000012', N'Paola Núñez', N'paola.nunez@email.com', N'7012-3456', DATEADD(DAY, 6, CAST(GETDATE() AS DATE)), N'Dedicatoria: Feliz cumpleaños Mamá', 27.00, 4.05, 22.95, 'Pendiente', 'Simulado', N'SIM-100200305', DATEADD(DAY, 0, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000013', N'Héctor Villalta', N'hector.villalta@email.com', N'7111-2222', DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), NULL, 17.00, 0.00, 17.00, 'Enviado', 'TransferenciaBancaria', N'TRF-20260003', DATEADD(DAY, -4, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000014', N'Mariela Cabrera', N'mariela.cabrera@email.com', N'7222-3333', DATEADD(DAY, -8, CAST(GETDATE() AS DATE)), NULL, 44.00, 4.40, 39.60, 'Entregado', 'Simulado', N'SIM-100200306', DATEADD(DAY, -14, SYSDATETIME())),
    ('ORD-DEMO-00000000000000000015', N'Oscar Trejo', N'oscar.trejo@email.com', N'7333-4444', DATEADD(DAY, 3, CAST(GETDATE() AS DATE)), N'Llamar antes de llegar', 29.40, 0.00, 29.40, 'Confirmado', 'ContraEntrega', NULL, DATEADD(DAY, -1, SYSDATETIME()));
GO

INSERT INTO PedidoDetalles
    (PedidoID, ProductoID, NombreProducto, PrecioUnitario, Cantidad, Subtotal)
VALUES
    (1, 3, N'Caja San Valentín', 12.00, 1, 12.00),
    (1, 1, N'Trufas de Maracuyá', 5.50, 1, 5.50),
    (2, 2, N'Tableta 70% Cacao', 3.75, 2, 7.50),
    (2, 7, N'Tableta Chocolate con Leche', 3.25, 1, 3.25),
    (3, 11, N'Caja Sorpresa Mamá', 15.00, 1, 15.00),
    (3, 4, N'Trufas de Café', 5.75, 2, 11.50),
    (4, 13, N'Bombones Surtidos x24', 22.00, 1, 22.00),
    (5, 1, N'Trufas de Maracuyá', 5.50, 2, 11.00),
    (5, 5, N'Trufas de Coco', 5.25, 2, 10.50),
    (5, 6, N'Trufas de Avellana', 6.00, 1, 6.00),
    (6, 10, N'Tableta con Almendras', 4.50, 3, 13.50),
    (7, 18, N'Caja Corporativa 50 piezas', 45.00, 1, 45.00),
    (8, 12, N'Caja Degustación', 18.50, 1, 18.50),
    (8, 14, N'Chocolate Caliente en Polvo', 6.50, 2, 13.00),
    (9, 9, N'Tableta 85% Cacao', 4.25, 4, 17.00),
    (10, 16, N'Bombones de Licor', 9.00, 1, 9.00),
    (10, 17, N'Bombones de Fresa', 8.00, 1, 8.00),
    (11, 2, N'Tableta 70% Cacao', 3.75, 1, 3.75),
    (11, 8, N'Tableta Chocolate Blanco', 3.50, 1, 3.50),
    (11, 15, N'Cacao para Hornear', 4.95, 1, 4.95),
    (12, 11, N'Caja Sorpresa Mamá', 15.00, 1, 15.00),
    (12, 3, N'Caja San Valentín', 12.00, 1, 12.00),
    (13, 4, N'Trufas de Café', 5.75, 1, 5.75),
    (13, 5, N'Trufas de Coco', 5.25, 1, 5.25),
    (13, 6, N'Trufas de Avellana', 6.00, 1, 6.00),
    (14, 13, N'Bombones Surtidos x24', 22.00, 2, 44.00),
    (15, 14, N'Chocolate Caliente en Polvo', 6.50, 3, 19.50),
    (15, 15, N'Cacao para Hornear', 4.95, 2, 9.90);
GO

INSERT INTO InformacionEmpresa
    (NombreEmpresa, Descripcion, Mision, Vision, Direccion, Telefono, Correo, HorarioAtencion)
VALUES
    ('Chocolates SV',
     'Somos una chocolatería artesanal salvadoreña que elabora trufas, tabletas y cajas de regalo con cacao de calidad, hechas a mano y con ingredientes seleccionados.',
     'Elaborar chocolates artesanales de excelente calidad que endulcen los momentos especiales de nuestros clientes.',
     'Ser la chocolatería artesanal de referencia en El Salvador, reconocida por su sabor, calidad y servicio.',
     'San Salvador, El Salvador',
     '7777-8888',
     'contacto@chocolatessv.com',
     'Lunes a sábado de 8:00 a.m. a 6:00 p.m.');
GO

INSERT INTO PreguntasFrecuentes (Pregunta, Respuesta, Orden)
VALUES
    (N'¿Cómo puedo rastrear mi pedido?', N'Ingresa tu número de orden y el correo electrónico que usaste al comprar en la sección de rastreo para consultar el estado actual de tu pedido.', 1),
    (N'¿Dónde encuentro mi número de orden?', N'El número de orden se muestra al finalizar tu compra y se envía al correo electrónico que registraste.', 2),
    (N'¿Qué significa cada estado del pedido?', N'Pendiente: recibimos tu pedido. Confirmado: fue validado. En Preparación: lo estamos elaborando. Enviado: va en camino. Entregado: ya lo recibiste. Cancelado: el pedido fue anulado.', 3),
    (N'¿Cómo uso un cupón de descuento?', N'Ingresa el código del cupón durante el proceso de compra. El descuento se aplica al total si tu pedido cumple con el monto mínimo de compra y el cupón está vigente.', 4),
    (N'¿Cuándo recibiré mi pedido?', N'Recibirás tu pedido en la fecha de entrega que seleccionaste al realizar la compra.', 5),
    (N'¿Puedo cancelar mi pedido?', N'Puedes solicitar la cancelación mientras tu pedido esté en estado Pendiente o Confirmado. Contáctanos por teléfono o correo lo antes posible.', 6),
    (N'¿Hacen envíos a todo el país?', N'Realizamos entregas en San Salvador y sus alrededores. Para otras zonas, contáctanos para confirmar cobertura y costo.', 7),
    (N'¿Qué métodos de pago aceptan?', N'Aceptamos pago simulado en línea, contra entrega y transferencia bancaria.', 8),
    (N'¿Los chocolates contienen alérgenos?', N'Algunos productos contienen leche, frutos secos o gluten. Puedes indicarlo en los comentarios de tu pedido y te confirmaremos los ingredientes.', 9),
    (N'¿Hacen pedidos personalizados o corporativos?', N'Sí, elaboramos cajas personalizadas y pedidos corporativos. Escríbenos con al menos una semana de anticipación.', 10),
    (N'¿Cómo debo conservar mis chocolates?', N'Guárdalos en un lugar fresco y seco, entre 15 °C y 20 °C, lejos de la luz directa del sol.', 11);
GO