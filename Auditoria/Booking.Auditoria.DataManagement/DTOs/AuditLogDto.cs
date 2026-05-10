namespace Booking.Auditoria.DataManagement.DTOs;

/// <summary>
/// DTO de respuesta para un registro de auditoría.
/// Usado en GET /api/auditoria-logs.
/// </summary>
public class AuditoriaLogDto
{
    public long IdLog { get; set; }
    public string TablaAfectada { get; set; } = string.Empty;
    public string EsquemaAfectado { get; set; } = "booking";
    public string Operacion { get; set; } = string.Empty;
    public string? IdRegistro { get; set; }
    public object? DatosAnteriores { get; set; }
    public object? DatosNuevos { get; set; }
    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaUtc { get; set; }
    public string? Ip { get; set; }
    public string? ServicioOrigen { get; set; }
    public string? EquipoOrigen { get; set; }
}
