using Booking.Auditoria.DataAccess.Context;
using Booking.Auditoria.DataAccess.Repositories;
using Booking.Auditoria.DataManagement.DTOs;
using Booking.Auditoria.DataManagement.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Booking.Auditoria.Tests.Unit;

/// <summary>
/// Tests unitarios del repositorio AuditoriaLogRepository usando EF Core InMemory.
/// </summary>
public class AuditoriaLogRepositoryTests : IDisposable
{
    private readonly AuditoriaDbContext _db;
    private readonly AuditoriaLogRepository _repo;

    public AuditoriaLogRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AuditoriaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db   = new AuditoriaDbContext(options);
        _repo = new AuditoriaLogRepository(_db);
    }

    // ── InsertAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task InsertAsync_DebeRetornarIdPositivo()
    {
        // Arrange
        var log = BuildLog("cliente", "INSERT");

        // Act
        var id = await _repo.InsertAsync(log);

        // Assert
        id.Should().BePositive();
    }

    [Fact]
    public async Task InsertAsync_DebePersistirTodosLosCampos()
    {
        // Arrange
        var log = BuildLog("reservas", "UPDATE",
            datosAnteriores: "{\"estado\":\"PEND\"}",
            datosNuevos:     "{\"estado\":\"CONF\"}",
            usuario:         "admin",
            servicio:        "Booking.Booking");

        // Act
        var id = await _repo.InsertAsync(log);
        var guardado = await _db.LogsAuditoria.FindAsync(id);

        // Assert
        guardado.Should().NotBeNull();
        guardado!.TablaAfectada.Should().Be("reservas");
        guardado.Operacion.Should().Be("UPDATE");
        guardado.DatosAnteriores.Should().Contain("PEND");
        guardado.DatosNuevos.Should().Contain("CONF");
        guardado.CreadoPorUsuario.Should().Be("admin");
        guardado.ServicioOrigen.Should().Be("Booking.Booking");
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_RegistroExistente_DebeRetornarDto()
    {
        var log = BuildLog("servicio", "DELETE");
        var id  = await _repo.InsertAsync(log);

        var dto = await _repo.GetByIdAsync(id);

        dto.Should().NotBeNull();
        dto!.TablaAfectada.Should().Be("servicio");
        dto.Operacion.Should().Be("DELETE");
    }

    [Fact]
    public async Task GetByIdAsync_RegistroNoExistente_DebeRetornarNull()
    {
        var dto = await _repo.GetByIdAsync(99999);
        dto.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_RegistroEliminado_DebeRetornarNull()
    {
        var log = BuildLog("cliente", "INSERT");
        var id  = await _repo.InsertAsync(log);
        await _repo.SoftDeleteAsync(id);

        var dto = await _repo.GetByIdAsync(id);
        dto.Should().BeNull();
    }

    // ── SoftDeleteAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task SoftDeleteAsync_RegistroExistente_DebeRetornarTrue()
    {
        var log = BuildLog("rol", "INSERT");
        var id  = await _repo.InsertAsync(log);

        var result = await _repo.SoftDeleteAsync(id);

        result.Should().BeTrue();
        var guardado = await _db.LogsAuditoria.FindAsync(id);
        guardado!.EsEliminadoLog.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteAsync_RegistroNoExistente_DebeRetornarFalse()
    {
        var result = await _repo.SoftDeleteAsync(99999);
        result.Should().BeFalse();
    }

    // ── QueryAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task QueryAsync_FiltrarPorTabla_DebeRetornarSoloCorrespondientes()
    {
        await _repo.InsertAsync(BuildLog("cliente",  "INSERT"));
        await _repo.InsertAsync(BuildLog("reservas", "INSERT"));
        await _repo.InsertAsync(BuildLog("cliente",  "UPDATE"));

        var result = await _repo.QueryAsync(new AuditoriaLogQueryDto { TablaAfectada = "cliente" });

        result.Items.Should().HaveCount(2);
        result.TotalRegistros.Should().Be(2);
        result.Items.All(i => i.TablaAfectada == "cliente").Should().BeTrue();
    }

    [Fact]
    public async Task QueryAsync_Paginacion_DebeRespetarTamanoPagina()
    {
        for (int i = 0; i < 5; i++)
            await _repo.InsertAsync(BuildLog("usuario_app", "INSERT"));

        var result = await _repo.QueryAsync(new AuditoriaLogQueryDto { TamanoPagina = 2, Pagina = 1 });

        result.Items.Should().HaveCount(2);
        result.TotalPaginas.Should().Be(3);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static LogAuditoria BuildLog(
        string tabla,
        string operacion,
        string? datosAnteriores = null,
        string? datosNuevos     = null,
        string? usuario         = "test_user",
        string? servicio        = "Booking.Tests") => new()
    {
        TablaAfectada    = tabla,
        EsquemaAfectado  = "booking",
        Operacion        = operacion,
        IdRegistro       = Guid.NewGuid().ToString(),
        DatosAnteriores  = datosAnteriores,
        DatosNuevos      = datosNuevos,
        CreadoPorUsuario = usuario,
        ServicioOrigen   = servicio,
        EquipoOrigen     = "test-host",
        Ip               = "127.0.0.1",
        FechaUtc         = DateTimeOffset.UtcNow
    };

    public void Dispose() => _db.Dispose();
}
