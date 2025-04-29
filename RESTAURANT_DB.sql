/*

CREATE DATABASE RESTAURANT_DB
GO

USE RESTAURANT_DB
GO

*/

create table ROL(
IdRol int primary key identity,
Descripcion varchar(50),
FechaRegistro datetime default getdate()
)
go

create table PERMISO(
IdPermiso int primary key identity,
IdRol int references ROL(IdRol),
NombreMenu varchar(50),
FechaRegistro datetime default getdate()
)
go

create table CLIENTE(
IdCliente int primary key identity,
Apellido varchar(50),
Nombre varchar(50),
Direccion varchar(150),
Telefono varchar(50),
Estado bit,
FechaRegistro datetime default getdate()
)
go

create table USUARIO(
IdUsuario int primary key identity,
Documento varchar(50),
Apellido varchar(50),
Nombre varchar(50),
Direccion varchar(100),
Correo varchar(50),
Clave varchar(150),
IdRol int references ROL(IdRol),
Estado bit,
FechaRegistro datetime default getdate()
)
go

create table CATEGORIA(
IdCategoria int primary key identity,
Descripcion varchar(100),
Estado bit,
FechaRegistro datetime default getdate()
)
go

create table PRODUCTO(
IdProducto int primary key identity,
Codigo varchar(50),
Nombre varchar(50),
Descripcion varchar(200),
IdCategoria int references CATEGORIA(IdCategoria),
Stock int not null default 0,
Precio decimal(10,2) default 0,
imagen image,
Estado bit,
FechaRegistro datetime default getdate()
)
go

create table VENTA(
IdVenta int primary key identity,
IdUsuario int references USUARIO(IdUsuario),
TipoDocumento varchar(50),
NumeroDocumento varchar(50),
DocumentoCliente varchar(50),
ApellidoCliente varchar(100),
NombreCliente varchar(100),
MontoPago decimal(10,2),
MontoCambio decimal(10,2),
MontoTotal decimal(10,2),
DesMetPago varchar(100),
FechaRegistro datetime default getdate(),

EstadoEntrega BIT DEFAULT 0,
EstadoPago BIT DEFAULT 0,

)
go

create table DETALLE_VENTA(
IdDetalleVenta int primary key identity,
IdVenta int references VENTA(IdVenta),
IdProducto int references PRODUCTO(IdProducto),
Precio decimal(10,2),
Cantidad int,
SubTotal decimal(10,2),
FechaRegistro datetime default getdate()
)
go

--##########################################################################--
------------------------------- Carga de datos -------------------------------

-- Se inserta los tipos de Roles

insert into rol (Descripcion)
values('Empleado')
go

insert into rol (Descripcion)
values('Administrador')
go

insert into rol (Descripcion)
values('SuperAdministrador')
go


--Consulta la Tabla Rol
 select * from rol


 -- Se inserta los tipos de Permisos para el sistema de Menu

 -- Permisos del tipo "EMPLEADO"
 insert into PERMISO(IdRol,NombreMenu) 
 values
(1,'menuventas'),
(1,'menuclientes'),
(1,'menuverproductos'),
(1,'menuacercade')
go

 -- Permisos del tipo "ADMINISTRADOR"
 insert into PERMISO(IdRol,NombreMenu) 
 values
(2,'menumantenedor'),
(2,'menuclientes'),
(2,'menudetalleventa'),
(2,'menuestadisticas'),
(2,'menuusuarios'),
(2,'menubackup'),
(2,'menuacercade')
go

/*
 -- Permisos del tipo "SUPERADMINISTRADOR"
 insert into PERMISO(IdRol,NombreMenu) 
 values
(3,'menuusuarios'),
(3,'menubackup'),
(3,'menuacercade')
go
*/

--Consulta la Tabla Permiso
select * from PERMISO
go


---------------- Se generan los metodos para manipular el formulario de USUARIOS ----------------

--Declaracion de Variables para manejo de los metodos
declare @idusuariogenerado int
declare @mensaje varchar(500)
go


CREATE PROC SP_REGISTRARUSUARIO(
@Documento varchar(50),
@Apellido varchar(100),
@Nombre varchar(100),
@Direccion varchar(100),
@Correo varchar(100),
@Clave varchar(100),
@IdRol int,
@Estado bit,
@IdUsuarioResultado int output,
@Mensaje varchar(500) output
)
as
begin
     set @IdUsuarioResultado = 0
	 set @Mensaje = ''

	 if not exists(select * from USUARIO where Documento = @Documento)
	 begin
	      insert into usuario(Documento,Apellido,Nombre,Direccion,Correo,Clave,IdRol,Estado) values
		  (@Documento,@Apellido,@Nombre,@Direccion,@Correo,@Clave,@IdRol,@Estado)

		  set @IdUsuarioResultado = SCOPE_IDENTITY()
     end
	 else
	     set @Mensaje = 'Numero de documento ya existente'
