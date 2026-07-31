using System.ComponentModel.DataAnnotations;

namespace RegistrosInstitucionales.Api.DTOs;

public class ConsultaRegistroRequest
{
    [Required(ErrorMessage = "El identificador es obligatorio.")]
    [NoWhitespace(ErrorMessage = "El identificador es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El identificador no puede superar 50 caracteres.")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [NoWhitespace(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>
/// Rechaza cadenas vacías o solo espacios (DataAnnotations [Required] no lo hace por sí solo).
/// </summary>
public class NoWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string texto && string.IsNullOrWhiteSpace(texto))
        {
            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} no puede estar vacío.");
        }

        return ValidationResult.Success;
    }
}
