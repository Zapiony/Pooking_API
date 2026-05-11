using Microservicio.Pooking.Servicio.DataAcces.Entities;
using Microservicio.Pooking.Servicio.DataAcces.Queries;
using Microservicio.Pooking.Servicio.DataManagement.Interfaces;
using Microservicio.Pooking.Servicio.DataManagement.Mappers;
using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.DataManagement.Services;

public sealed class ServicioDataService : IServicioDataService
{
    private readonly IUnitOfWork _uow;

    public ServicioDataService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServicioDataModel?> ObtenerPorIdAsync(int idServicio, CancellationToken ct = default)
    {
        var e = await _uow.ServicioRepository.ObtenerPorIdAsync(idServicio, ct);
        if (e is null) return null;
        var tipo = await _uow.TipoServicioRepository.ObtenerPorIdAsync(e.IdTipoServicio, ct);
        return ServicioDataMapper.AModelo(e, tipo);
    }

    public async Task<ServicioDataModel?> ObtenerPorGuidAsync(Guid guidServicio, CancellationToken ct = default)
    {
        var e = await _uow.ServicioRepository.ObtenerPorGuidAsync(guidServicio, ct);
        if (e is null) return null;
        var tipo = await _uow.TipoServicioRepository.ObtenerPorIdAsync(e.IdTipoServicio, ct);
        return ServicioDataMapper.AModelo(e, tipo);
    }

    public async Task<ServicioDataModel?> ObtenerConTipoPorGuidAsync(Guid guidServicio, CancellationToken ct = default)
    {
        var e = await _uow.ServicioRepository.ObtenerConTipoServicioPorGuidAsync(guidServicio, ct);
        return e is null ? null : ServicioDataMapper.AModelo(e);
    }

    public async Task<ServicioDataModel?> ObtenerDetallePorGuidAsync(Guid guidServicio, CancellationToken ct = default)
    {
        var dto = await _uow.ServicioQueryRepository.ObtenerDetalleAsync(guidServicio, ct);
        return dto is null ? null : ServicioDataMapper.AModeloDesdeDetalle(dto);
    }

