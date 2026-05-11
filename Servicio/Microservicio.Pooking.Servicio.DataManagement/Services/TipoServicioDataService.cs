using Microservicio.Pooking.Servicio.DataManagement.Interfaces;
using Microservicio.Pooking.Servicio.DataManagement.Mappers;
using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.DataManagement.Services;

public sealed class TipoServicioDataService : ITipoServicioDataService
{
    private readonly IUnitOfWork _uow;

    public TipoServicioDataService(IUnitOfWork uow) => _uow = uow;

    public async Task<TipoServicioDataModel?> ObtenerPorIdAsync(int idTipoServicio, CancellationToken ct = default)
    {
        var e = await _uow.TipoServicioRepository.ObtenerPorIdAsync(idTipoServicio, ct);
        return e is null ? null : TipoServicioDataMapper.AModelo(e);
    }

    public async Task<TipoServicioDataModel?> ObtenerPorGuidAsync(Guid guidTipoServicio, CancellationToken ct = default)
    {
        var e = await _uow.TipoServicioRepository.ObtenerPorGuidAsync(guidTipoServicio, ct);
        return e is null ? null : TipoServicioDataMapper.AModelo(e);
    }

    public async Task<TipoServicioDataModel?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default)
    {
        var e = await _uow.TipoServicioRepository.ObtenerPorNombreAsync(nombre, ct);
        return e is null ? null : TipoServicioDataMapper.AModelo(e);
    }

    public async Task<IReadOnlyList<TipoServicioDataModel>> ListarActivosAsync(CancellationToken ct = default)
    {
        var items = await _uow.TipoServicioRepository.ObtenerTodosActivosAsync(ct);
        return items.Select(TipoServicioDataMapper.AModelo).ToList();
    }

    public async Task<DataPagedResult<TipoServicioDataModel>> ListarPaginadoAsync(
        int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        var p = await _uow.TipoServicioRepository.ObtenerTodosPaginadoAsync(paginaActual, tamanoPagina, ct);
        return DataPagedResult<TipoServicioDataModel>.DesdeDal(p, TipoServicioDataMapper.AModelo);
    }

    public async Task<TipoServicioDataModel> CrearAsync(TipoServicioDataModel modelo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(modelo);

        if (await _uow.TipoServicioRepository.ExisteNombreAsync(modelo.Nombre, ct))
            throw new InvalidOperationException("Ya existe un tipo de servicio con el mismo nombre.");

        var entidad = TipoServicioDataMapper.ANuevaEntidad(modelo);
        await _uow.TipoServicioRepository.AgregarAsync(entidad, ct);
        await _uow.SaveChangesAsync(ct);
        return TipoServicioDataMapper.AModelo(entidad);
    }

    public async Task<TipoServicioDataModel> ActualizarAsync(TipoServicioDataModel modelo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(modelo);

        var entidad = await _uow.TipoServicioRepository.ObtenerPorGuidAsync(modelo.GuidTipoServicio, ct)
            ?? throw new InvalidOperationException("No se encontró el tipo de servicio a actualizar.");

        TipoServicioDataMapper.AplicarCambios(entidad, modelo);
        _uow.TipoServicioRepository.Actualizar(entidad);
        await _uow.SaveChangesAsync(ct);
        return TipoServicioDataMapper.AModelo(entidad);
    }

    public async Task EliminarLogicoAsync(Guid guidTipoServicio, CancellationToken ct = default)
    {
        var entidad = await _uow.TipoServicioRepository.ObtenerPorGuidAsync(guidTipoServicio, ct)
            ?? throw new InvalidOperationException("No se encontró el tipo de servicio a eliminar.");

        _uow.TipoServicioRepository.EliminarLogico(entidad);
        await _uow.SaveChangesAsync(ct);
    }
}
