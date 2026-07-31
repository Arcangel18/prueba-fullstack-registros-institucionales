using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Repositories;

public class RegistroRepository : IRegistroRepository
{
    private readonly AppDbContext _context;

    public RegistroRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Registro?> BuscarAsync(
        string identificador,
        string nombre,
        CancellationToken cancellationToken = default)
    {
        return await _context.Registros
            .AsNoTracking()
            .FirstOrDefaultAsync(
                registro =>
                    registro.Identificador == identificador &&
                    registro.Nombre == nombre,
                cancellationToken);
    }
}