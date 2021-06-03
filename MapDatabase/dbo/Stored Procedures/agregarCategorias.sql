CREATE procedure [dbo].[agregarCategorias]  @usuario int
AS
begin

exec agregarCategoriasIngresos @usuario;
exec agregarCategoriasEgresos @usuario;

end

