namespace Booking.Auditoria.DataManagement.DTOs;

/// <summary>
/// Parámetros de filtrado y paginación para consultas de logs de auditoría.
/// Usado en GET /api/auditoria-logs (solo ADMINISTRADOR).
/// </summary>
public class AuditoriaLogQueryDto
{
    /// <summary>Filtra por nombre de tabla. Ej: 'cliente', 'reservas'.</summary>
    public string? TablaAfectada { get; set; }

    /// <summary>Filtra por tipo de operación: INSERT, UPDATE, DELETE.</summary>
    public string? Operacion { get; set; }

    /// <summary>Filtra por usuario que ejecutó la operación.</summary>
    public string? CreadoPorUsuario { get; set; }

    /// <summary>Filtra por servicio de origen.</summary>
    public string? ServicioOrigen { get; set; }

    /// <summary>Fecha de inicio del rango (UTC).</summary>
    public DateTimeOffset? FechaDesde { get; set; }

    /// <summary>Fecha de fin del rango (UTC).</summary>
    public DateTimeOffset? FechaHasta { get; set; }

    /// <summary>Número de página (1-indexed). Por defecto 1.</summary>
    public int Pagina { get; set; } = 1;

    /// <summary>Registros por página. Por defecto 50, máximo 200.</summary>
    public int TamanoPagina { get; set; } = 50;
}
