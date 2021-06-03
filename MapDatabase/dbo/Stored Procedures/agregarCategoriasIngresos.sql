CREATE procedure [dbo].[agregarCategoriasIngresos]  @usuario int
AS
begin
Declare  @id_categoria int, @descripcion_categoria varchar(30)

--@id_user int, @tipo int, @movimiento varchar(30),
-- declare a cursor

---para los egresos

DECLARE insert_ingresos CURSOR FOR 
SELECT id, descripcion from ingresospordefecto
group by id, descripcion 
--simple cursor in sql server 

-- open cursor and fetch first row into variables
OPEN insert_ingresos
FETCH NEXT FROM insert_ingresos into @id_categoria, @descripcion_categoria

-- check for a new row
WHILE @@FETCH_STATUS=0
BEGIN
-- do complex operation here
Insert into [dbo].[categorias]
           ([id_user],
		    [id_tipo],
			[descripcion])
		    values (@usuario, 1, @descripcion_categoria);
-- get next available row into variables
FETCH NEXT FROM insert_ingresos into @id_categoria, @descripcion_categoria
END
close insert_ingresos
Deallocate insert_ingresos
end

