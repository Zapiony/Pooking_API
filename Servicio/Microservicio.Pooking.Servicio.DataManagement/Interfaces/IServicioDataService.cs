using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.DataManagement.Interfaces;

public interface IServicioDataService
{
    Task<ServicioDataModel?> ObtenerPorIdAsync(int idServicio, CancellationToken ct = default);
    Task<ServicioDataModel?> ObtenerPorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<ServicioDataModel?> ObtenerConTipoPorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<ServicioDataModel?> ObtenerDetallePorGuidAsync(Guid guidServicio, CancellationToken ct = default);
    Task<DataPagedResult<ServicioResumenDataModel>> ListarOBuscarAsync(ServicioFiltroDataModel filtro, CancellationToken ct = default);
    Task<DataPagedResult<ServicioDataModel>> ListarEntidadesPaginadoAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<ServicioDataModel> CrearAsync(ServicioDataModel modelo, CancellationToken ct = default);
    Task<ServicioDataModel> ActualizarAsync(ServicioDataModel modelo, CancellationToken ct = default);
    Task EliminarLogicoAsync(Guid guidServicio, CancellationToken ct = default);
}
