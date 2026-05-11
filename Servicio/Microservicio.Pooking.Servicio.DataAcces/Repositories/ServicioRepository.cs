using Microservicio.Pooking.Servicio.DataAcces.Common;
using Microservicio.Pooking.Servicio.DataAcces.Entities;
using Microservicio.Pooking.Servicio.DataAcces.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Servicio.DataAcces.Repositories;

/// <summary>
/// Nunca llama SaveChanges directamente.
/// Esa responsabilidad pertenece al UnitOfWork de la capa superior.
/// </summary>
public class ServicioRepository : IServicioRepository
{
    private readonly DbContext _context;

    public ServicioRepository(DbContext context) => _context = context;

    private IQueryable<ServicioEntity> QueryVigentes =>
        _context.Set<ServicioEntity>().Where(s => !s.EsEliminado);

    public async Task<ServicioEntity?> ObtenerPorIdAsync(int idServicio, CancellationToken ct = default)
        => await QueryVigentes.FirstOrDefaultAsync(s => s.IdServicio == idServicio, ct);

    public async Task<ServicioEntity?> ObtenerPorGuidAsync(Guid guidServicio, CancellationToken ct = default)
        => await QueryVigentes.FirstOrDefaultAsync(s => s.GuidServicio == guidServicio, ct);

    public async Task<ServicioEntity?> ObtenerConTipoServicioPorGuidAsync(Guid guidServicio, CancellationToken ct = default)
        => await QueryVigentes
            .Include(s => s.TipoServicio)
            .FirstOrDefaultAsync(s => s.GuidServicio == guidServicio, ct);

    public async Task<IReadOnlyList<ServicioEntity>> ObtenerPorTipoServicioAsync(int idTipoServicio, CancellationToken ct = default)
        => await QueryVigentes
            .Where(s => s.IdTipoServicio == idTipoServicio && s.Estado == "ACT")
            .OrderBy(s => s.RazonSocial)
            .ToListAsync(ct);

    public async Task<PagedResult<ServicioEntity>> ObtenerTodosPaginadoAsync(
        int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        var query = QueryVigentes.OrderBy(s => s.RazonSocial);
        var total = await query.CountAsync(ct);

        if (total == 0)
            return PagedResult<ServicioEntity>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return new PagedResult<ServicioEntity>(items, total, paginaActual, tamanoPagina);
    }

    public async Task<PagedResult<ServicioEntity>> BuscarPorRazonSocialAsync(
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
            return PagedResult<ServicioEntity>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return new PagedResult<ServicioEntity>(items, total, paginaActual, tamanoPagina);
    }

    public async Task<bool> ExisteIdentificacionAsync(
        string tipoIdentificacion, string numeroIdentificacion, CancellationToken ct = default)
        => await _context.Set<ServicioEntity>()
            .AnyAsync(s =>
                s.TipoIdentificacion == tipoIdentificacion &&
                s.NumeroIdentificacion == numeroIdentificacion, ct);

    public async Task AgregarAsync(ServicioEntity servicio, CancellationToken ct = default)
        => await _context.Set<ServicioEntity>().AddAsync(servicio, ct);

    public void Actualizar(ServicioEntity servicio)
        => _context.Set<ServicioEntity>().Update(servicio);

    public void EliminarLogico(ServicioEntity servicio)
    {
        servicio.EsEliminado = true;
        servicio.Estado = "INA";
        _context.Set<ServicioEntity>().Update(servicio);
    }
}
