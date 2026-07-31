using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Repositories;

public class EntidadRepository : IEntidadRepository
{
    private readonly AppDbContext _context;

    public EntidadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Entidad?> ObtenerPorApiKeyAsync(
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        return await _context.Entidades
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entidad => entidad.ApiKey == apiKey,
                cancellationToken);
    }

    public async Task<Entidad?> ObtenerPorIdentificacionFiscalAsync(
        string identificacionFiscal,
        CancellationToken cancellationToken = default)
    {
        return await _context.Entidades
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entidad => entidad.IdentificacionFiscal == identificacionFiscal,
                cancellationToken);
    }

    public async Task CrearAsync(
        Entidad entidad,
        CancellationToken cancellationToken = default)
    {
        _context.Entidades.Add(entidad);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
