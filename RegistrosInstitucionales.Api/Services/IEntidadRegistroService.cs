using RegistrosInstitucionales.Api.DTOs;

namespace RegistrosInstitucionales.Api.Services;

public interface IEntidadRegistroService
{
    Task<RegistrarEntidadResponse> RegistrarAsync(
        RegistrarEntidadRequest request,
        CancellationToken cancellationToken = default);
}
