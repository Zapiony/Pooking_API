using Microservicio.Pooking.Servicio.Business.DTOs;
using Microservicio.Pooking.Servicio.Business.DTOs.Servicio;
using Microservicio.Pooking.Servicio.Business.Exceptions;
using Microservicio.Pooking.Servicio.Business.Interfaces;
using Microservicio.Pooking.Servicio.Business.Mappers;
using Microservicio.Pooking.Servicio.Business.Validators;
using Microservicio.Pooking.Servicio.DataManagement.Interfaces;

namespace Microservicio.Pooking.Servicio.Business.Services;

public sealed class ServicioService : IServicioService
{
    private readonly IServicioDataService _data;

    public ServicioService(IServicioDataService data) => _data = data;

    public async Task<ServicioResponse?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        if (guid == Guid.Empty) throw new ValidationException("GuidServicio no es válido.");
        var m = await _data.ObtenerPorGuidAsync(guid, ct);
        return m is null ? null : ServicioBusinessMapper.ARespuesta(m);
    }

    public async Task<ServicioResponse?> ObtenerDetallePorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        if (guid == Guid.Empty) throw new ValidationException("GuidServicio no es válido.");
        var m = await _data.ObtenerDetallePorGuidAsync(guid, ct);
        return m is null ? null : ServicioBusinessMapper.ARespuesta(m);
    }

    public async Task<PagedResultado<ServicioResumenResponse>> ListarOBuscarAsync(
        ServicioFiltroRequest filtro, CancellationToken ct = default)
    {
        ServicioValidator.ValidarFiltro(filtro);
        var filtroData = ServicioBusinessMapper.AFiltroData(filtro);
        var p = await _data.ListarOBuscarAsync(filtroData, ct);
        return ServicioBusinessMapper.APaginado(p, ServicioBusinessMapper.AResumenRespuesta);
    }

    public async Task<PagedResultado<ServicioResponse>> ListarEntidadesPaginadoAsync(
        int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        ServicioValidator.ValidarPaginacion(paginaActual, tamanoPagina);
        var p = await _data.ListarEntidadesPaginadoAsync(paginaActual, tamanoPagina, ct);
        return ServicioBusinessMapper.APaginado(p, ServicioBusinessMapper.ARespuesta);
    }

    public async Task<ServicioResponse> CrearAsync(CrearServicioRequest request, CancellationToken ct = default)
    {
        ServicioValidator.ValidarCrear(request);
        var modelo = ServicioBusinessMapper.ACrearDataModel(request);
        try
        {
            var creado = await _data.CrearAsync(modelo, ct);
            return ServicioBusinessMapper.ARespuesta(creado);
        }
        catch (InvalidOperationException ex) { DataServiceExceptionMapper.PropagarSiInvalidOperation(ex); throw; }
    }

    public async Task<ServicioResponse> ActualizarAsync(ActualizarServicioRequest request, CancellationToken ct = default)
    {
        ServicioValidator.ValidarActualizar(request);
        var existente = await _data.ObtenerConTipoPorGuidAsync(request.GuidServicio, ct)
            ?? throw new NotFoundException("No se encontró el servicio indicado.");
        ServicioBusinessMapper.AplicarActualizacion(request, existente);
        try
        {
            var actualizado = await _data.ActualizarAsync(existente, ct);
            return ServicioBusinessMapper.ARespuesta(actualizado);
        }
        catch (InvalidOperationException ex) { DataServiceExceptionMapper.PropagarSiInvalidOperation(ex); throw; }
    }

    public async Task EliminarAsync(Guid guid, CancellationToken ct = default)
    {
        if (guid == Guid.Empty) throw new ValidationException("GuidServicio no es válido.");
        try { await _data.EliminarLogicoAsync(guid, ct); }
        catch (InvalidOperationException ex) { DataServiceExceptionMapper.PropagarSiInvalidOperation(ex); throw; }
    }
}
