using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.DataManagement.Interfaces;

public interface ITipoServicioDataService
{
    Task<TipoServicioDataModel?> ObtenerPorIdAsync(int idTipoServicio, CancellationToken ct = default);
    Task<TipoServicioDataModel?> ObtenerPorGuidAsync(Guid guidTipoServicio, CancellationToken ct = default);
    Task<TipoServicioDataModel?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default);
    Task<IReadOnlyList<TipoServicioDataModel>> ListarActivosAsync(CancellationToken ct = default);
    Task<DataPagedResult<TipoServicioDataModel>> ListarPaginadoAsync(int paginaActual, int tamanoPagina, CancellationToken ct = default);
    Task<TipoServicioDataModel> CrearAsync(TipoServicioDataModel modelo, CancellationToken ct = default);
    Task<TipoServicioDataModel> ActualizarAsync(TipoServicioDataModel modelo, CancellationToken ct = default);
    Task EliminarLogicoAsync(Guid guidTipoServicio, CancellationToken ct = default);
}
