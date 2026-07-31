using RegistrosInstitucionales.Api.DTOs;
using RegistrosInstitucionales.Api.Exceptions;
using RegistrosInstitucionales.Api.Models;
using RegistrosInstitucionales.Api.Repositories;

namespace RegistrosInstitucionales.Api.Services;

public class RegistroConsultaService : IRegistroConsultaService
{
    private readonly IEntidadRepository _entidadRepository;
    private readonly IRegistroRepository _registroRepository;
    private readonly ILogAccesoRepository _logAccesoRepository;

    public RegistroConsultaService(
        IEntidadRepository entidadRepository,
        IRegistroRepository registroRepository,
        ILogAccesoRepository logAccesoRepository)
    {
        _entidadRepository = entidadRepository;
        _registroRepository = registroRepository;
        _logAccesoRepository = logAccesoRepository;
    }

    public async Task<ConsultaRegistroResponse> ConsultarAsync(
        string apiKey,
        ConsultaRegistroRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            await RegistrarIntentoAsync(
                null,
                request.Identificador,
                "RECHAZADO",
                "API_KEY_AUSENTE",
                cancellationToken);

            throw new ApiKeyInvalidaException(
                "Debe proporcionar la API Key en el header X-API-Key.");
        }

        var entidad = await _entidadRepository.ObtenerPorApiKeyAsync(
            apiKey.Trim(),
            cancellationToken);

        if (entidad is null)
        {
            await RegistrarIntentoAsync(
                null,
                request.Identificador,
                "RECHAZADO",
                "API_KEY_INVALIDA",
                cancellationToken);

            throw new ApiKeyInvalidaException("La API Key no es válida.");
        }

        if (!entidad.Activa)
        {
            await RegistrarIntentoAsync(
                entidad.Id,
                request.Identificador,
                "RECHAZADO",
                "ENTIDAD_INACTIVA",
                cancellationToken);

            throw new AccesoDenegadoException(
                "La entidad se encuentra inactiva.");
        }

        var fechaActual = DateTime.UtcNow;

        if (fechaActual < entidad.FechaInicioConvenio ||
            fechaActual > entidad.FechaFinConvenio)
        {
            await RegistrarIntentoAsync(
                entidad.Id,
                request.Identificador,
                "RECHAZADO",
                "CONVENIO_NO_VIGENTE",
                cancellationToken);

            throw new AccesoDenegadoException(
                "El convenio de la entidad no se encuentra vigente.");
        }

        var consultasRealizadas =
            await _logAccesoRepository.ContarConsultasAprobadasHoyAsync(
                entidad.Id,
                cancellationToken);

        if (consultasRealizadas >= entidad.CuotaDiaria)
        {
            await RegistrarIntentoAsync(
                entidad.Id,
                request.Identificador,
                "RECHAZADO",
                "CUOTA_DIARIA_EXCEDIDA",
                cancellationToken);

            throw new CuotaExcedidaException(
                "La entidad ha alcanzado su cuota diaria de consultas.");
        }

        var registro = await _registroRepository.BuscarAsync(
            request.Identificador.Trim(),
            request.Nombre.Trim(),
            cancellationToken);

        if (registro is null)
        {
            await RegistrarIntentoAsync(
                entidad.Id,
                request.Identificador,
                "RECHAZADO",
                "REGISTRO_NO_ENCONTRADO",
                cancellationToken);

            throw new RegistroNoEncontradoException(
                "No se encontró un registro con los datos proporcionados.");
        }

        await RegistrarIntentoAsync(
            entidad.Id,
            request.Identificador,
            "APROBADO",
            "CONSULTA_EXITOSA",
            cancellationToken);

        return new ConsultaRegistroResponse
        {
            Estado = registro.Estado,
            NumeroRegistro = registro.NumeroRegistro,
            FechaEvento = registro.FechaEvento,
            FechaInscripcion = registro.FechaInscripcion
        };
    }

    private async Task RegistrarIntentoAsync(
        int? entidadId,
        string? identificador,
        string resultado,
        string motivo,
        CancellationToken cancellationToken)
    {
        var log = new LogAcceso
        {
            EntidadId = entidadId,
            FechaHora = DateTime.UtcNow,
            TipoConsulta = "CONSULTA_REGISTRO",
            Resultado = resultado,
            Motivo = motivo,
            IdentificadorConsultado = identificador
        };

        await _logAccesoRepository.RegistrarAsync(
            log,
            cancellationToken);
    }
}