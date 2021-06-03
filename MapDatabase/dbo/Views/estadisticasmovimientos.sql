
CREATE VIEW [dbo].[estadisticasmovimientos]
AS
SELECT    ROW_NUMBER() OVER(ORDER BY total DESC) AS row,     fechaFormateada, movimiento, tipo, id_user,  cast(SUM(total) AS real)  AS total
FROM            (SELECT        movimiento, tipo, { fn CONCAT(CAST(YEAR(fecha) AS varchar), CAST(MONTH(fecha) AS varchar)) } AS fechaFormateada, precioUnitario AS total, id_user
                          FROM            dbo.movimientos) AS D
GROUP BY movimiento, fechaFormateada, id_user, total, tipo
