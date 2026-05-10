using Booking.Auditoria.DataAccess.Context;
using Booking.Auditoria.DataManagement.DTOs;
using Booking.Auditoria.DataManagement.Entities;
using Booking.Auditoria.DataManagement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking.Auditoria.DataAccess.Repositories;

/// <summary>
/// Implementación concreta de IAuditoriaLogRepository usando EF Core + PostgreSQL.
/// </summary>
public class AuditoriaLogRepository : IAuditoriaLogRepository
{
    private readonly AuditoriaDbContext _db;

    public AuditoriaLogRepository(AuditoriaDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<long> InsertAsync(LogAuditoria log, CancellationToken ct = default)
    {
        _db.LogsAuditoria.Add(log);
        await _db.SaveChangesAsync(ct);
        return log.IdLog;
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<AuditoriaLogDto>> QueryAsync(AuditoriaLogQueryDto query, CancellationToken ct = default)
    {
        var q = _db.LogsAuditoria
                   .Where(l => !l.EsEliminadoLog)
                   .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.TablaAfectada))
            q = q.Where(l => l.TablaAfectada == query.TablaAfectada);

        if (!string.IsNullOrWhiteSpace(query.Operacion))
            q = q.Where(l => l.Operacion == query.Operacion.ToUpperInvariant());

        if (!string.IsNullOrWhiteSpace(query.CreadoPorUsuario))
            q = q.Where(l => l.CreadoPorUsuario == query.CreadoPorUsuario);

        if (!string.IsNullOrWhiteSpace(query.ServicioOrigen))
            q = q.Where(l => l.ServicioOrigen == query.ServicioOrigen);

        if (query.FechaDesde.HasValue)
            q = q.Where(l => l.FechaUtc >= query.FechaDesde.Value);

        if (query.FechaHasta.HasValue)
            q = q.Where(l => l.FechaUtc <= query.FechaHasta.Value);

        var total = await q.CountAsync(ct);

        var tamano = Math.Clamp(query.TamanoPagina, 1, 200);
        var pagina = Math.Max(query.Pagina, 1);

        var items = await q
            .OrderByDescending(l => l.FechaUtc)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .Select(l => MapToDto(l))
            .ToListAsync(ct);

        return new PagedResultDto<AuditoriaLogDto>
        {
            Items = items,
            TotalRegistros = total,
            Pagina = pagina,
            TamanoPagina = tamano
        };
    }

    /// <inheritdoc />
    public async Task<AuditoriaLogDto?> GetByIdAsync(long idLog, CancellationToken ct = default)
    {
        var log = await _db.LogsAuditoria
                           .FirstOrDefaultAsync(l => l.IdLog == idLog && !l.EsEliminadoLog, ct);

        return log is null ? null : MapToDto(log);
    }

    /// <inheritdoc />
    public async Task<bool> SoftDeleteAsync(long idLog, CancellationToken ct = default)
    {
        var log = await _db.LogsAuditoria
                           .FirstOrDefaultAsync(l => l.IdLog == idLog && !l.EsEliminadoLog, ct);

        if (log is null) return false;

        log.EsEliminadoLog = true;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ─────────────────────────────────────────────
    // Helpers privados
    // ─────────────────────────────────────────────

    private static AuditoriaLogDto MapToDto(LogAuditoria l) => new()
    {
        IdLog            = l.IdLog,
        TablaAfectada    = l.TablaAfectada,
        EsquemaAfectado  = l.EsquemaAfectado,
        Operacion        = l.Operacion,
        IdRegistro       = l.IdRegistro,
        DatosAnteriores  = l.DatosAnteriores is not null
                               ? System.Text.Json.JsonDocument.Parse(l.DatosAnteriores).RootElement
                               : null,
        DatosNuevos      = l.DatosNuevos is not null
                               ? System.Text.Json.JsonDocument.Parse(l.DatosNuevos).RootElement
                               : null,
        CreadoPorUsuario = l.CreadoPorUsuario,
        FechaUtc         = l.FechaUtc,
        Ip               = l.Ip,
        ServicioOrigen   = l.ServicioOrigen,
        EquipoOrigen     = l.EquipoOrigen
    };
}
