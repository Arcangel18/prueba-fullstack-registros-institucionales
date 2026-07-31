using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Entidades.AnyAsync())
        {
            context.Entidades.Add(new Entidad
            {
                Nombre = "Entidad de prueba",
                ApiKey = "API-KEY-PRUEBA-123",
                Activa = true,
                FechaInicioConvenio = DateTime.UtcNow.AddMonths(-1),
                FechaFinConvenio = DateTime.UtcNow.AddYears(1),
                CuotaDiaria = 5
            });
        }

        if (!await context.Registros.AnyAsync())
        {
            context.Registros.Add(new Registro
            {
                Identificador = "8-123-456",
                Nombre = "Juan Pérez",
                Estado = "ACTIVO",
                NumeroRegistro = "REG-2026-0001",
                FechaEvento = new DateTime(2026, 1, 15),
                FechaInscripcion = new DateTime(2026, 1, 20)
            });
        }

        await context.SaveChangesAsync();
    }
}