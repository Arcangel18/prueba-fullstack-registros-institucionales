/*
================================================================================
  Pregunta 7 — Esquema de base de datos SQL Server
  Servicio de consulta de registros institucionales

  Puntos cubiertos:
    11. Tabla Registros (salida del endpoint + campos de búsqueda)
    12. Tabla Entidades (convenio, cuota diaria, activo/inactivo)
    13. Tabla LogAccesos (auditoría + cálculo de cuota diaria)
    14. Dos índices justificados por patrón de consulta
    15. Trigger que impide APROBADO si la entidad ya superó su cuota

  Cómo ejecutar (opcional, sobre una BD vacía de demostración):
    USE master;
    CREATE DATABASE SviDb_P7;
    GO
    USE SviDb_P7;
    GO
    -- luego ejecutar este script completo
================================================================================
*/

USE SviDb;
GO

/* --------------------------------------------------------------------------
   12. Entidades autorizadas
   --------------------------------------------------------------------------
   Decisiones de diseño:
   - ApiKey única: autenticación del endpoint sin exponer el Id interno.
   - Activa (bit): estado operativo independiente de las fechas de convenio.
   - FechaInicioConvenio / FechaFinConvenio: vigencia del convenio.
   - CuotaDiaria: configurable POR entidad (no hardcodeada).
   - CHECK CuotaDiaria > 0: evita cuotas inválidas.
   - CHECK de fechas: el fin no puede ser anterior al inicio.
*/
CREATE TABLE dbo.Entidades
(
    Id                    INT            NOT NULL IDENTITY(1, 1),
    Nombre                NVARCHAR(200)  NOT NULL,
    ApiKey                NVARCHAR(200)  NOT NULL,
    Activa                BIT            NOT NULL
        CONSTRAINT DF_Entidades_Activa DEFAULT (1),
    FechaInicioConvenio   DATETIME2(0)   NOT NULL,
    FechaFinConvenio      DATETIME2(0)   NOT NULL,
    CuotaDiaria           INT            NOT NULL
        CONSTRAINT DF_Entidades_CuotaDiaria DEFAULT (100),

    CONSTRAINT PK_Entidades PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Entidades_ApiKey UNIQUE (ApiKey),
    CONSTRAINT CK_Entidades_CuotaDiaria_Positiva
        CHECK (CuotaDiaria > 0),
    CONSTRAINT CK_Entidades_Convenio_Fechas
        CHECK (FechaFinConvenio >= FechaInicioConvenio)
);
GO

/* --------------------------------------------------------------------------
   11. Registros institucionales (tabla principal de consulta)
   --------------------------------------------------------------------------
   Decisiones de diseño:
   - Identificador + Nombre: claves de búsqueda del POST /api/registros/consulta.
     Ambos son obligatorios (el negocio no permite buscar solo por identificador).
   - Estado, NumeroRegistro, FechaEvento, FechaInscripcion: campos de salida
     exigidos por el enunciado.
   - NumeroRegistro único: evita duplicados del número institucional.
*/
CREATE TABLE dbo.Registros
(
    Id                 INT            NOT NULL IDENTITY(1, 1),
    Identificador      NVARCHAR(50)   NOT NULL,
    Nombre             NVARCHAR(150)  NOT NULL,
    Estado             NVARCHAR(50)   NOT NULL,
    NumeroRegistro     NVARCHAR(50)   NOT NULL,
    FechaEvento        DATETIME2(0)   NOT NULL,
    FechaInscripcion   DATETIME2(0)   NOT NULL,

    CONSTRAINT PK_Registros PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Registros_NumeroRegistro UNIQUE (NumeroRegistro)
);
GO

