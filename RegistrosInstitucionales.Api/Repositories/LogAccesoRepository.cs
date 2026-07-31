using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Repositories;

public class LogAccesoRepository : ILogAccesoRepository
{
    private readonly AppDbContext _context;

    public LogAccesoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> ContarConsultasAprobadasHoyAsync(
        int entidadId,
        CancellationToken cancellationToken = default)
    {
        var inicioDia = DateTime.UtcNow.Date;
        var finDia = inicioDia.AddDays(1);

        return await _context.LogsAcceso.CountAsync(
            log =>
                log.EntidadId == entidadId &&
                log.Resultado == "APROBADO" &&
                log.FechaHora >= inicioDia &&
                log.FechaHora < finDia,
            cancellationToken);
    }

    public async Task RegistrarAsync(
        LogAcceso logAcceso,
        CancellationToken cancellationToken = default)
    {
        _context.LogsAcceso.Add(logAcceso);

        await _context.SaveChangesAsync(cancellationToken);
    }
}