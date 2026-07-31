namespace RegistrosInstitucionales.Api.Models;

public class Entidad
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public bool Activa { get; set; }

    public DateTime FechaInicioConvenio { get; set; }

    public DateTime FechaFinConvenio { get; set; }

    public int CuotaDiaria { get; set; }

    /// <summary>Número de identificación fiscal (solo dígitos, incluye verificador).</summary>
    public string? IdentificacionFiscal { get; set; }

    public string? IpPublica { get; set; }

    public string? EnlaceTecnico { get; set; }

    public string? CorreoResponsable { get; set; }

    public string? DocumentoAutorizacionRuta { get; set; }

    public string? ResolucionHabilitanteRuta { get; set; }

    public ICollection<LogAcceso> LogsAcceso { get; set; } = new List<LogAcceso>();
}
