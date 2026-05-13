namespace Booking.Auditoria.DataManagement.DTOs;

/// <summary>
/// DTO de entrada para registrar un evento de auditoría desde el Middleware.
/// Generado cuando llega un evento como BookingCreated, CustomerRegistered, etc.
/// </summary>
public class CreateAuditoriaLogDto
{
    /// <summary>Nombre de la tabla afectada. Ej: 'cliente', 'reservas', 'servicio'.</summary>
    public string TablaAfectada { get; set; } = string.Empty;

    /// <summary>Esquema afectado. Siempre 'booking' en este sistema.</summary>
    public string EsquemaAfectado { get; set; } = "booking";

    /// <summary>INSERT | UPDATE | DELETE</summary>
    public string Operacion { get; set; } = string.Empty;

    /// <summary>GUID o ID del registro afectado extraído del payload del evento.</summary>
    public string? IdRegistro { get; set; }

    /// <summary>JSONB con el estado anterior (UPDATE/DELETE). Puede ser null en INSERT.</summary>
    public string? DatosAnteriores { get; set; }

    /// <summary>JSONB con el nuevo estado (INSERT/UPDATE). Puede ser null en DELETE.</summary>
    public string? DatosNuevos { get; set; }

    /// <summary>Username extraído de los claims del JWT propagado.</summary>
    public string? CreadoPorUsuario { get; set; }

    /// <summary>IP del request original propagada vía correlationId.</summary>
    public string? Ip { get; set; }

    /// <summary>Nombre del microservicio productor del evento.</summary>
    public string? ServicioOrigen { get; set; }

    /// <summary>Hostname del servidor del servicio productor.</summary>
    public string? EquipoOrigen { get; set; }
}
