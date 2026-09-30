USE ChocolatesSV;
GO

IF OBJECT_ID('InformacionEmpresa', 'U') IS NULL
BEGIN
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
        FechaActualizacion DATETIME2 NOT NULL CONSTRAINT DF_InformacionEmpresa_FechaActualizacion DEFAULT SYSDATETIME()
    );
END
GO

IF OBJECT_ID('PreguntasFrecuentes', 'U') IS NULL
BEGIN
    CREATE TABLE PreguntasFrecuentes (
        PreguntaID INT IDENTITY(1,1) PRIMARY KEY,
        Pregunta NVARCHAR(255) NOT NULL,
        Respuesta NVARCHAR(MAX) NOT NULL,
        Orden INT NOT NULL CONSTRAINT DF_PreguntasFrecuentes_Orden DEFAULT 0,
        Activo BIT NOT NULL CONSTRAINT DF_PreguntasFrecuentes_Activo DEFAULT 1,
        FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_PreguntasFrecuentes_FechaCreacion DEFAULT SYSDATETIME()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM InformacionEmpresa)
BEGIN
    INSERT INTO InformacionEmpresa (NombreEmpresa, Descripcion, Mision, Vision)
    VALUES (
        'Chocolates SV',
        'Chocolates SV es una chocolatería artesanal que elabora trufas, tabletas y cajas de regalo con cacao de calidad, pensadas para compartir y regalar.',
        'Ofrecer chocolates artesanales de excelente calidad que endulcen los momentos importantes de nuestros clientes.',
        'Ser la chocolatería artesanal de referencia, reconocida por su sabor, su calidad y su servicio.'
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM PreguntasFrecuentes)
BEGIN
    INSERT INTO PreguntasFrecuentes (Pregunta, Respuesta, Orden)
    VALUES
        ('¿Cómo puedo rastrear mi pedido?',
         'Ingresa tu número de orden y el correo electrónico que usaste al comprar en la sección de rastreo de pedidos. Verás el estado actual de tu pedido.', 1),
        ('¿Dónde encuentro mi número de orden?',
         'El número de orden se muestra al finalizar tu compra. Guárdalo, porque junto con tu correo electrónico lo necesitarás para rastrear tu pedido.', 2),
        ('¿Qué estados puede tener mi pedido?',
         'Un pedido puede estar Pendiente, Confirmado, En Preparación, Enviado, Entregado o Cancelado.', 3),
        ('¿Puedo usar un cupón de descuento?',
         'Sí. Ingresa tu código de cupón durante la compra. El descuento se aplica si el cupón está vigente y tu compra cumple con el monto mínimo requerido.', 4),
        ('¿Cómo elijo la fecha de entrega?',
         'Al finalizar tu compra seleccionas la fecha en la que deseas recibir tu pedido.', 5),
        ('¿Puedo cancelar mi pedido?',
         'Puedes solicitar la cancelación mientras tu pedido no haya sido enviado. Contáctanos lo antes posible.', 6);
END
GO