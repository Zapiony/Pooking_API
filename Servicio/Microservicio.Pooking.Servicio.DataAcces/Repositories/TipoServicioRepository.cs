using Microservicio.Pooking.Servicio.DataAcces.Common;
using Microservicio.Pooking.Servicio.DataAcces.Entities;
using Microservicio.Pooking.Servicio.DataAcces.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Servicio.DataAcces.Repositories;

/// <summary>
/// Nunca llama SaveChanges directamente.
/// Esa responsabilidad pertenece al UnitOfWork de la capa superior.
/// </summary>
public class TipoServicioRepository : ITipoServicioRepository
{
    private readonly DbContext _context;

    public TipoServicioRepository(DbContext context) => _context = context;

    private IQueryable<TipoServicioEntity> QueryVigentes =>
        _context.Set<TipoServicioEntity>().Where(ts => !ts.EsEliminado);

    public async Task<TipoServicioEntity?> ObtenerPorIdAsync(int idTipoServicio, CancellationToken ct = default)
        => await QueryVigentes.FirstOrDefaultAsync(ts => ts.IdTipoServicio == idTipoServicio, ct);

    public async Task<TipoServicioEntity?> ObtenerPorGuidAsync(Guid guidTipoServicio, CancellationToken ct = default)
        => await QueryVigentes.FirstOrDefaultAsync(ts => ts.GuidTipoServicio == guidTipoServicio, ct);

    public async Task<TipoServicioEntity?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default)
        => await QueryVigentes.FirstOrDefaultAsync(ts => ts.Nombre == nombre, ct);

    public async Task<IReadOnlyList<TipoServicioEntity>> ObtenerTodosActivosAsync(CancellationToken ct = default)
        => await QueryVigentes
            .Where(ts => ts.Estado == "ACT")
            .OrderBy(ts => ts.Nombre)
            .ToListAsync(ct);

    public async Task<PagedResult<TipoServicioEntity>> ObtenerTodosPaginadoAsync(
        int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        var query = QueryVigentes.OrderBy(ts => ts.Nombre);
        var total = await query.CountAsync(ct);

        if (total == 0)
            return PagedResult<TipoServicioEntity>.Vacio(paginaActual, tamanoPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return new PagedResult<TipoServicioEntity>(items, total, paginaActual, tamanoPagina);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, CancellationToken ct = default)
        => await _context.Set<TipoServicioEntity>().AnyAsync(ts => ts.Nombre == nombre, ct);

    public async Task AgregarAsync(TipoServicioEntity tipoServicio, CancellationToken ct = default)
        => await _context.Set<TipoServicioEntity>().AddAsync(tipoServicio, ct);

    public void Actualizar(TipoServicioEntity tipoServicio)
        => _context.Set<TipoServicioEntity>().Update(tipoServicio);

    public void EliminarLogico(TipoServicioEntity tipoServicio)
    {
        tipoServicio.EsEliminado = true;
        tipoServicio.Estado = "INA";
        _context.Set<TipoServicioEntity>().Update(tipoServicio);
    }
}