    public async Task<DataPagedResult<ServicioResumenDataModel>> ListarOBuscarAsync(
        ServicioFiltroDataModel filtro, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(filtro);
        ArgumentOutOfRangeException.ThrowIfLessThan(filtro.PaginaActual, 1, nameof(filtro.PaginaActual));
        ArgumentOutOfRangeException.ThrowIfLessThan(filtro.TamanoPagina, 1, nameof(filtro.TamanoPagina));

        var tieneTermino = !string.IsNullOrWhiteSpace(filtro.Termino);
        var tieneTipo = filtro.GuidTipoServicio.HasValue;

        if (!tieneTermino && !tieneTipo)
        {
            var r = await _uow.ServicioQueryRepository.ListarServiciosAsync(filtro.PaginaActual, filtro.TamanoPagina, ct);
            return DataPagedResult<ServicioResumenDataModel>.DesdeDal(r, ServicioDataMapper.AResumen);
        }

        if (tieneTermino && !tieneTipo)
        {
            var r = await _uow.ServicioQueryRepository.BuscarServiciosAsync(filtro.Termino!.Trim(), filtro.PaginaActual, filtro.TamanoPagina, ct);
            return DataPagedResult<ServicioResumenDataModel>.DesdeDal(r, ServicioDataMapper.AResumen);
        }

        var listaTipo = await _uow.ServicioQueryRepository.ListarServiciosPorTipoAsync(filtro.GuidTipoServicio!.Value, ct);

        if (!tieneTermino)
            return PaginarEnMemoria(listaTipo, filtro.PaginaActual, filtro.TamanoPagina);

        var term = filtro.Termino!.Trim();
        var filtrados = listaTipo
            .Where(s =>
                s.RazonSocial.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (s.NombreComercial?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        return PaginarEnMemoria(filtrados, filtro.PaginaActual, filtro.TamanoPagina);
    }

    public async Task<DataPagedResult<ServicioDataModel>> ListarEntidadesPaginadoAsync(
        int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        var p = await _uow.ServicioRepository.ObtenerTodosPaginadoAsync(paginaActual, tamanoPagina, ct);
        var modelos = new List<ServicioDataModel>(p.Items.Count);
        foreach (var e in p.Items)
        {
            var tipo = await _uow.TipoServicioRepository.ObtenerPorIdAsync(e.IdTipoServicio, ct);
            modelos.Add(ServicioDataMapper.AModelo(e, tipo));
        }
        return new DataPagedResult<ServicioDataModel>(modelos, p.TotalRegistros, p.PaginaActual, p.TamanoPagina);
    }

    public async Task<ServicioDataModel> CrearAsync(ServicioDataModel modelo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(modelo);

        var tipo = await ResolverTipoAsync(modelo, ct)
            ?? throw new InvalidOperationException("No se encontró el tipo de servicio indicado.");

        if (await _uow.ServicioRepository.ExisteIdentificacionAsync(modelo.TipoIdentificacion, modelo.NumeroIdentificacion, ct))
            throw new InvalidOperationException("Ya existe un servicio con la misma identificación.");

        var entidad = ServicioDataMapper.ANuevaEntidad(modelo, tipo.IdTipoServicio);
        await _uow.ServicioRepository.AgregarAsync(entidad, ct);
        await _uow.SaveChangesAsync(ct);

        return ServicioDataMapper.AModelo(entidad, tipo);
    }

    public async Task<ServicioDataModel> ActualizarAsync(ServicioDataModel modelo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(modelo);

        var entidad = await _uow.ServicioRepository.ObtenerConTipoServicioPorGuidAsync(modelo.GuidServicio, ct)
            ?? throw new InvalidOperationException("No se encontró el servicio a actualizar.");

        var tipoDestino = await ResolverTipoAsync(modelo, ct);
        if (tipoDestino is not null)
            modelo.IdTipoServicio = tipoDestino.IdTipoServicio;

        ServicioDataMapper.AplicarCambios(entidad, modelo);
        _uow.ServicioRepository.Actualizar(entidad);
        await _uow.SaveChangesAsync(ct);

        var tipo = await _uow.TipoServicioRepository.ObtenerPorIdAsync(entidad.IdTipoServicio, ct);
        return ServicioDataMapper.AModelo(entidad, tipo);
    }

    public async Task EliminarLogicoAsync(Guid guidServicio, CancellationToken ct = default)
    {
        var entidad = await _uow.ServicioRepository.ObtenerPorGuidAsync(guidServicio, ct)
            ?? throw new InvalidOperationException("No se encontró el servicio a eliminar.");

        _uow.ServicioRepository.EliminarLogico(entidad);
        await _uow.SaveChangesAsync(ct);
    }

    private async Task<TipoServicioEntity?> ResolverTipoAsync(ServicioDataModel modelo, CancellationToken ct)
    {
        if (modelo.GuidTipoServicio != Guid.Empty)
            return await _uow.TipoServicioRepository.ObtenerPorGuidAsync(modelo.GuidTipoServicio, ct);
        if (modelo.IdTipoServicio > 0)
            return await _uow.TipoServicioRepository.ObtenerPorIdAsync(modelo.IdTipoServicio, ct);
        return null;
    }

    private static DataPagedResult<ServicioResumenDataModel> PaginarEnMemoria(
        IReadOnlyList<ServicioResumenDto> items, int paginaActual, int tamanoPagina)
    {
        var slice = items
            .Skip((paginaActual - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(ServicioDataMapper.AResumen)
            .ToList();
        return new DataPagedResult<ServicioResumenDataModel>(slice, items.Count, paginaActual, tamanoPagina);
    }
}