/* --------------------------------------------------------------------------
   13. Log de accesos (auditoría + cuota diaria)
   --------------------------------------------------------------------------
   Decisiones de diseño:
   - EntidadId nullable: permite auditar intentos con API Key inválida/ausente
     (aún no hay entidad asociada).
   - FechaHora: base del conteo diario (CAST a DATE / rango del día).
   - Resultado: APROBADO | RECHAZADO (y variantes de motivo en Motivo).
   - Motivo: causa concreta (CUOTA_DIARIA_EXCEDIDA, CONVENIO_NO_VIGENTE, etc.).
   - TipoConsulta: distingue tipos si el servicio crece.
   - IdentificadorConsultado: traza qué se buscó.
   - FK Restrict: no se borra una entidad si tiene historial de auditoría.
*/
CREATE TABLE dbo.LogAccesos
(
    Id                       BIGINT         NOT NULL IDENTITY(1, 1),
    EntidadId                INT            NULL,
    FechaHora                DATETIME2(0)   NOT NULL
        CONSTRAINT DF_LogAccesos_FechaHora DEFAULT (SYSUTCDATETIME()),
    TipoConsulta             NVARCHAR(50)   NOT NULL
        CONSTRAINT DF_LogAccesos_TipoConsulta DEFAULT (N'CONSULTA_REGISTRO'),
    Resultado                NVARCHAR(30)   NOT NULL,
    Motivo                   NVARCHAR(200)  NOT NULL,
    IdentificadorConsultado  NVARCHAR(50)   NULL,

    CONSTRAINT PK_LogAccesos PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_LogAccesos_Entidades
        FOREIGN KEY (EntidadId) REFERENCES dbo.Entidades (Id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT CK_LogAccesos_Resultado
        CHECK (Resultado IN (N'APROBADO', N'RECHAZADO'))
);
GO

/* --------------------------------------------------------------------------
   14. Índices justificados por patrones de consulta
   --------------------------------------------------------------------------

   Índice 1 — Búsqueda individual por identificador + nombre
   Patrón: WHERE Identificador = @id AND Nombre = @nombre
   (endpoint POST /api/registros/consulta)
*/
CREATE NONCLUSTERED INDEX IX_Registros_Identificador_Nombre
ON dbo.Registros (Identificador, Nombre)
INCLUDE (Estado, NumeroRegistro, FechaEvento, FechaInscripcion);
GO

/*
   Índice 2 — Conteo de consultas diarias aprobadas por entidad
   Patrón: WHERE EntidadId = @id
             AND Resultado = 'APROBADO'
             AND FechaHora >= @inicioDia AND FechaHora < @finDia
   (cálculo de cuota diaria antes de aprobar una consulta)
*/
CREATE NONCLUSTERED INDEX IX_LogAccesos_Entidad_Fecha_Resultado
ON dbo.LogAccesos (EntidadId, FechaHora, Resultado)
INCLUDE (TipoConsulta, Motivo);
GO

/* --------------------------------------------------------------------------
   15. Trigger: impedir APROBADO si la entidad ya superó su cuota diaria
   --------------------------------------------------------------------------
   Decisión:
   - Se eligió TRIGGER (no solo CHECK de fila) porque la regla cruza tablas:
     necesita contar filas en LogAccesos y comparar con Entidades.CuotaDiaria.
   - Solo actúa sobre inserts con Resultado = 'APROBADO' y EntidadId NOT NULL.
   - Usa rango semiabierto [inicio del día UTC, inicio del día siguiente) para
     no depender de CONVERT a texto y no truncar el último instante del día.
   - COUNT(*) + 1 contempla el propio insert que se está evaluando (inserted).
   - Si falla, lanza error 50001 y hace ROLLBACK de la transacción.
*/
CREATE OR ALTER TRIGGER dbo.TR_LogAccesos_ValidarCuotaDiaria
ON dbo.LogAccesos
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM inserted
        WHERE Resultado = N'APROBADO'
          AND EntidadId IS NOT NULL
    )
    BEGIN
        RETURN;
    END;

    IF EXISTS (
        SELECT 1
        FROM inserted AS i
        INNER JOIN dbo.Entidades AS e
            ON e.Id = i.EntidadId
        WHERE i.Resultado = N'APROBADO'
          AND i.EntidadId IS NOT NULL
          AND (
                SELECT COUNT(*)
                FROM dbo.LogAccesos AS l
                WHERE l.EntidadId = i.EntidadId
                  AND l.Resultado = N'APROBADO'
                  AND l.FechaHora >= CAST(CAST(SYSUTCDATETIME() AS date) AS datetime2(0))
                  AND l.FechaHora <  DATEADD(day, 1, CAST(CAST(SYSUTCDATETIME() AS date) AS datetime2(0)))
              ) > e.CuotaDiaria
    )
    BEGIN
        -- THROW aborta el lote y revierte el INSERT (AFTER INSERT ya escribió las filas).
        THROW 50001,
              N'No se puede registrar un acceso APROBADO: la entidad ya alcanzó su cuota diaria configurada.',
              1;
    END;
END;
GO

/*
================================================================================
  Notas de diseño adicionales
  - La cuota también se valida en la capa de aplicación (API). El trigger es
    la salvaguarda a nivel de base de datos (defense in depth), como pide el
    punto 15 del enunciado.
  - Los campos extra de registro administrativo (IP, PDFs, etc.) de la Pregunta 6
    se omiten aquí a propósito: el enunciado de la Pregunta 7 pide el esquema
    del servicio de consulta (puntos 11–15), no el del formulario completo.
================================================================================
*/
