using Microsoft.AspNetCore.Mvc;
using RegistrosInstitucionales.Api.DTOs;
using RegistrosInstitucionales.Api.Services;

namespace RegistrosInstitucionales.Api.Controllers;

[ApiController]
[Route("api/entidades")]
public class EntidadesController : ControllerBase
{
    private readonly IEntidadRegistroService _entidadRegistroService;
    private readonly ILogger<EntidadesController> _logger;

    public EntidadesController(
        IEntidadRegistroService entidadRegistroService,
        ILogger<EntidadesController> logger)
    {
        _entidadRegistroService = entidadRegistroService;
        _logger = logger;
    }

    [HttpPost]
    [RequestSizeLimit(12 * 1024 * 1024)]
    [ProducesResponseType(typeof(RegistrarEntidadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarEntidadResponse>> Registrar(
        [FromForm] RegistrarEntidadRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _entidadRegistroService.RegistrarAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(Registrar),
                new { id = resultado.Id },
                resultado);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ErrorResponse
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = exception.Message
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error al registrar la entidad.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Ocurrió un error interno al registrar la entidad."
                });
        }
    }
}
