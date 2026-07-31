# Evaluación Fullstack — Preguntas 4, 5, 6 y 7

API REST en .NET 8 para consultar registros institucionales (con API Key,
convenio, cuota diaria y auditoría), migración SQL Sybase → SQL Server,
formulario React de registro de entidades y esquema SQL Server del servicio.

## Tecnologías

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core + SQL Server
- xUnit + Moq
- Swagger
- React + Vite (Pregunta 6)
- Docker

## Arquitectura (Pregunta 4)

- Controller: solicitudes y respuestas HTTP
- Service: reglas de negocio
- Repository: acceso a datos
- DTO: entrada / salida
- EF Core: persistencia en SQL Server

## Requisitos

- .NET SDK 8
- Docker (SQL Server)
- Node.js 18+ (solo para el frontend)

## 1. Ejecutar SQL Server

```bash
docker run \
  --name sqlserver-registros \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=password123" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

La cadena de conexión en `appsettings.json` usa la misma contraseña: `password123`.

## 2. Crear la base de datos y ejecutar la API

```bash
cd RegistrosInstitucionales.Api
dotnet ef database update
dotnet run
```

La API queda en `http://localhost:5160` (Swagger en `/swagger`).

## 3. Ejecutar las pruebas

Desde la raíz:

```bash
dotnet test
```

## Endpoint de consulta (Pregunta 4)

`POST /api/registros/consulta`

### Header

```
X-API-Key: API-KEY-PRUEBA-123
```

### Body

```json
{
  "identificador": "8-123-456",
  "nombre": "Juan Pérez"
}
```

### Respuestas HTTP

- 200: consulta exitosa
- 400: datos de entrada inválidos
- 401: API Key ausente o inválida
- 403: entidad inactiva o convenio no vigente
- 404: registro no encontrado
- 429: cuota diaria excedida
- 500: error interno

---

## Pregunta 5 — Migración Sybase a SQL Server

### Punto 8 y 9

Consulta migrada, problemas de rendimiento e índices en:

`Preguntas5/pregunta5.sql`

### Punto 10 — LINQ / EF Core

- `RegistrosInstitucionales.Api/DTOs/HistorialAccesoDto.cs`
- `RegistrosInstitucionales.Api/Repositories/IReporteRepository.cs`
- `RegistrosInstitucionales.Api/Repositories/ReporteRepository.cs`

---

## Pregunta 6 — Formulario de registro de entidad

Frontend: `Pregunta6/registros-institucionales-web/`

Backend: `POST /api/entidades` (multipart/form-data) en `EntidadesController`.

### Ejecutar el frontend

```bash
cd Pregunta6/registros-institucionales-web
npm install
npm run dev
```

Abre `http://localhost:5173`. El archivo `.env` apunta a la API:

```
VITE_API_URL=http://localhost:5160
```

### Campos del formulario / payload

- `identificacionFiscal`
- `nombreOficial`
- `ipPublica`
- `enlaceTecnico`
- `correoResponsable`
- `documentoAutorizacion` (PDF ≤ 5 MB)
- `resolucionHabilitante` (PDF ≤ 5 MB)

Para el dígito verificador se usó módulo 11 (suposición documentada; el enunciado no define la regla institucional exacta).

---

## Pregunta 7 — Esquema de base de datos SQL Server

Script exclusivo en:

`Pregunta7/pregunta7.sql`

### Qué incluye (puntos 11–15)

| Punto  | Contenido                                                                                                                                     |
| ------ | --------------------------------------------------------------------------------------------------------------------------------------------- |
| **11** | Tabla `Registros`: `Identificador`, `Nombre` (búsqueda) + `Estado`, `NumeroRegistro`, `FechaEvento`, `FechaInscripcion` (salida del endpoint) |
| **12** | Tabla `Entidades`: `Activa`, `FechaInicioConvenio`, `FechaFinConvenio`, `CuotaDiaria` configurable por entidad, `ApiKey`                      |
| **13** | Tabla `LogAccesos`: `EntidadId`, `FechaHora`, `TipoConsulta`, `Resultado`, `Motivo`, `IdentificadorConsultado` (auditoría + cuota)            |
| **14** | Índice `IX_Registros_Identificador_Nombre` (búsqueda individual) e índice `IX_LogAccesos_Entidad_Fecha_Resultado` (conteo diario por entidad) |
| **15** | Trigger `TR_LogAccesos_ValidarCuotaDiaria`: impide insertar un acceso `APROBADO` si la entidad ya superó su `CuotaDiaria` del día             |

Las decisiones de diseño están comentadas dentro del mismo script SQL.

### Cómo ejecutarlo (opcional)

Sobre SQL Server, con la base `SviDb` creada:

```bash
# Ejemplo con sqlcmd (ajusta usuario/contraseña según tu Docker)
sqlcmd -S localhost,1433 -U sa -P "password123" -d SviDb -i Pregunta7/pregunta7.sql
```

> Nota: la API ya genera un esquema equivalente vía migraciones EF Core.
