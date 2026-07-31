namespace RegistrosInstitucionales.Api.Models;

public class LogAcceso
{
    public long Id { get; set; }

    public int? EntidadId { get; set; }

    public Entidad? Entidad { get; set; }

    public DateTime FechaHora { get; set; }

    public string TipoConsulta { get; set; } = "CONSULTA_REGISTRO";

    public string Resultado { get; set; } = string.Empty;

    public string Motivo { get; set; } = string.Empty;

    public string? IdentificadorConsultado { get; set; }
}