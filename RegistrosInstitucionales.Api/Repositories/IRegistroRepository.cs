using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Repositories;

public interface IRegistroRepository
{
    Task<Registro?> BuscarAsync(
        string identificador,
        string nombre,
        CancellationToken cancellationToken = default);
}