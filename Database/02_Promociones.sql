-- =============================================
-- Chocolates SV - Módulo de Promociones
-- Programador 2
-- Requiere: tabla Productos
-- =============================================

-- ---------- Tabla principal ----------
CREATE TABLE Promociones (
    PromocionID       INT IDENTITY(1,1) PRIMARY KEY,
    Nombre            NVARCHAR(100) NOT NULL,
    Descripcion       NVARCHAR(500) NULL,
    TipoPromocion     NVARCHAR(20)  NOT NULL,   -- Temporada | Producto | Categoria | Combo | Cupon
    TipoDescuento     NVARCHAR(20)  NOT NULL,   -- Porcentaje | MontoFijo
    ValorDescuento    DECIMAL(10,2) NOT NULL,
    PrecioCombo       DECIMAL(10,2) NULL,       -- solo si es Combo
    CodigoCupon       NVARCHAR(30)  NULL,       -- solo si es Cupon
    MontoMinimoCompra DECIMAL(10,2) NULL,       -- ej. cupón válido desde $5
    UsosMaximos       INT NULL,                 -- NULL = ilimitado
    UsosActuales      INT NOT NULL DEFAULT 0,
    CategoriaID       INT NULL,                 -- solo si es Categoria
    FechaInicio       DATETIME2 NOT NULL,
    FechaFin          DATETIME2 NOT NULL,
    Activo            BIT NOT NULL DEFAULT 1,
    FechaCreacion     DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_Promociones_Tipo     CHECK (TipoPromocion IN ('Temporada','Producto','Categoria','Combo','Cupon')),
    CONSTRAINT CK_Promociones_TipoDesc CHECK (TipoDescuento IN ('Porcentaje','MontoFijo')),
    CONSTRAINT CK_Promociones_Valor    CHECK (ValorDescuento >= 0),
    CONSTRAINT CK_Promociones_Fechas   CHECK (FechaFin >= FechaInicio)
);
GO

-- Un código de cupón no se puede repetir (pero varios NULL sí)
CREATE UNIQUE INDEX UX_Promociones_CodigoCupon
    ON Promociones(CodigoCupon) WHERE CodigoCupon IS NOT NULL;
GO

-- ---------- Productos incluidos en una promoción / combo ----------
CREATE TABLE PromocionProductos (
    PromocionID INT NOT NULL,
    ProductoID  INT NOT NULL,
    Cantidad    INT NOT NULL DEFAULT 1,

    CONSTRAINT PK_PromocionProductos PRIMARY KEY (PromocionID, ProductoID),
    CONSTRAINT FK_PromProd_Promociones FOREIGN KEY (PromocionID)
        REFERENCES Promociones(PromocionID) ON DELETE CASCADE,
    CONSTRAINT FK_PromProd_Productos FOREIGN KEY (ProductoID)
        REFERENCES Productos(ProductoID)
);
GO

-- ---------- Datos de prueba ----------
INSERT INTO Promociones
    (Nombre, Descripcion, TipoPromocion, TipoDescuento, ValorDescuento,
     CodigoCupon, MontoMinimoCompra, UsosMaximos, FechaInicio, FechaFin)
VALUES
    ('Cupón Bienvenida', '10% en tu primera compra', 'Cupon', 'Porcentaje', 10,
     'DULCE10', 5.00, 100, '2026-01-01', '2026-12-31'),
    ('Navidad Chocolatosa', '$3 de descuento en compras navideñas', 'Temporada', 'MontoFijo', 3,
     NULL, 15.00, NULL, '2026-12-01', '2026-12-25'),
    ('Cupón vencido', 'Para probar validaciones', 'Cupon', 'Porcentaje', 20,
     'VIEJO20', NULL, NULL, '2025-01-01', '2025-01-31');
GO