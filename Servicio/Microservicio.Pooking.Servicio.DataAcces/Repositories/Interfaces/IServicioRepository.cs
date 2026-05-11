using Microservicio.Pooking.Servicio.DataAcces.Common;
using Microservicio.Pooking.Servicio.DataAcces.Entities;

namespace Microservicio.Pooking.Servicio.DataAcces.Repositories.Interfaces;

public interface IServicioRepository
{
    Task<ServicioEntity?> ObtenerPorIdAsync(int idServicio, CancellationToken ct = default);
    Task<ServicioEntity?> ObtenerPorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<ServicioEntity?> ObtenerConTipoServicioPorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<IReadOnlyList<ServicioEntity>> ObtenerPorTipoServicioAsync(int idTipoServicio, CancellationToken ct = default);
    Task<PagedResult<ServicioEntity>> ObtenerTodosPaginadoAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<PagedResult<ServicioEntity>> BuscarPorRazonSocialAsync(string termino, int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<bool> ExisteIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken ct = default);
    Task AgregarAsync(ServicioEntity servicio, CancellationToken ct = default);
    void Actualizar(ServicioEntity servicio);
    void EliminarLogico(ServicioEntity servicio);
}
