namespace Microservicio.Pooking.Servicio.DataManagement.Models;

public sealed class ServicioDataModel
{
    public int IdServicio { get; set; }
    public Guid GuidServicio { get; set; }
    public int IdTipoServicio { get; set; }
    public Guid GuidTipoServicio { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string CorreoContacto { get; set; } = string.Empty;
    public string? TelefonoContacto { get; set; }
    public string? Direccion { get; set; }
    public string? SitioWeb { get; set; }
    public string? LogoUrl { get; set; }
    public string Estado { get; set; } = "ACT";
    public bool EsEliminado { get; set; }
    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaRegistroUtc { get; set; }
    public string? ModificadoPorUsuario { get; set; }
    public DateTimeOffset? FechaModificacionUtc { get; set; }
    public string? ModificacionIp { get; set; }
    public string? ServicioOrigen { get; set; }
    public byte[] RowVersion { get; set; } = [];
    /// <summary>Solo lectura — disponible en listados/detalle enriquecido.</summary>
    public string? TipoServicioNombre { get; set; }
}

/// <summary>Vista resumida para listados (proyección CQRS).</summary>
public sealed record ServicioResumenDataModel(
    Guid GuidServicio,
    string RazonSocial,
    string? NombreComercial,
    string TipoServicioNombre,
    string Estado);
