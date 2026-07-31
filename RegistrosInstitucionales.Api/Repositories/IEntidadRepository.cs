using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Repositories;

public interface IEntidadRepository
{
    Task<Entidad?> ObtenerPorApiKeyAsync(
        string apiKey,
        CancellationToken cancellationToken = default);

    Task<Entidad?> ObtenerPorIdentificacionFiscalAsync(
        string identificacionFiscal,
        CancellationToken cancellationToken = default);

    Task CrearAsync(
        Entidad entidad,
        CancellationToken cancellationToken = default);
}
