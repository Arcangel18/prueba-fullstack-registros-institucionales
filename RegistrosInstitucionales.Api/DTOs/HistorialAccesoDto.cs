namespace RegistrosInstitucionales.Api.DTOs;

public class HistorialAccesoDto
{
    public string NombreEntidad { get; set; } = string.Empty;

    public string TipoConsulta { get; set; } = string.Empty;

    public long TotalConsultas { get; set; }

    public DateOnly Fecha { get; set; }

    public string FechaFormato => Fecha.ToString("dd/MM/yyyy");
}