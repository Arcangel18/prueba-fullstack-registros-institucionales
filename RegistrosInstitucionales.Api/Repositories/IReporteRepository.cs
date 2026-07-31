using RegistrosInstitucionales.Api.DTOs;

namespace RegistrosInstitucionales.Api.Repositories;

public interface IReporteRepository
{
    Task<List<HistorialAccesoDto>> ObtenerHistorialAccesosAsync(
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        CancellationToken cancellationToken = default);
}