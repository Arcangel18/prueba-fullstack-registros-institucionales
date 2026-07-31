namespace RegistrosInstitucionales.Api.DTOs;

public class RegistrarEntidadResponse
{
    public int Id { get; set; }

    public string NombreOficial { get; set; } = string.Empty;

    public string IdentificacionFiscal { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;
}
