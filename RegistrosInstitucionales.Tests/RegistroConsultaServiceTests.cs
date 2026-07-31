using Moq;
using RegistrosInstitucionales.Api.DTOs;
using RegistrosInstitucionales.Api.Exceptions;
using RegistrosInstitucionales.Api.Models;
using RegistrosInstitucionales.Api.Repositories;
using RegistrosInstitucionales.Api.Services;

namespace RegistrosInstitucionales.Tests;

public class RegistroConsultaServiceTests
{
    [Fact]
    public async Task ConsultarAsync_CuandoAlcanzaCuotaDiaria_LanzaCuotaExcedidaException()
    {
        // Arrange
        var entidadRepositoryMock = new Mock<IEntidadRepository>();
        var registroRepositoryMock = new Mock<IRegistroRepository>();
        var logRepositoryMock = new Mock<ILogAccesoRepository>();

        var entidad = new Entidad
        {
            Id = 1,
            Nombre = "Entidad de prueba",
            ApiKey = "API-KEY-PRUEBA",
            Activa = true,
            FechaInicioConvenio = DateTime.UtcNow.AddDays(-10),
            FechaFinConvenio = DateTime.UtcNow.AddDays(10),
            CuotaDiaria = 5
        };

        entidadRepositoryMock
            .Setup(repository => repository.ObtenerPorApiKeyAsync(
                "API-KEY-PRUEBA",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entidad);

        logRepositoryMock
            .Setup(repository => repository.ContarConsultasAprobadasHoyAsync(
                entidad.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        logRepositoryMock
            .Setup(repository => repository.RegistrarAsync(
                It.IsAny<LogAcceso>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RegistroConsultaService(
            entidadRepositoryMock.Object,
            registroRepositoryMock.Object,
            logRepositoryMock.Object);

        var request = new ConsultaRegistroRequest
        {
            Identificador = "8-123-456",
            Nombre = "Juan Pérez"
        };

        // Act
        var action = async () =>
            await service.ConsultarAsync(
                "API-KEY-PRUEBA",
                request);

        // Assert
        await Assert.ThrowsAsync<CuotaExcedidaException>(action);

        registroRepositoryMock.Verify(
            repository => repository.BuscarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}