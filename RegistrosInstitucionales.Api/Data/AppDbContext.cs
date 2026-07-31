using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Models;

namespace RegistrosInstitucionales.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Entidad> Entidades => Set<Entidad>();

    public DbSet<Registro> Registros => Set<Registro>();

    public DbSet<LogAcceso> LogsAcceso => Set<LogAcceso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entidad>(entity =>
        {
            entity.ToTable("Entidades");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nombre)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ApiKey)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.IdentificacionFiscal)
                .HasMaxLength(20);

            entity.Property(x => x.IpPublica)
                .HasMaxLength(45);

            entity.Property(x => x.EnlaceTecnico)
                .HasMaxLength(100);

            entity.Property(x => x.CorreoResponsable)
                .HasMaxLength(100);

            entity.Property(x => x.DocumentoAutorizacionRuta)
                .HasMaxLength(500);

            entity.Property(x => x.ResolucionHabilitanteRuta)
                .HasMaxLength(500);

            entity.HasIndex(x => x.ApiKey)
                .IsUnique();

            entity.HasIndex(x => x.IdentificacionFiscal)
                .IsUnique()
                .HasFilter("[IdentificacionFiscal] IS NOT NULL");
        });

        modelBuilder.Entity<Registro>(entity =>
        {
            entity.ToTable("Registros");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Identificador)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Estado)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.NumeroRegistro)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.Identificador,
                x.Nombre
            });
        });

        modelBuilder.Entity<LogAcceso>(entity =>
        {
            entity.ToTable("LogAccesos");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Resultado)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Motivo)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.TipoConsulta)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.EntidadId,
                x.FechaHora,
                x.Resultado
            });

            entity.HasOne(x => x.Entidad)
                .WithMany(x => x.LogsAcceso)
                .HasForeignKey(x => x.EntidadId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
