using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Repositories;

public interface ILogAccesoRepository
{
    Task<int> ContarConsultasAprobadasHoyAsync(
        int entidadId,
        CancellationToken cancellationToken = default);

    Task RegistrarAsync(
        LogAcceso logAcceso,
        CancellationToken cancellationToken = default);
}