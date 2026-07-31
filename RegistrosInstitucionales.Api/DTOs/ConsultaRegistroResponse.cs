namespace RegistrosInstitucionales.Api.DTOs;

public class ConsultaRegistroResponse
{
    public string Estado { get; set; } = string.Empty;

    public string NumeroRegistro { get; set; } = string.Empty;

    public DateTime FechaEvento { get; set; }

    public DateTime FechaInscripcion { get; set; }
}