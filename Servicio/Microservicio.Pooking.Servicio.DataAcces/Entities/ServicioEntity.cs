namespace Microservicio.Pooking.Servicio.DataAcces.Entities;

/// <summary>
/// Representa un proveedor o servicio externo integrable en la plataforma Pooking.
/// Cada instancia está vinculada a un tipo de servicio (Vuelos, Alojamiento, etc.)
/// </summary>
public class ServicioEntity
{
    // ── Identificación ────────────────────────────────────────────────────
    /// <summary>PK interna. No se expone en la API.</summary>
    public int IdServicio { get; set; }

    /// <summary>Identificador público expuesto en la API REST.</summary>
    public Guid GuidServicio { get; set; }

    // ── Relación con tipo ─────────────────────────────────────────────────
    /// <summary>FK al tipo de servicio que clasifica este proveedor.</summary>
    public int IdTipoServicio { get; set; }

    // ── Datos del proveedor ───────────────────────────────────────────────
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }

    /// <summary>RUC | CI | PASS | EXT.</summary>
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;

    public string CorreoContacto { get; set; } = string.Empty;
    public string? TelefonoContacto { get; set; }
    public string? Direccion { get; set; }
    public string? SitioWeb { get; set; }
    public string? LogoUrl { get; set; }

    // ── Estado y ciclo de vida ────────────────────────────────────────────
    /// <summary>ACT = Activo | INA = Inactivo | SUS = Suspendido.</summary>
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
    public TipoServicioEntity TipoServicio { get; set; } = null!;
}
