using RegistrosInstitucionales.Api.DTOs;

namespace RegistrosInstitucionales.Api.Services;

public interface IRegistroConsultaService
{
    Task<ConsultaRegistroResponse> ConsultarAsync(
        string apiKey,
        ConsultaRegistroRequest request,
        CancellationToken cancellationToken = default);
}