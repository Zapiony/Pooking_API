using Booking.Auditoria.API.Controllers;
using Booking.Auditoria.DataManagement.DTOs;
using Booking.Auditoria.DataManagement.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Booking.Auditoria.Tests.Unit;

/// <summary>
/// Tests unitarios del AuditoriaController usando Moq para el repositorio.
/// </summary>
public class AuditoriaControllerTests
{
    private readonly Mock<IAuditoriaLogRepository> _repoMock;
    private readonly AuditoriaController _controller;

    public AuditoriaControllerTests()
    {
        _repoMock  = new Mock<IAuditoriaLogRepository>();
        _controller = new AuditoriaController(_repoMock.Object, NullLogger<AuditoriaController>.Instance);
    }

    // ── POST ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_DatosValidos_DebeRetornar201()
    {
        _repoMock.Setup(r => r.InsertAsync(It.IsAny<DataManagement.Entities.LogAuditoria>(), default))
                 .ReturnsAsync(42L);

        var dto = new CreateAuditoriaLogDto
        {
            TablaAfectada = "cliente",
            Operacion     = "INSERT",
            IdRegistro    = Guid.NewGuid().ToString(),
            DatosNuevos   = "{\"nombres\":\"Juan\"}",
            ServicioOrigen = "Booking.Customer"
        };

        var result = await _controller.Create(dto, default);

        result.Should().BeOfType<CreatedAtActionResult>()
              .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Create_TablaVacia_DebeRetornar400()
    {
        var dto = new CreateAuditoriaLogDto { TablaAfectada = "", Operacion = "INSERT" };

        var result = await _controller.Create(dto, default);

        result.Should().BeOfType<BadRequestObjectResult>()
              .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Create_OperacionInvalida_DebeRetornar400()
    {
        var dto = new CreateAuditoriaLogDto { TablaAfectada = "cliente", Operacion = "SELECT" };

        var result = await _controller.Create(dto, default);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── GET by ID ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_RegistroExistente_DebeRetornar200()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1, default))
                 .ReturnsAsync(new AuditoriaLogDto { IdLog = 1, TablaAfectada = "rol", Operacion = "DELETE" });

        var result = await _controller.GetById(1, default);

        result.Should().BeOfType<OkObjectResult>()
              .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetById_RegistroNoExistente_DebeRetornar404()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((AuditoriaLogDto?)null);

        var result = await _controller.GetById(999, default);

        result.Should().BeOfType<NotFoundResult>();
    }

    // ── DELETE ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task SoftDelete_RegistroExistente_DebeRetornar204()
    {
        _repoMock.Setup(r => r.SoftDeleteAsync(1, default)).ReturnsAsync(true);

        var result = await _controller.SoftDelete(1, default);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task SoftDelete_RegistroNoExistente_DebeRetornar404()
    {
        _repoMock.Setup(r => r.SoftDeleteAsync(999, default)).ReturnsAsync(false);

        var result = await _controller.SoftDelete(999, default);

        result.Should().BeOfType<NotFoundResult>();
    }
}
