using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;
using Microservicio.Pooking.Cliente.Business.Exceptions;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microservicio.Pooking.Cliente.Business.Mappers;
using Microservicio.Pooking.Cliente.Business.Validators;
using Microservicio.Pooking.Cliente.DataManagement.Interfaces;

namespace Microservicio.Pooking.Cliente.Business.Services;

public class FavoritosService : IFavoritosService
{
    private readonly IFavoritosDataService _favoritosDataService;
    private readonly IClienteDataService _clienteDataService;

    public FavoritosService(
        IFavoritosDataService favoritosDataService,
        IClienteDataService clienteDataService)
    {
        _favoritosDataService = favoritosDataService;
        _clienteDataService = clienteDataService;
    }

    public async Task<FavoritoResponse> ObtenerPorGuidAsync(Guid guidFavorito, CancellationToken cancellationToken = default)
    {
        var model = await _favoritosDataService.ObtenerPorGuidAsync(guidFavorito, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el favorito con GUID '{guidFavorito}'.");

        return FavoritosBusinessMapper.ToResponse(model);
    }

    public async Task<PagedResultado<FavoritoResponse>> ListarPorClienteAsync(
        Guid guidCliente, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        // Validar que el cliente exista
        _ = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        var paged = await _favoritosDataService.ListarPorClienteAsync(guidCliente, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(FavoritosBusinessMapper.ToResponse);

        return new PagedResultado<FavoritoResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<FavoritoResponse> CrearAsync(CrearFavoritoRequest request, CancellationToken cancellationToken = default)
    {
        var errores = FavoritosValidator.ValidarCrear(request);
        if (errores.Count > 0)
            throw new ValidationException("Errores de validación al crear favorito.", errores);

        // Validar que el cliente exista
        _ = await _clienteDataService.ObtenerPorGuidAsync(request.GuidClienteRef, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{request.GuidClienteRef}'.");

        // Validar que no exista ya el mismo (cliente, servicio)
        if (await _favoritosDataService.ExisteFavoritoAsync(
            request.GuidClienteRef, request.GuidServicioRef, cancellationToken))
        {
            throw new BusinessException("Este servicio ya está marcado como favorito por el cliente.");
        }

        // TODO: validar existencia y estado del servicio vía gRPC al Catálogo
        // Según el roadmap, si el servicio está INA o SUS, se rechaza el favorito.

        var dataModel = FavoritosBusinessMapper.ToDataModelFromCreate(request);
        var creado = await _favoritosDataService.CrearAsync(dataModel, cancellationToken);

        return FavoritosBusinessMapper.ToResponse(creado);
    }

    public async Task EliminarAsync(Guid guidFavorito, CancellationToken cancellationToken = default)
    {
        _ = await _favoritosDataService.ObtenerPorGuidAsync(guidFavorito, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el favorito con GUID '{guidFavorito}'.");

        await _favoritosDataService.EliminarLogicoAsync(guidFavorito, cancellationToken);
    }

    private static void NormalizarPaginacion(ref int pagina, ref int tamanio)
    {
        if (pagina < 1) pagina = 1;
        if (tamanio < 1) tamanio = 10;
        if (tamanio > 100) tamanio = 100;
    }
}
