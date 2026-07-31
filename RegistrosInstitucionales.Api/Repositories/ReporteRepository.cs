using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.DTOs;

namespace RegistrosInstitucionales.Api.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly AppDbContext _context;

    public ReporteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HistorialAccesoDto>> ObtenerHistorialAccesosAsync(
        DateTime fechaInicio,
        DateTime fechaFinExclusiva,
        CancellationToken cancellationToken = default)
    {
        var consulta =
            from log in _context.LogsAcceso.AsNoTracking()
            join entidad in _context.Entidades.AsNoTracking()
                on log.EntidadId equals entidad.Id
            where log.FechaHora >= fechaInicio
                  && log.FechaHora < fechaFinExclusiva
                  && log.Resultado == "APROBADO"
            group log by new
            {
                entidad.Nombre,
                log.TipoConsulta,
                Fecha = log.FechaHora.Date
            }
            into grupo
            orderby grupo.LongCount() descending
            select new
            {
                NombreEntidad = grupo.Key.Nombre,
                TipoConsulta = grupo.Key.TipoConsulta,
                TotalConsultas = grupo.LongCount(),
                Fecha = grupo.Key.Fecha
            };

        var resultados = await consulta
            .Take(100)
            .ToListAsync(cancellationToken);

        return resultados
            .Select(resultado => new HistorialAccesoDto
            {
                NombreEntidad = resultado.NombreEntidad,
                TipoConsulta = resultado.TipoConsulta,
                TotalConsultas = resultado.TotalConsultas,
                Fecha = DateOnly.FromDateTime(resultado.Fecha)
            })
            .ToList();
    }
}