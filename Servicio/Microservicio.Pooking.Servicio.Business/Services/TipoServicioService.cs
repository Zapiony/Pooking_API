using Microservicio.Pooking.Servicio.Business.DTOs;
using Microservicio.Pooking.Servicio.Business.DTOs.TipoServicio;
using Microservicio.Pooking.Servicio.Business.Exceptions;
using Microservicio.Pooking.Servicio.Business.Interfaces;
using Microservicio.Pooking.Servicio.Business.Mappers;
using Microservicio.Pooking.Servicio.Business.Validators;
using Microservicio.Pooking.Servicio.DataManagement.Interfaces;

namespace Microservicio.Pooking.Servicio.Business.Services;

public sealed class TipoServicioService : ITipoServicioService
{
    private readonly ITipoServicioDataService _data;

    public TipoServicioService(ITipoServicioDataService data) => _data = data;

    public async Task<TipoServicioResponse?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        if (guid == Guid.Empty) throw new ValidationException("GuidTipoServicio no es válido.");
        var m = await _data.ObtenerPorGuidAsync(guid, ct);
        return m is null ? null : TipoServicioBusinessMapper.ARespuesta(m);
    }

    public async Task<TipoServicioResponse?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ValidationException("Nombre es obligatorio.");
        var m = await _data.ObtenerPorNombreAsync(nombre.Trim(), ct);
        return m is null ? null : TipoServicioBusinessMapper.ARespuesta(m);
    }

    public async Task<IReadOnlyList<TipoServicioResponse>> ListarActivosAsync(CancellationToken ct = default)
    {
        var items = await _data.ListarActivosAsync(ct);
        return items.Select(TipoServicioBusinessMapper.ARespuesta).ToList();
    }

    public async Task<PagedResultado<TipoServicioResponse>> ListarPaginadoAsync(
        int paginaActual, int tamanoPagina, CancellationToken ct = default)
    {
        TipoServicioValidator.ValidarPaginacion(paginaActual, tamanoPagina);
        var p = await _data.ListarPaginadoAsync(paginaActual, tamanoPagina, ct);
        return TipoServicioBusinessMapper.APaginado(p, TipoServicioBusinessMapper.ARespuesta);
    }

    public async Task<TipoServicioResponse> CrearAsync(CrearTipoServicioRequest request, CancellationToken ct = default)
    {
        TipoServicioValidator.ValidarCrear(request);
        var modelo = TipoServicioBusinessMapper.ACrearDataModel(request);
        try
        {
            var creado = await _data.CrearAsync(modelo, ct);
            return TipoServicioBusinessMapper.ARespuesta(creado);
        }
        catch (InvalidOperationException ex) { DataServiceExceptionMapper.PropagarSiInvalidOperation(ex); throw; }
    }

    public async Task<TipoServicioResponse> ActualizarAsync(ActualizarTipoServicioRequest request, CancellationToken ct = default)
    {
        TipoServicioValidator.ValidarActualizar(request);
        var existente = await _data.ObtenerPorGuidAsync(request.GuidTipoServicio, ct)
            ?? throw new NotFoundException("No se encontró el tipo de servicio indicado.");
        TipoServicioBusinessMapper.AplicarActualizacion(request, existente);
        try
        {
            var actualizado = await _data.ActualizarAsync(existente, ct);
            return TipoServicioBusinessMapper.ARespuesta(actualizado);
        }
        catch (InvalidOperationException ex) { DataServiceExceptionMapper.PropagarSiInvalidOperation(ex); throw; }
    }

    public async Task EliminarAsync(Guid guid, CancellationToken ct = default)
    {
        if (guid == Guid.Empty) throw new ValidationException("GuidTipoServicio no es válido.");
        try { await _data.EliminarLogicoAsync(guid, ct); }
        catch (InvalidOperationException ex) { DataServiceExceptionMapper.PropagarSiInvalidOperation(ex); throw; }
    }
}
