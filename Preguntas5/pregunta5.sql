/*

    Pregunta 5.8

    Migración de consulta Sybase a SQL Server 2019.

*/

SELECT TOP (100)
    e.nombre_entidad,
    l.tipo_consulta,
    COUNT_BIG(*) AS total_consultas,
    CONVERT(char(10), CONVERT(date, l.fecha_hora), 103) AS fecha_formato
FROM dbo.LogAccesos AS l
INNER JOIN dbo.Entidades AS e
    ON e.id = l.entidad_id
WHERE l.fecha_hora >= DATEFROMPARTS(2025, 1, 1)
  AND l.fecha_hora < DATEFROMPARTS(2026, 1, 1)
  AND l.resultado = 'APROBADO'
GROUP BY
    e.nombre_entidad,
    l.tipo_consulta,
    CONVERT(date, l.fecha_hora)
ORDER BY
    total_consultas DESC;




    /*
    Pregunta 5.9
    Problemas de rendimiento y diseño detectados.

    1. Uso de CONVERT(VARCHAR, fecha_hora, 103) dentro del GROUP BY.
       Esto ejecuta una conversión por cada fila y agrupa utilizando texto,
       en lugar de utilizar un tipo date.

       Solución:
       Agrupar con CONVERT(date, fecha_hora) y convertir a formato de
       presentación únicamente en el SELECT.

    2. Uso de BETWEEN con una fecha final sin hora.
       BETWEEN '2025-01-01' AND '2025-12-31' puede excluir registros del
       31 de diciembre posteriores a las 00:00:00.

       Solución:
       Usar un rango semiabierto:
       fecha_hora >= '20250101' AND fecha_hora < '20260101'.

    3. Posible ausencia de un índice adecuado sobre LogAccesos.
       La consulta filtra por resultado y fecha_hora, y luego relaciona
       entidad_id y utiliza tipo_consulta.

       Solución:
       Crear un índice que apoye el filtro y cubra las columnas utilizadas.
*/

CREATE INDEX IX_LogAccesos_Resultado_FechaHora

ON dbo.LogAccesos (resultado,fecha_hora)

INCLUDE (entidad_id,tipo_consulta);