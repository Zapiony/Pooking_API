using Microservicio.Pooking.Servicio.DataAccess.Common;
using Microservicio.Pooking.Servicio.DataAccess.Entities;

namespace Microservicio.Pooking.Servicio.DataAccess.Repositories.Interfaces;

public interface IServicioRepository
{
    Task<ServicioEntity?> ObtenerPorIdAsync(int idServicio, CancellationToken ct = default);
    Task<ServicioEntity?> ObtenerPorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<ServicioEntity?> ObtenerConTipoServicioPorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<IReadOnlyList<ServicioEntity>> ObtenerPorTipoServicioAsync(int idTipoServicio, CancellationToken ct = default);
    Task<PagedResult<ServicioEntity>> ObtenerTodosPaginadoAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<PagedResult<ServicioEntity>> BuscarPorRazonSocialAsync(string termino, int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<bool> ExisteIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken ct = default);
    Task<bool> TieneServiciosAsociadosAsync(Guid guidTipoServicio, CancellationToken ct = default);
    Task AgregarAsync(ServicioEntity servicio, CancellationToken ct = default);
    void Actualizar(ServicioEntity servicio);
    void EliminarLogico(ServicioEntity servicio);
}
