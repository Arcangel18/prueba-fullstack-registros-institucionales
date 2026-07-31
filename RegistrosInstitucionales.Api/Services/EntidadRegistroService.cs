using RegistrosInstitucionales.Api.DTOs;
using RegistrosInstitucionales.Api.Models;
using RegistrosInstitucionales.Api.Repositories;

namespace RegistrosInstitucionales.Api.Services;

public class EntidadRegistroService : IEntidadRegistroService
{
    private const long MaxArchivoBytes = 5 * 1024 * 1024;
    private readonly IEntidadRepository _entidadRepository;
    private readonly IWebHostEnvironment _environment;

    public EntidadRegistroService(
        IEntidadRepository entidadRepository,
        IWebHostEnvironment environment)
    {
        _entidadRepository = entidadRepository;
        _environment = environment;
    }

    public async Task<RegistrarEntidadResponse> RegistrarAsync(
        RegistrarEntidadRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidarArchivo(request.DocumentoAutorizacion!, "Documento de autorización institucional");
        ValidarArchivo(request.ResolucionHabilitante!, "Resolución habilitante");

        var identificacion = request.IdentificacionFiscal.Trim();
        var existente = await _entidadRepository.ObtenerPorIdentificacionFiscalAsync(
            identificacion,
            cancellationToken);

        if (existente is not null)
        {
            throw new InvalidOperationException(
                "Ya existe una entidad registrada con esa identificación fiscal.");
        }

        var carpetaRelativa = Path.Combine("uploads", "entidades", Guid.NewGuid().ToString("N"));
        var carpetaAbsoluta = Path.Combine(_environment.ContentRootPath, carpetaRelativa);
        Directory.CreateDirectory(carpetaAbsoluta);

        var rutaAutorizacion = await GuardarPdfAsync(
            request.DocumentoAutorizacion!,
            carpetaAbsoluta,
            carpetaRelativa,
            "autorizacion",
            cancellationToken);

        var rutaResolucion = await GuardarPdfAsync(
            request.ResolucionHabilitante!,
            carpetaAbsoluta,
            carpetaRelativa,
            "resolucion",
            cancellationToken);

        var ahora = DateTime.UtcNow;
        var entidad = new Entidad
        {
            Nombre = request.NombreOficial.Trim(),
            IdentificacionFiscal = identificacion,
            IpPublica = request.IpPublica.Trim(),
            EnlaceTecnico = request.EnlaceTecnico.Trim(),
            CorreoResponsable = request.CorreoResponsable.Trim(),
            DocumentoAutorizacionRuta = rutaAutorizacion,
            ResolucionHabilitanteRuta = rutaResolucion,
            ApiKey = $"API-KEY-{Guid.NewGuid():N}".ToUpperInvariant(),
            Activa = true,
            FechaInicioConvenio = ahora,
            FechaFinConvenio = ahora.AddYears(1),
            CuotaDiaria = 100
        };

        await _entidadRepository.CrearAsync(entidad, cancellationToken);

        return new RegistrarEntidadResponse
        {
            Id = entidad.Id,
            NombreOficial = entidad.Nombre,
            IdentificacionFiscal = entidad.IdentificacionFiscal!,
            ApiKey = entidad.ApiKey,
            Mensaje = "La entidad se registró correctamente."
        };
    }

    private static void ValidarArchivo(IFormFile archivo, string etiqueta)
    {
        if (archivo.Length == 0)
        {
            throw new ArgumentException($"{etiqueta} está vacío.");
        }

        if (archivo.Length > MaxArchivoBytes)
        {
            throw new ArgumentException($"{etiqueta} no debe superar 5 MB.");
        }

        var esPdfPorTipo = string.Equals(
            archivo.ContentType,
            "application/pdf",
            StringComparison.OrdinalIgnoreCase);
        var esPdfPorNombre = archivo.FileName.EndsWith(
            ".pdf",
            StringComparison.OrdinalIgnoreCase);

        if (!esPdfPorTipo && !esPdfPorNombre)
        {
            throw new ArgumentException($"{etiqueta} debe ser un archivo PDF.");
        }
    }

    private static async Task<string> GuardarPdfAsync(
        IFormFile archivo,
        string carpetaAbsoluta,
        string carpetaRelativa,
        string prefijo,
        CancellationToken cancellationToken)
    {
        var nombreArchivo = $"{prefijo}.pdf";
        var rutaAbsoluta = Path.Combine(carpetaAbsoluta, nombreArchivo);

        await using var stream = File.Create(rutaAbsoluta);
        await archivo.CopyToAsync(stream, cancellationToken);

        return Path.Combine(carpetaRelativa, nombreArchivo).Replace('\\', '/');
    }
}
