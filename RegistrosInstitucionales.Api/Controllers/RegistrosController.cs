using Microsoft.AspNetCore.Mvc;
using RegistrosInstitucionales.Api.DTOs;
using RegistrosInstitucionales.Api.Exceptions;
using RegistrosInstitucionales.Api.Services;

namespace RegistrosInstitucionales.Api.Controllers;

[ApiController]
[Route("api/registros")]
public class RegistrosController : ControllerBase
{
    private readonly IRegistroConsultaService _registroConsultaService;
    private readonly ILogger<RegistrosController> _logger;

    public RegistrosController(
        IRegistroConsultaService registroConsultaService,
        ILogger<RegistrosController> logger)
    {
        _registroConsultaService = registroConsultaService;
        _logger = logger;
    }

    [HttpPost("consulta")]
    [ProducesResponseType(typeof(ConsultaRegistroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConsultaRegistroResponse>> Consultar(
        [FromHeader(Name = "X-API-Key")] string? apiKey,
        [FromBody] ConsultaRegistroRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // La ausencia o invalidez de la API Key se valida y audita en el servicio
            // para cumplir el requisito de registrar todo intento (aprobado o rechazado).
            var resultado = await _registroConsultaService.ConsultarAsync(
                apiKey ?? string.Empty,
                request,
                cancellationToken);

            return Ok(resultado);
        }
        catch (ApiKeyInvalidaException exception)
        {
            return Unauthorized(new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = exception.Message
            });
        }
        catch (AccesoDenegadoException exception)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new ErrorResponse
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = exception.Message
                });
        }
        catch (RegistroNoEncontradoException exception)
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = exception.Message
            });
        }
        catch (CuotaExcedidaException exception)
        {
            return StatusCode(
                StatusCodes.Status429TooManyRequests,
                new ErrorResponse
                {
                    StatusCode = StatusCodes.Status429TooManyRequests,
                    Message = exception.Message
                });
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error inesperado durante la consulta de registros.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Ocurrió un error interno al procesar la solicitud."
                });
        }
    }
}