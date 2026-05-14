using Microservicio.Pooking.Servicio.DataAccess.Common;
using Microservicio.Pooking.Servicio.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Servicio.DataAccess.Queries;

/// <summary>
/// Consultas de solo lectura para el dominio de servicios.
/// AsNoTracking en todas las consultas para máximo rendimiento.
/// Usa proyecciones (Select) para no cargar entidades completas.
/// </summary>
public class ServicioQueryRepository : IServicioQueryRepository
{
    private readonly DbContext _context;

    public ServicioQueryRepository(DbContext context) => _context = context;

    // Estado INA queda oculto igual que un borrado logico; solo ACT es visible en lecturas publicas.
    private IQueryable<ServicioEntity> QueryVigentes =>
        _context.Set<ServicioEntity>()
                .AsNoTracking()
                .Where(s => !s.EsEliminado && s.Estado == "ACT");

    public async Task<PagedResult<ServicioResumenDto>> ListarServiciosAsync(
        int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        var query = QueryVigentes.OrderBy(s => s.RazonSocial);
        var total = await query.CountAsync(ct);

        if (total == 0)
            return PagedResult<ServicioResumenDto>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(s => new ServicioResumenDto(
                s.GuidServicio,
                s.RazonSocial,
                s.NombreComercial,
                s.TipoServicio.Nombre,
                s.Estado))
            .ToListAsync(ct);

        return new PagedResult<ServicioResumenDto>(items, total, paginaActual, tamanoPagina);
    }

    public async Task<ServicioDetalleDto?> ObtenerDetalleAsync(Guid guidServicio, CancellationToken ct = default)
        => await QueryVigentes
            .Where(s => s.GuidServicio == guidServicio)
            .Select(s => new ServicioDetalleDto(
                s.GuidServicio,
                s.RazonSocial,
                s.NombreComercial,
                s.TipoIdentificacion,
                s.NumeroIdentificacion,
                s.CorreoContacto,
                s.TelefonoContacto,
                s.Direccion,
                s.SitioWeb,
                s.LogoUrl,
                s.Estado,
                s.EsEliminado,
                s.TipoServicio.GuidTipoServicio,
                s.TipoServicio.Nombre,
                s.CreadoPorUsuario,
                s.FechaRegistroUtc,
                s.ModificadoPorUsuario,
                s.FechaModificacionUtc))
            .FirstOrDefaultAsync(ct);

    public async Task<PagedResult<ServicioResumenDto>> BuscarServiciosAsync(
        string termino, int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        var patron = $"%{termino.Trim()}%";
        var query = QueryVigentes
            .Where(s =>
                EF.Functions.ILike(s.RazonSocial, patron) ||
                (s.NombreComercial != null && EF.Functions.ILike(s.NombreComercial, patron)))
            .OrderBy(s => s.RazonSocial);

        var total = await query.CountAsync(ct);

        if (total == 0)
            return PagedResult<ServicioResumenDto>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(s => new ServicioResumenDto(
                s.GuidServicio,
                s.RazonSocial,
                s.NombreComercial,
                s.TipoServicio.Nombre,
                s.Estado))
            .ToListAsync(ct);

        return new PagedResult<ServicioResumenDto>(items, total, paginaActual, tamanoPagina);
    }

    public async Task<IReadOnlyList<ServicioResumenDto>> ListarServiciosPorTipoAsync(
        Guid tipoServicioGuid, CancellationToken ct = default)
        => await QueryVigentes
            .Where(s => s.TipoServicio.GuidTipoServicio == tipoServicioGuid && s.Estado == "ACT")
            .OrderBy(s => s.RazonSocial)
            .Select(s => new ServicioResumenDto(
                s.GuidServicio,
                s.RazonSocial,
                s.NombreComercial,
                s.TipoServicio.Nombre,
                s.Estado))
            .ToListAsync(ct);
}
