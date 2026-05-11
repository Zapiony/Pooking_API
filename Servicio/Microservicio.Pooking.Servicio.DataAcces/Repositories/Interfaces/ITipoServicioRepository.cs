using Microservicio.Pooking.Servicio.DataAcces.Common;
using Microservicio.Pooking.Servicio.DataAcces.Entities;

namespace Microservicio.Pooking.Servicio.DataAcces.Repositories.Interfaces;

public interface ITipoServicioRepository
{
    Task<TipoServicioEntity?> ObtenerPorIdAsync(int idTipoServicio, CancellationToken ct = default);
    Task<TipoServicioEntity?> ObtenerPorGuidAsync(Guid guidTipoServicio, CancellationToken ct = default);
    Task<TipoServicioEntity?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default);
    Task<IReadOnlyList<TipoServicioEntity>> ObtenerTodosActivosAsync(CancellationToken ct = default);
    Task<PagedResult<TipoServicioEntity>> ObtenerTodosPaginadoAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(string nombre, CancellationToken ct = default);
    Task AgregarAsync(TipoServicioEntity tipoServicio, CancellationToken ct = default);
    void Actualizar(TipoServicioEntity tipoServicio);
    void EliminarLogico(TipoServicioEntity tipoServicio);
}
