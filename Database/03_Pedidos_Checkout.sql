-- =============================================
-- Chocolates SV - Checkout y detalle de pedidos
-- Programador 3
-- Requiere: tablas Pedidos y Productos
-- =============================================

IF COL_LENGTH('Pedidos', 'CodigoOrden') IS NULL
BEGIN
	ALTER TABLE Pedidos
		ADD CodigoOrden NVARCHAR(32) NULL;
END
GO

UPDATE Pedidos
SET CodigoOrden = CONCAT('ORD-', UPPER(LEFT(CONVERT(VARCHAR(32), NEWID()), 28)))
WHERE CodigoOrden IS NULL;
GO

ALTER TABLE Pedidos
	ALTER COLUMN CodigoOrden NVARCHAR(32) NOT NULL;
GO

IF NOT EXISTS (
	SELECT 1 FROM sys.indexes
	WHERE name = 'UX_Pedidos_CodigoOrden'
	  AND object_id = OBJECT_ID('Pedidos')
)
BEGIN
	CREATE UNIQUE INDEX UX_Pedidos_CodigoOrden ON Pedidos(CodigoOrden);
END
GO

IF OBJECT_ID('PedidoDetalles', 'U') IS NULL
BEGIN
	CREATE TABLE PedidoDetalles (
		PedidoDetalleID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
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
			REFERENCES Pedidos(PedidoID),
		CONSTRAINT FK_PedidoDetalles_Productos FOREIGN KEY (ProductoID)
			REFERENCES Productos(ProductoID)
	);

	CREATE INDEX IX_PedidoDetalles_PedidoID ON PedidoDetalles(PedidoID);
END
GO
