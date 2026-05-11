using Microservicio.Pooking.Servicio.Business.DTOs;
using Microservicio.Pooking.Servicio.Business.DTOs.Servicio;

namespace Microservicio.Pooking.Servicio.Business.Interfaces;

public interface IServicioService
{
    Task<ServicioResponse?> ObtenerPorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<ServicioResponse?> ObtenerDetallePorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<PagedResultado<ServicioResumenResponse>> ListarOBuscarAsync(ServicioFiltroRequest filtro, CancellationToken ct = default);
    Task<PagedResultado<ServicioResponse>> ListarEntidadesPaginadoAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<ServicioResponse> CrearAsync(CrearServicioRequest request, CancellationToken ct = default);
    Task<ServicioResponse> ActualizarAsync(ActualizarServicioRequest request, CancellationToken ct = default);
    Task EliminarAsync(Guid guidServicio, CancellationToken ct = default);
}
