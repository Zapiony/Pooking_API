namespace Microservicio.Pooking.Servicio.DataAcces.Entities;

/// <summary>
/// Catálogo cerrado de categorías de servicio integrable en la plataforma Pooking:
/// Vuelos | Alojamiento | Atracciones | Alquiler de Carros.
/// </summary>
public class TipoServicioEntity
{
    // ── Identificación ────────────────────────────────────────────────────
    /// <summary>PK interna. No se expone en la API.</summary>
    public int IdTipoServicio { get; set; }

    /// <summary>Identificador público expuesto en la API REST.</summary>
    public Guid GuidTipoServicio { get; set; }

    // ── Datos funcionales ─────────────────────────────────────────────────
    /// <summary>Vuelos | Alojamiento | Atracciones | Alquiler de Carros.</summary>
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    // ── Estado y ciclo de vida ────────────────────────────────────────────
    /// <summary>ACT = Activo | INA = Inactivo.</summary>
    public string Estado { get; set; } = "ACT";

    /// <summary>Borrado lógico. false = vigente, true = eliminado.</summary>
    public bool EsEliminado { get; set; } = false;

    // ── Auditoría ─────────────────────────────────────────────────────────
    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaRegistroUtc { get; set; }
    public string? ModificadoPorUsuario { get; set; }
    public DateTimeOffset? FechaModificacionUtc { get; set; }
    public string? ModificacionIp { get; set; }
    public string? ServicioOrigen { get; set; }

    // ── Navegación ────────────────────────────────────────────────────────
    public ICollection<ServicioEntity> Servicios { get; set; } = [];
}
