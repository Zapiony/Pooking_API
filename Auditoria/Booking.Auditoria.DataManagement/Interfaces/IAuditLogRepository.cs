using Booking.Auditoria.DataManagement.DTOs;
using Booking.Auditoria.DataManagement.Entities;

namespace Booking.Auditoria.DataManagement.Interfaces;

/// <summary>
/// Contrato del repositorio de log_auditoria.
/// Implementado en DataAccess; consumido por la API directamente (no hay capa Business
/// ya que Audit es un servicio receptor de eventos — la lógica reside en el Middleware).
/// </summary>
public interface IAuditoriaLogRepository
{
    /// <summary>
    /// Inserta un nuevo registro en log_auditoria.
    /// Llamado desde el AuditoriaController al recibir eventos del Middleware.
    /// </summary>
    Task<long> InsertAsync(LogAuditoria log, CancellationToken ct = default);

    /// <summary>
    /// Consulta paginada con filtros opcionales.
    /// Solo disponible para ADMINISTRADOR.
    /// </summary>
    Task<PagedResultDto<AuditoriaLogDto>> QueryAsync(AuditoriaLogQueryDto query, CancellationToken ct = default);

    /// <summary>
    /// Obtiene un registro específico por id_log.
    /// </summary>
    Task<AuditoriaLogDto?> GetByIdAsync(long idLog, CancellationToken ct = default);

    /// <summary>
    /// Borrado lógico de un registro de log (es_eliminado_log = true).
    /// Solo ADMINISTRADOR.
    /// </summary>
    Task<bool> SoftDeleteAsync(long idLog, CancellationToken ct = default);
}
