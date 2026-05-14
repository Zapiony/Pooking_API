using Microservicio.Pooking.Servicio.DataAccess.Common;

namespace Microservicio.Pooking.Servicio.DataAccess.Queries;

// ── DTOs de proyección ────────────────────────────────────────────────────

/// <summary>Vista resumida para listados (sin cargar toda la entidad).</summary>
public sealed record ServicioResumenDto(
    Guid GuidServicio,
    string RazonSocial,
    string? NombreComercial,
    string TipoServicioNombre,
    string Estado
);

/// <summary>Vista detallada para administración.</summary>
public sealed record ServicioDetalleDto(
    Guid GuidServicio,
    string RazonSocial,
    string? NombreComercial,
    string TipoIdentificacion,
    string NumeroIdentificacion,
    string CorreoContacto,
    string? TelefonoContacto,
    string? Direccion,
    string? SitioWeb,
    string? LogoUrl,
    string Estado,
    bool EsEliminado,
    Guid GuidTipoServicio,
    string TipoServicioNombre,
    string? CreadoPorUsuario,
    DateTimeOffset FechaRegistroUtc,
    string? ModificadoPorUsuario,
    DateTimeOffset? FechaModificacionUtc
);

// ── Contrato de consultas ─────────────────────────────────────────────────

public interface IServicioQueryRepository
{
    Task<PagedResult<ServicioResumenDto>> ListarServiciosAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<ServicioDetalleDto?> ObtenerDetalleAsync(Guid guidServicio, CancellationToken ct = default);
    Task<PagedResult<ServicioResumenDto>> BuscarServiciosAsync(string termino, int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<IReadOnlyList<ServicioResumenDto>> ListarServiciosPorTipoAsync(Guid tipoServicioGuid, CancellationToken ct = default);
}
