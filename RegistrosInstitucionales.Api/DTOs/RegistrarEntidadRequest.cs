using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RegistrosInstitucionales.Api.DTOs;

public class RegistrarEntidadRequest
{
    [Required(ErrorMessage = "La identificación fiscal es obligatoria.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "La identificación fiscal solo debe contener dígitos.")]
    [MinLength(2, ErrorMessage = "La identificación fiscal es demasiado corta.")]
    [MaxLength(20, ErrorMessage = "La identificación fiscal no puede superar 20 caracteres.")]
    public string IdentificacionFiscal { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre oficial es obligatorio.")]
    [MinLength(3, ErrorMessage = "El nombre oficial es demasiado corto.")]
    [MaxLength(200, ErrorMessage = "El nombre oficial no puede superar 200 caracteres.")]
    public string NombreOficial { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección IP pública es obligatoria.")]
    [RegularExpression(
        @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?)$",
        ErrorMessage = "La dirección IP no tiene un formato IPv4 válido.")]
    public string IpPublica { get; set; } = string.Empty;

    [Required(ErrorMessage = "El enlace técnico es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El enlace técnico no puede superar 100 caracteres.")]
    public string EnlaceTecnico { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo del responsable es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [MaxLength(100, ErrorMessage = "El correo no puede superar 100 caracteres.")]
    public string CorreoResponsable { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe adjuntar el documento de autorización institucional.")]
    public IFormFile? DocumentoAutorizacion { get; set; }

    [Required(ErrorMessage = "Debe adjuntar la resolución o acto administrativo habilitante.")]
    public IFormFile? ResolucionHabilitante { get; set; }
}
