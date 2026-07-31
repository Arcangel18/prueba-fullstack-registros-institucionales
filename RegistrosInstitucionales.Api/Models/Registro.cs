namespace RegistrosInstitucionales.Api.Models;

public class Registro
{
    public int Id { get; set; }

    public string Identificador { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public string NumeroRegistro { get; set; } = string.Empty;

    public DateTime FechaEvento { get; set; }

    public DateTime FechaInscripcion { get; set; }
}