end

go

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_EDITARUSUARIO(
@IdUsuario int, 
@Documento varchar(50),
@Apellido varchar(100),
@Nombre varchar(100),
@Direccion varchar(100),
@Correo varchar(100),
@Clave varchar(100),
@IdRol int,
@Estado bit,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
     set @Respuesta = 0
	 set @Mensaje = ''

	 if not exists(select * from USUARIO where Documento = @Documento and IdUsuario != @IdUsuario)
	 begin
	      update usuario set 
		  Documento = @Documento,
		  Apellido = @Apellido,
		  Nombre = @Nombre,
		  Direccion = @Direccion,
		  Correo = @Correo,
		  Clave = @Clave,
		  IdRol = @IdRol,
		  Estado = @Estado
		  where IdUsuario = @IdUsuario

		  set @Respuesta = 1

     end
	 else
	     set @Mensaje = '!Numero de documento ya existente!'

end

go

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_ELIMINARUSUARIO(
@IdUsuario int, 
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
     set @Respuesta = 0
	 set @Mensaje = ''
	 declare @pasoreglas bit = 1

	 if exists (select * from VENTA V
	 inner join usuario u on u.IdUsuario = V.IdUsuario
	 where U.IDUSUARIO = @IdUsuario
	 )
	 begin 
	    set @pasoreglas = 0
        set @Respuesta = 0
	    set @Mensaje = @Mensaje + 'No se puede eliminar por que el usuario se encuentra relacionado a una VENTA\n'
     end

	 if(@pasoreglas = 1)
	 begin
	    delete from USUARIO where IdUsuario = @IdUsuario
		set @Respuesta = 1
	 end
end

go


---------------- Se generan los metodos para manipular el formulario de CLIENTES ----------------

-- Declaración de Variables para manejo de los métodos
DECLARE @idclientegenerado INT;
DECLARE @mensaje VARCHAR(500);
GO

CREATE PROC SP_REGISTRARCLIENTE(
    @Apellido VARCHAR(100),
    @Nombre VARCHAR(100),
    @Direccion VARCHAR(100),
    @Telefono VARCHAR(100),
    @Estado BIT,
    @IdClienteResultado INT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    SET @IdClienteResultado = 0;
    SET @Mensaje = '';

    -- Inserción del cliente sin el campo Documento
    INSERT INTO cliente (Apellido, Nombre, Direccion, Telefono, Estado) 
    VALUES (@Apellido, @Nombre, @Direccion, @Telefono, @Estado);

    -- Recuperar el ID generado
    SET @IdClienteResultado = SCOPE_IDENTITY();
END;
GO

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_EDITARCLIENTE(
    @IdCliente INT,
    @Apellido VARCHAR(100),
    @Nombre VARCHAR(100),
    @Direccion VARCHAR(100),    
    @Telefono VARCHAR(100),
    @Estado BIT,
    @Respuesta BIT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    SET @Respuesta = 0;
    SET @Mensaje = '';

    -- Actualización de cliente sin validar Documento
    UPDATE cliente
    SET 
        Apellido = @Apellido,
        Nombre = @Nombre,
        Direccion = @Direccion,      
        Telefono = @Telefono,
        Estado = @Estado
    WHERE IdCliente = @IdCliente;

    SET @Respuesta = 1;
END;
GO

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_ELIMINARCLIENTE(
@IdCliente int, 
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
     set @Respuesta = 0
	 set @Mensaje = ''

	delete from CLIENTE where IdCliente = @IdCliente
	set @Respuesta = 1
end

go

---------------- Se generan los metodos para manipular el formulario de CATEGORIA ----------------

--Declaracion de Variables para manejo de los metodos
declare @idcategoriagenerado int
declare @mensaje varchar(500)
go


CREATE PROCEDURE SP_REGISTRARCATEGORIA(
    @Descripcion VARCHAR(50),
    @Estado BIT,
    @IdCategoriaResultado INT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    SET @IdCategoriaResultado = 0
    SET @Mensaje = ''

    IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion)
    BEGIN
        INSERT INTO categoria (Descripcion, Estado) VALUES
        (@Descripcion, @Estado)

        SET @IdCategoriaResultado = SCOPE_IDENTITY()
    END
    ELSE
    BEGIN
        SET @Mensaje = 'La categoría con la misma descripción ya existe'
    END
END
GO


-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_EDITARCATEGORIA(
@IdCategoria int, 
@Descripcion varchar(50),
@Estado bit,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
     set @Respuesta = 0
	 set @Mensaje = ''

	 if not exists(select * from CATEGORIA where Descripcion = @Descripcion and IdCategoria != @IdCategoria)
	 begin
	      update categoria set 
		  Descripcion = @Descripcion,
		  Estado = @Estado
		  where IdCategoria = @IdCategoria

		  set @Respuesta = 1
     end
	 else
	     set @Mensaje = '!La Categoria ya existente!'

end

go

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_ELIMINARCATEGORIA(
@IdCategoria int, 
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
     set @Respuesta = 0
	 set @Mensaje = ''

	delete from CATEGORIA where IdCategoria = @IdCategoria
	set @Respuesta = 1

end

go

---------------- Se generan los metodos para manipular el formulario de PRODUCTOS ----------------

--Declaracion de Variables para manejo de los metodos
declare @idproductogenerado int
declare @mensaje varchar(500)
go

-- Definir el procedimiento almacenado
CREATE PROC SP_REGISTRARPRODUCTO(
    @Codigo varchar(50),
    @Nombre varchar(100),
    @Descripcion varchar(100),
    @IdCategoria int,
	@Stock int,
	@Precio decimal,
    @Estado bit,
    @Resultado int output,
    @Mensaje varchar(500) output
)
as
begin
    set @Resultado = 0
    set @Mensaje = ''

    -- Verificar si el producto ya existe por su Código
    if not exists(select * from PRODUCTO where Codigo = @Codigo)
    begin
        -- Insertar el nuevo producto
        insert into PRODUCTO (Codigo, Nombre, Descripcion, IdCategoria,Stock,Precio,Estado) 
        values (@Codigo, @Nombre, @Descripcion, @IdCategoria,@Stock,@Precio,@Estado)

        -- Obtener el ID del producto recién insertado
        set @Resultado = SCOPE_IDENTITY()
    end
    else
        set @Mensaje = 'Código ya existente'

end

go

-------------------------------------------------------------------------------------------------------------------

-- Definir el procedimiento almacenado
CREATE PROC SP_EDITARPRODUCTO (
    @IdProducto int,
    @Codigo varchar(50),
    @Nombre varchar(100),
    @Descripcion varchar(100),
    @IdCategoria int,
	@Stock int,
	@Precio decimal,
    @Estado bit,
    @Resultado bit output,
    @Mensaje varchar(500) output
)
as
begin
    set @Resultado = 1

    -- Verificar si otro producto ya tiene el mismo Código
    if not exists(select * from PRODUCTO where codigo = @Codigo and IdProducto != @IdProducto)

        -- Actualizar el producto existente con los nuevos valores
        update PRODUCTO set 
            codigo = @Codigo,
            Nombre = @Nombre,
            Descripcion = @Descripcion,
            IdCategoria = @IdCategoria,
			Stock = @Stock,
			Precio = @Precio,
            Estado = @Estado
            where IdProducto = @IdProducto
     else
	 begin
        -- Establecer la respuesta como 1 para indicar éxito
        set @Resultado = 0
		set @Mensaje = '¡Código de producto ya existente!'
    end
end

go

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_ELIMINARPRODUCTO(
@IdProducto int, 
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
     set @Respuesta = 0
	 set @Mensaje = ''
	 declare @pasoreglas bit = 1

	 if exists (select * from DETALLE_VENTA dv
	 inner join PRODUCTO p on p.IdProducto = dv.IdProducto
	 where dv.IdProducto = @IdProducto
	 )
	 begin 
	    set @pasoreglas = 0
        set @Respuesta = 0
	    set @Mensaje = @Mensaje + 'No se puede eliminar el producto por que se encuentra relacionado a una VENTA'
     end

	 if(@pasoreglas = 1)
	 begin
	    delete from PRODUCTO where IdProducto = @IdProducto
		set @Respuesta = 1
	 end

end

go

-------------------------------------------------------------------------------------------------------------------

--Se insertan un Usuarios con sus datos correspondientes
-- LAS CLAVES AL HABER SIDO ENCRIPTADAS SERAN REGISTRADAS DE TAL MODO, EL VALOR DE TODAS ES LA MISMA (123)

insert into USUARIO(Documento, Apellido, Nombre,Direccion, Correo, Clave, IdRol, Estado)
values
('11111111', 'ApeUsuario1','NomUsuario1','Casita 1', '@gmail.com', '$2a$11$11YH.2nEoLsOjMogIXeY9eSeQ1n3FwGgc2/K0cJejVIHGc0k3ld2G' ,1,1),
('22222222', 'ApeUsuario2','NomUsuario2','Casita 2', '@gmail.com', '$2a$11$vfJOhNuMlA0wV6iCbWOmHO8FsYhYtpkmZyyWaIdP.3T77GHdCGB9W' ,1,1),
('33333333', 'ApeUsuario3','NomUsuario3','Casita 3', '@gmail.com', '$2a$11$kcGAIS1MGXAcK2ZUOltByem6TRVBaZsJjGEPtSqjscM3CFI4QLDrC' ,2,1),
('44444444', 'ApeUsuario4','NomUsuario4','Casita 4', '@gmail.com', '$2a$11$QktLppcleoNEj9yAjwDibubUlGmIx9uIpxNhfvpUgsK.MHshqe2C2' ,3,1),

('1', 'ApeUsuario5','NomUsuario5','Casita 5', '@gmail.com', '$2a$11$OqAsmqaReh5S9g4JEZZEzuvXRe08FfwOeGZyzkY4al6hdw1FPskt.' ,1,1),
('2', 'ApeUsuario6','NomUsuario6','Casita 5', '@gmail.com', '$2a$11$OqAsmqaReh5S9g4JEZZEzuvXRe08FfwOeGZyzkY4al6hdw1FPskt.' ,1,1),
('3', 'ApeUsuario7','NomUsuario7','Casita 5', '@gmail.com', '$2a$11$OqAsmqaReh5S9g4JEZZEzuvXRe08FfwOeGZyzkY4al6hdw1FPskt.' ,2,1),
('4', 'ApeUsuario8','NomUsuario8','Casita 5', '@gmail.com', '$2a$11$OqAsmqaReh5S9g4JEZZEzuvXRe08FfwOeGZyzkY4al6hdw1FPskt.' ,3,1);
go


--Consulta la Tabla Usuario
select * from usuario


-------------------------------------------------------------------------------------------------------------------

-- Generar 10 registros en la tabla CLIENTE con números de documento de Argentina (DNI)
INSERT INTO CLIENTE (Apellido, Nombre, Direccion, Telefono, Estado)
VALUES
('(A)ANONIMO', '(A)ANONIMO', '(A)ANONIMO', '0000000000', 1),
('Gómez', 'Ana', 'Calle 123', '123456789', 1),
('López', 'Juan', 'Avenida 456', '987654321', 1),
('Rodríguez', 'Lucía', 'Calle Principal', '555555555', 1),
('Martínez', 'Carlos', 'Boulevard 789', '777777777', 1),
('Fernández', 'Sofía', 'Calle Secundaria', '999999999', 1),
('Pérez', 'Luis', 'Calle de la Plaza', '111111111', 1),
('García', 'María', 'Avenida Central', '222222222', 1),
('Díaz', 'Pedro', 'Calle 456', '333333333', 1),
('Vargas', 'Elena', 'Calle 789', '444444444', 1);


select * from cliente

-------------------------------------------------------------------------------------------------------------------

--Se insertan una Categoria con sus datos correspondientes

insert into	CATEGORIA(Descripcion,Estado)
values
('Placa',1),
('Modulo',1),
('Sensor',1);
go

select * from CATEGORIA
go

-------------------------------------------------------------------------------------------------------------------

select * from PRODUCTO

--Pone por defecto el Estado en activo del Producto
update PRODUCTO set	Estado = 1

-- Insertar registros en la tabla PRODUCTO
INSERT INTO PRODUCTO (Codigo, Nombre, Descripcion, IdCategoria, Stock, Precio, Estado)
VALUES
('ARD001', 'Arduino Uno', 'Placa de desarrollo basada en ATmega328P', 1, 50, 10900.00, 1),
('ARD002', 'Arduino Nano', 'Versión compacta de la placa Arduino', 2, 30, 8500.00, 1),
('ARD003', 'Sensor de Temperatura', 'Sensor de temperatura para proyectos Arduino', 1, 40, 5500.00, 1),
('ARD004', 'Módulo Bluetooth HC-05', 'Módulo Bluetooth para comunicación inalámbrica', 3, 20, 6300.00, 1),
('ARD005', 'Servomotor SG90', 'Servomotor pequeño para proyectos de robótica', 2, 60, 3300.00, 1),
('ARD006', 'Pantalla LCD 16x2', 'Pantalla alfanumérica para mostrar información', 1, 10, 7200.00, 1),
('ARD007', 'Kit de Inicio Arduino', 'Kit completo para principiantes en Arduino', 2, 45, 22000.00, 1),
('ARD008', 'Módulo WiFi ESP8266', 'Módulo WiFi para conectar proyectos a la red', 3, 55, 5000.00, 1),
('ARD009', 'Sensor de Movimiento PIR', 'Sensor de movimiento para detección', 1, 25, 3900.00, 1),
('ARD010', 'Motor Paso a Paso', 'Motor para control preciso de movimiento', 2, 15, 5800.00, 1),
('ARD011', 'Shield Ethernet W5100', 'Shield para conectar Arduino a la red Ethernet', 1, 48, 11900.00, 1),
('ARD012', 'Módulo GPS NEO-6M', 'Módulo GPS para proyectos de localización', 3, 32, 14900.00, 1),
('ARD013', 'Sensor de Luz LDR', 'Sensor de luz para medir niveles de iluminación', 2, 37, 2800.00, 1),
('ARD014', 'Módulo Relé', 'Módulo para controlar dispositivos externos', 1, 22, 6600.00, 1),
('ARD015', 'Módulo RFID RC522', 'Módulo para lectura de tarjetas RFID', 3, 18, 9400.00, 1),
('ARD016', 'Joystick Arduino', 'Joystick para control manual en proyectos', 2, 28, 8300.99, 1),
('ARD017', 'Sensor Ultrasónico HC-SR04', 'Sensor de distancia para mediciones precisas', 1, 42, 3500.00, 1),
('ARD018', 'Módulo RTC DS3231', 'Módulo de tiempo real para llevar el tiempo', 3, 13, 6000.00, 1),
('ARD019', 'Kit de Sensores Arduino', 'Kit variado con sensores para proyectos', 2, 19, 16500.00, 1),
('ARD020', 'Display 7 Segmentos', 'Display para mostrar números en proyectos', 1, 33, 3300.00, 1);


---------------- Se generan los metodos para manipular el formulario de Ventas ----------------

CREATE TYPE dbo.EDetalle_Venta AS TABLE
(
    IdProducto INT,
    Precio DECIMAL(18, 2),
    Cantidad INT,
    SubTotal DECIMAL(18, 2)
);
GO


CREATE PROCEDURE usp_RegistrarVenta
(
    @IdUsuario INT,
    @TipoDocumento VARCHAR(500),
    @NumeroDocumento VARCHAR(500),
    @DocumentoCliente VARCHAR(500),
    @ApellidoCliente VARCHAR(100),
    @NombreCliente VARCHAR(100),
    @MontoPago DECIMAL(18, 2),
    @MontoCambio DECIMAL(18, 2),
    @MontoTotal DECIMAL(18, 2),
    @DesMetPago VARCHAR(100),
    @DetalleVenta EDetalle_Venta READONLY,
    @Resultado BIT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        DECLARE @IdVenta INT = 0;
        SET @Resultado = 1;
        SET @Mensaje = '';

        BEGIN TRANSACTION Registro;

        INSERT INTO VENTA(IdUsuario, TipoDocumento, NumeroDocumento, DocumentoCliente, ApellidoCliente, NombreCliente, MontoPago, MontoCambio, MontoTotal, DesMetPago, EstadoEntrega, EstadoPago)
        VALUES (@IdUsuario, @TipoDocumento, @NumeroDocumento, @DocumentoCliente, @ApellidoCliente, @NombreCliente, @MontoPago, @MontoCambio, @MontoTotal, @DesMetPago, 0, 0);

        SET @IdVenta = SCOPE_IDENTITY();

        INSERT INTO DETALLE_VENTA(IdVenta, IdProducto, Precio, Cantidad, SubTotal)
        SELECT @IdVenta, IdProducto, Precio, Cantidad, SubTotal FROM @DetalleVenta;

        COMMIT TRANSACTION Registro;
    END TRY
    BEGIN CATCH
        SET @Resultado = 0;
        SET @Mensaje = ERROR_MESSAGE();
        ROLLBACK TRANSACTION Registro;
    END CATCH;
END;
GO


--------------------------------------------- Procedimiento para generar una venta ----------------------------------------------------------

DECLARE @DetalleVenta EDetalle_Venta;

INSERT INTO @DetalleVenta (IdProducto, Precio, Cantidad, SubTotal)
VALUES
(1, 50.00, 2, 100.00),
(2, 30.00, 3, 90.00),
(3, 25.00, 1, 25.00);

DECLARE @Resultado BIT;
DECLARE @Mensaje VARCHAR(500);

---- Otros parámetros
DECLARE @IdUsuario INT = 5;  -- Proporciona el valor correcto para @IdUsuario
DECLARE @TipoDocumento VARCHAR(500) = 'Factura';  -- Proporciona el valor correcto para @TipoDocumento
DECLARE @NumeroDocumento VARCHAR(500) = '00001';  -- Proporciona el valor correcto para @NumeroDocumento
DECLARE @DocumentoCliente VARCHAR(500) = '1';  -- Proporciona el valor correcto para @DocumentoCliente
DECLARE @ApellidoCliente VARCHAR(100) = 'Gómez';  -- Proporciona el valor correcto para @ApellidoCliente
DECLARE @NombreCliente VARCHAR(100) = 'Ana';  -- Proporciona el valor correcto para @NombreCliente
DECLARE @MontoPago DECIMAL(18, 2) = 300.00;  -- Proporciona el valor correcto para @MontoPago
DECLARE @MontoCambio DECIMAL(18, 2) = 50.00;  -- Proporciona el valor correcto para @MontoCambio
DECLARE @MontoTotal DECIMAL(18, 2) = 350.00;  -- Proporciona el valor correcto para @MontoTotal
DECLARE @DesMetPago VARCHAR(100) = 'Efectivo';  -- Proporciona el valor correcto para @DesMetPago

 --Llamada al procedimiento almacenado

EXEC usp_RegistrarVenta
    @IdUsuario = @IdUsuario,
    @TipoDocumento = @TipoDocumento,
    @NumeroDocumento = @NumeroDocumento,
    @DocumentoCliente = @DocumentoCliente,
    @ApellidoCliente = @ApellidoCliente,
    @NombreCliente = @NombreCliente,
    @MontoPago = @MontoPago,
    @MontoCambio = @MontoCambio,
    @MontoTotal = @MontoTotal,
    @DesMetPago = @DesMetPago,
    @DetalleVenta = @DetalleVenta,  -- Pasa la tabla tipo como parámetro directamente
    @Resultado = @Resultado OUTPUT,  -- Agrega los parámetros de salida
    @Mensaje = @Mensaje OUTPUT;

IF @Resultado = 1
    PRINT 'Venta registrada con éxito';
ELSE
    PRINT 'Error al registrar la venta. Mensaje: ' + @Mensaje;
go

-------------------------------------- Consulta de Detalle de Venta ------------------------------------------

select v.IdVenta,u.Apellido,u.Nombre,
v.DocumentoCliente, v.ApellidoCliente,v.NombreCliente,
v.TipoDocumento,v.NumeroDocumento,
v.MontoPago,v.MontoCambio,v.MontoTotal,v.DesMetPago,
convert (char(10),v.FechaRegistro,103)[FechaRegistro]
from VENTA v
inner join USUARIO u on u.IdUsuario = v.IdUsuario
where v.NumeroDocumento = '00001'
go

select 
p.Nombre,dv.Precio,dv.Cantidad,dv.SubTotal
from DETALLE_VENTA dv
inner join PRODUCTO p on p.IdProducto = dv.IdProducto
where dv.IdVenta = 1
go

------------------------------------- Procedimiento Almacenado Reporte de Ventas -------------------------------------

CREATE PROCEDURE sp_ReporteVentas(
    @FechaInicio VARCHAR(50),
    @FechaFin VARCHAR(50),
    @IdUsuario INT
)
AS
BEGIN
    -- Establecer el formato de fecha para asegurar interpretación correcta
    SET DATEFORMAT dmy;

    -- Verificar si el IdUsuario tiene ventas asociadas
    IF NOT EXISTS (SELECT 1 FROM VENTA WHERE IdUsuario = @IdUsuario)
    BEGIN
        -- Si el IdUsuario no tiene ventas asociadas, traer todas las ventas
        SELECT
            V.IdVenta,
            CONVERT(char(10), v.FechaRegistro, 103) AS [FechaRegistro],
            V.TipoDocumento,
            V.NumeroDocumento,
            V.MontoTotal,
            U.Apellido + ', ' + U.Nombre AS usuarioregistro,
            C.Apellido + ', ' + C.Nombre AS nombrecompletocliente,
            V.DesMetPago,
            CASE 
                WHEN V.EstadoEntrega = 1 THEN 'SI' 
                ELSE 'NO' 
            END AS EstadoEntrega,
            CASE 
                WHEN V.EstadoPago = 1 THEN 'SI' 
                ELSE 'NO' 
            END AS EstadoPago
        FROM
            VENTA V
        INNER JOIN
            USUARIO U ON V.IdUsuario = U.IdUsuario
        LEFT JOIN
            CLIENTE C ON V.DocumentoCliente = C.IdCliente
        WHERE
            V.FechaRegistro BETWEEN 
                CONVERT(DATETIME, @FechaInicio, 103) 
                AND DATEADD(SECOND, -1, DATEADD(DAY, 1, CONVERT(DATETIME, @FechaFin, 103)));
    END
    ELSE
    BEGIN
        -- Si el IdUsuario tiene ventas asociadas, traer solo sus ventas
        SELECT
            V.IdVenta,
            CONVERT(char(10), v.FechaRegistro, 103) AS [FechaRegistro],
            V.TipoDocumento,
            V.NumeroDocumento,
            V.MontoTotal,
            U.Apellido + ', ' + U.Nombre AS usuarioregistro,
            C.Apellido + ', ' + C.Nombre AS nombrecompletocliente,
            V.DesMetPago,
            CASE 
                WHEN V.EstadoEntrega = 1 THEN 'SI' 
                ELSE 'NO' 
            END AS EstadoEntrega,
            CASE 
                WHEN V.EstadoPago = 1 THEN 'SI' 
                ELSE 'NO' 
            END AS EstadoPago
        FROM
            VENTA V
        INNER JOIN
            USUARIO U ON V.IdUsuario = U.IdUsuario
        LEFT JOIN
            CLIENTE C ON V.DocumentoCliente = C.IdCliente
        WHERE
            V.FechaRegistro BETWEEN 
                CONVERT(DATETIME, @FechaInicio, 103) 
                AND DATEADD(SECOND, -1, DATEADD(DAY, 1, CONVERT(DATETIME, @FechaFin, 103)))
            AND V.IdUsuario = @IdUsuario;
    END
END

GO

--------------------------- Funcion almacenada para cambiar estado de entregado ---------------------------------------

CREATE PROCEDURE usp_ActualizarEstadoEntrega
(
    @IdVenta INT,          -- ID de la venta a actualizar
    @EstadoEntrega BIT,    -- Nuevo estado: 1 para "Entregado", 0 para "No entregado"
    @Resultado BIT OUTPUT, -- Indica si la operación fue exitosa
    @Mensaje VARCHAR(500) OUTPUT -- Mensaje en caso de error
)
AS
BEGIN
    BEGIN TRY
        -- Inicializamos los valores de salida
        SET @Resultado = 1;
        SET @Mensaje = 'Estado actualizado correctamente.';

        -- Actualizamos el estado de la venta
        UPDATE VENTA
        SET EstadoEntrega = @EstadoEntrega
        WHERE IdVenta = @IdVenta;

        -- Validamos si se encontró la venta
        IF @@ROWCOUNT = 0
        BEGIN
            SET @Resultado = 0;
            SET @Mensaje = 'Venta no encontrada.';
        END
    END TRY
    BEGIN CATCH
        -- En caso de error, devolvemos el mensaje del error
        SET @Resultado = 0;
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH;
END;
GO

------------------------------ Permite modificar el estado de entrega --------------------------------------------

DECLARE @Resultado BIT;
DECLARE @Mensaje VARCHAR(500);

EXEC usp_ActualizarEstadoEntrega
    @IdVenta = 1,
    @EstadoEntrega = 1,
    @Resultado = @Resultado OUTPUT,
    @Mensaje = @Mensaje OUTPUT;

SELECT @Resultado AS Resultado, @Mensaje AS Mensaje;
go

------------------------------ Funcion almacenada para cambiar estado de pago --------------------------------------

CREATE PROCEDURE usp_ActualizarEstadoPago
(
    @IdVenta INT,          -- ID de la venta a actualizar
    @EstadoPago BIT,       -- Nuevo estado: 1 para "Pagado", 0 para "No pagado"
    @Resultado BIT OUTPUT, -- Indica si la operación fue exitosa
    @Mensaje VARCHAR(500) OUTPUT -- Mensaje en caso de error
)
AS
BEGIN
    BEGIN TRY
        -- Inicializamos los valores de salida
        SET @Resultado = 1;
        SET @Mensaje = 'Estado de pago actualizado correctamente.';

        -- Actualizamos el estado de pago de la venta
        UPDATE VENTA
        SET EstadoPago = @EstadoPago
        WHERE IdVenta = @IdVenta;

        -- Validamos si se encontró la venta
        IF @@ROWCOUNT = 0
        BEGIN
            SET @Resultado = 0;
            SET @Mensaje = 'Venta no encontrada.';
        END
    END TRY
    BEGIN CATCH
        -- En caso de error, devolvemos el mensaje del error
        SET @Resultado = 0;
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH;
END;
GO

------------------------------ Permite modificar el estado de pago --------------------------------------------

DECLARE @Resultado BIT;
DECLARE @Mensaje VARCHAR(500);

EXEC usp_ActualizarEstadoPago
    @IdVenta = 1,
    @EstadoPago = 1,
    @Resultado = @Resultado OUTPUT,
    @Mensaje = @Mensaje OUTPUT;

SELECT @Resultado AS Resultado, @Mensaje AS Mensaje;
go


---------------------------------------------- Consulta de las ventas ----------------------------------------------

SELECT IdVenta, TipoDocumento, NumeroDocumento, DocumentoCliente, ApellidoCliente, NombreCliente, MontoTotal, EstadoEntrega, EstadoPago
FROM VENTA;
go

------------------------------------- Prueba de Procedimiento Almacenado -------------------------------------

DECLARE @FechaInicio VARCHAR(50) = CONVERT(VARCHAR(10), GETDATE(), 103);
DECLARE @FechaFin VARCHAR(50) = CONVERT(VARCHAR(10), GETDATE(), 103);
DECLARE @IdUsuario INT = 5;

EXEC sp_ReporteVentas @FechaInicio, @FechaFin, @IdUsuario;
GO



--########################################################################################################################################

CREATE TABLE SoftwareState (
    ID INT PRIMARY KEY IDENTITY(1,1),  
    FechaInicio DATE NOT NULL,               -- Registra la fecha de la primera ejecución.
    DiasPermitidos INT NOT NULL DEFAULT 15,  -- Días restantes para uso limitado.
    Activado BIT NOT NULL DEFAULT 0,         -- Indica si el software está activado (1) o no (0).
    CodigoActivacion NVARCHAR(50),           -- Código que permite desbloquear el software.	
    FechaActivacion DATETIME NULL,           -- Fecha en la que se activó el software.
    UltimaVerificacion DATETIME NULL             -- Fecha de la última verificación.
);
GO
---------------- Se generan los metodos para manipular el formulario de LICENCIAS ----------------

-- Declaración de Variables para manejo de los métodos
DECLARE @idlicenciagenerado INT;
DECLARE @mensaje VARCHAR(500);
GO

CREATE PROC SP_REGISTRARLICENCIA(
    @CodigoActivacion VARCHAR(50),
    @Activado BIT,
    @FechaInicio DATE, -- Solo la fecha
    @FechaActivacion DATETIME,  -- Solo la hora
	@DiasPermitidos INT,
    
    -- Parámetros de Salida (OUTPUT):
    @IdLicenciaResultado INT OUTPUT, 
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    -- Inicializa las variables de salida:
    SET @IdLicenciaResultado = 0;
    SET @Mensaje = '';

    -- Valida si el número de documento ya está registrado
    IF NOT EXISTS (SELECT 1 FROM SoftwareState WHERE CodigoActivacion = @CodigoActivacion)
    BEGIN
        INSERT INTO SoftwareState (CodigoActivacion, Activado, FechaInicio, FechaActivacion, DiasPermitidos)
        VALUES (@CodigoActivacion, @Activado, @FechaInicio, @FechaActivacion, @DiasPermitidos);

        -- Obtiene el ID generado automáticamente (clave primaria)
        SET @IdLicenciaResultado = SCOPE_IDENTITY();
        SET @Mensaje = 'Licencia registrada exitosamente.';
    END
    ELSE
    BEGIN
        SET @Mensaje = 'Número de Activacion ya existente.';
    END
END;
GO

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_EDITARLICENCIA(
    @ID INT, 
    @CodigoActivacion VARCHAR(50),
    @Activado BIT,
    @FechaInicio DATE, -- Solo la fecha
    @FechaActivacion DATETIME,  -- Solo la hora
	@DiasPermitidos INT,

    -- Parámetros de Salida (OUTPUT):
    @Respuesta BIT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    -- Inicializar variables de salida:
    SET @Respuesta = 0;
    SET @Mensaje = '';

    -- Comprueba si ya existe otro cliente con el mismo número de documento que no sea el cliente actual.
    IF NOT EXISTS (SELECT 1 FROM SoftwareState WHERE CodigoActivacion = @CodigoActivacion AND ID != @ID)
    BEGIN
        -- Actualizar el cliente
        UPDATE SoftwareState
        SET 
            CodigoActivacion = @CodigoActivacion,
            Activado = @Activado,
            FechaInicio = @FechaInicio,
            FechaActivacion = @FechaActivacion,
			DiasPermitidos = @DiasPermitidos

        WHERE ID = @ID; -- Asegura que la operación afecte únicamente al cliente identificado.

        SET @Respuesta = 1; -- Indica que la operación fue exitosa.
        SET @Mensaje = 'Licencia actualizada correctamente.';
    END
    ELSE
    BEGIN
        SET @Mensaje = '¡Número de Activacion ya existente!';
    END
END;
GO

-------------------------------------------------------------------------------------------------------------------

CREATE PROC SP_ELIMINARLICENCIA(
@ID int, 
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
     set @Respuesta = 0
	 set @Mensaje = ''

		delete from SoftwareState where ID = @ID
		set @Respuesta = 1

end
go

-------------------------------------------------------------------------------------------------------------------

INSERT INTO SoftwareState (FechaInicio, DiasPermitidos, Activado, CodigoActivacion, FechaActivacion, UltimaVerificacion)
VALUES (
    GETDATE(),                        -- Fecha actual como fecha de inicio
    20,                                -- Días permitidos iniciales
    0,                                 -- Activado en falso (0) por defecto
    CAST(CONVERT(INT, FORMAT(GETDATE(), 'yyyyMMdd')) * 27 AS VARCHAR),  -- Código de activación generado dinámicamente
    GETDATE(),                        -- Fecha de activación no configurada
    GETDATE()                         -- Fecha de última verificación configurada como la fecha actual
);
go
------------- Consultas para ver poceso de generacion de codigo -------------
SELECT 
    GETDATE() AS FechaActual, 
    FORMAT(GETDATE(), 'yyyyMMdd') AS FechaEnEntero, 
    CONVERT(INT, FORMAT(GETDATE(), 'yyyyMMdd')) AS FechaComoNumero,
    CONVERT(INT, FORMAT(GETDATE(), 'yyyyMMdd')) * 27 AS CodigoActivacionCalculado;

SELECT * FROM SoftwareState;
GO
----------------------------------
/*

DELETE FROM SoftwareState;

*/
--########################################################################################################################################
