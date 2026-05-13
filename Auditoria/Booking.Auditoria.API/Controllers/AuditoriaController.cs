using Booking.Auditoria.DataManagement.DTOs;
using Booking.Auditoria.DataManagement.Entities;
using Booking.Auditoria.DataManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Auditoria.API.Controllers;

/// <summary>
/// Endpoints REST del servicio de Auditoría.
/// POST /api/auditoria-logs  → recibe eventos del Middleware (sin JWT, red interna)
/// GET  /api/auditoria-logs  → consulta paginada (solo ADMINISTRADOR)
/// GET  /api/auditoria-logs/{id} → detalle (solo ADMINISTRADOR)
/// DELETE /api/auditoria-logs/{id} → borrado lógico (solo ADMINISTRADOR)
/// </summary>
[ApiController]
[Route("api/auditoria-logs")]
public class AuditoriaController : ControllerBase
{
    private readonly IAuditoriaLogRepository _repo;
    private readonly ILogger<AuditoriaController> _logger;

    public AuditoriaController(IAuditoriaLogRepository repo, ILogger<AuditoriaController> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/auditoria-logs
    // Recibe eventos del Middleware FIFO. No requiere JWT
    // (el Middleware es red interna; proteger con header secreto si se expone).
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Registra un evento de auditoría proveniente del Middleware.
    /// </summary>
    /// <response code="201">Registro creado exitosamente.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAuditoriaLogDto dto,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.TablaAfectada))
            return BadRequest(new { error = "tabla_afectada es requerida." });

        if (!new[] { "INSERT", "UPDATE", "DELETE" }.Contains(dto.Operacion?.ToUpperInvariant()))
            return BadRequest(new { error = "operacion debe ser INSERT, UPDATE o DELETE." });

        var log = new LogAuditoria
        {
            TablaAfectada    = dto.TablaAfectada.Trim(),
            EsquemaAfectado  = string.IsNullOrWhiteSpace(dto.EsquemaAfectado) ? "booking" : dto.EsquemaAfectado.Trim(),
            Operacion        = dto.Operacion!.ToUpperInvariant(),
            IdRegistro       = dto.IdRegistro,
            DatosAnteriores  = dto.DatosAnteriores,
            DatosNuevos      = dto.DatosNuevos,
            CreadoPorUsuario = dto.CreadoPorUsuario,
            Ip               = dto.Ip,
            ServicioOrigen   = dto.ServicioOrigen,
            EquipoOrigen     = dto.EquipoOrigen,
            FechaUtc         = DateTimeOffset.UtcNow
        };

        var id = await _repo.InsertAsync(log, ct);

        _logger.LogInformation(
            "[AuditoriaREST] Registro creado: id_log={Id} tabla={Tabla} op={Op}",
            id, log.TablaAfectada, log.Operacion);

        return CreatedAtAction(nameof(GetById), new { id }, new { id_log = id });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET /api/auditoria-logs
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Consulta paginada de registros de auditoría. Solo ADMINISTRADOR.
    /// </summary>
    /// <response code="200">Lista paginada.</response>
    [HttpGet]
    [Authorize(Policy = "SoloAdmin")]
    [ProducesResponseType(typeof(PagedResultDto<AuditoriaLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] AuditoriaLogQueryDto query,
        CancellationToken ct)
    {
        var result = await _repo.QueryAsync(query, ct);
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET /api/auditoria-logs/{id}
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Obtiene un registro de auditoría por su id. Solo ADMINISTRADOR.
    /// </summary>
    /// <response code="200">Registro encontrado.</response>
    /// <response code="404">No encontrado.</response>
    [HttpGet("{id:long}")]
    [Authorize(Policy = "SoloAdmin")]
    [ProducesResponseType(typeof(AuditoriaLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var dto = await _repo.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DELETE /api/auditoria-logs/{id}  (borrado lógico)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Borrado lógico de un registro de log (es_eliminado_log = true). Solo ADMINISTRADOR.
    /// </summary>
    /// <response code="204">Borrado exitoso.</response>
    /// <response code="404">No encontrado.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "SoloAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDelete(long id, CancellationToken ct)
    {
        var deleted = await _repo.SoftDeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
