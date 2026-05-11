using Microservicio.Pooking.Servicio.Business.DTOs;
using Microservicio.Pooking.Servicio.Business.DTOs.TipoServicio;

namespace Microservicio.Pooking.Servicio.Business.Interfaces;

public interface ITipoServicioService
{
    Task<TipoServicioResponse?> ObtenerPorGuidAsync(Guid guidTipoServicio, CancellationToken ct = default);
    Task<TipoServicioResponse?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default);
    Task<IReadOnlyList<TipoServicioResponse>> ListarActivosAsync(CancellationToken ct = default);
    Task<PagedResultado<TipoServicioResponse>> ListarPaginadoAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<TipoServicioResponse> CrearAsync(CrearTipoServicioRequest request, CancellationToken ct = default);
    Task<TipoServicioResponse> ActualizarAsync(ActualizarTipoServicioRequest request, CancellationToken ct = default);
    Task EliminarAsync(Guid guidTipoServicio, CancellationToken ct = default);
}
