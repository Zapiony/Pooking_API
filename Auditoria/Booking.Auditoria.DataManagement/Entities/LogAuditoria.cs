using System.Text.Json.Serialization;

namespace Booking.Auditoria.DataManagement.Entities;

/// <summary>
/// Espejo exacto de booking.log_auditoria (04_auditoria.sql).
/// Sin FK físicas hacia ningún otro dominio — desacoplamiento total.
/// </summary>
public class LogAuditoria
{
    // [1] Identificación técnica
    public long IdLog { get; set; }

    // [2] Datos funcionales
    public string TablaAfectada { get; set; } = string.Empty;
    public string EsquemaAfectado { get; set; } = "booking";

    /// <summary>INSERT | UPDATE | DELETE</summary>
    public string Operacion { get; set; } = string.Empty;

    /// <summary>GUID o ID del registro afectado (TEXT en BD).</summary>
    public string? IdRegistro { get; set; }

    /// <summary>Estado completo de la fila antes de la operación (UPDATE/DELETE). JSONB.</summary>
    public string? DatosAnteriores { get; set; }

    /// <summary>Estado completo de la fila después de la operación (INSERT/UPDATE). JSONB.</summary>
    public string? DatosNuevos { get; set; }

    // [3] Trazabilidad del actor
    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? Ip { get; set; }
    public string? ServicioOrigen { get; set; }
    public string? EquipoOrigen { get; set; }

    // [4] Ciclo de vida del propio log
    public bool EsEliminadoLog { get; set; } = false;
}
