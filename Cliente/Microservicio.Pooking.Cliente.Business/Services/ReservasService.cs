using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Reservas;
using Microservicio.Pooking.Cliente.Business.Exceptions;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microservicio.Pooking.Cliente.Business.Mappers;
using Microservicio.Pooking.Cliente.Business.Validators;
using Microservicio.Pooking.Cliente.DataManagement.Interfaces;

namespace Microservicio.Pooking.Cliente.Business.Services;

public class ReservasService : IReservasService
{
    private readonly IReservasDataService _reservasDataService;
    private readonly IClienteDataService _clienteDataService;

    public ReservasService(
        IReservasDataService reservasDataService,
        IClienteDataService clienteDataService)
    {
        _reservasDataService = reservasDataService;
        _clienteDataService = clienteDataService;
    }

    public async Task<ReservaResponse> ObtenerPorGuidAsync(Guid guidReserva, CancellationToken cancellationToken = default)
    {
        var model = await _reservasDataService.ObtenerPorGuidAsync(guidReserva, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la reserva con GUID '{guidReserva}'.");

        return ReservasBusinessMapper.ToResponse(model);
    }

    public async Task<PagedResultado<ReservaResponse>> ListarPorClienteAsync(
        Guid guidCliente, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        var cliente = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        var paged = await _reservasDataService.ListarPorClienteAsync(cliente.IdCliente, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ReservasBusinessMapper.ToResponse);

        return new PagedResultado<ReservaResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<PagedResultado<ReservaResponse>> ListarPorEstadoAsync(
        string estado, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        var estadosValidos = new[] { "PEND", "CONF", "CANC", "COMP" };
        if (!estadosValidos.Contains(estado, StringComparer.OrdinalIgnoreCase))
            throw new ValidationException(
                "Estado inválido.",
                new[] { "El estado debe ser PEND, CONF, CANC o COMP." });

        var paged = await _reservasDataService.ListarPorEstadoAsync(estado.ToUpperInvariant(), pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ReservasBusinessMapper.ToResponse);

        return new PagedResultado<ReservaResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<ReservaResponse> CrearAsync(CrearReservaRequest request, CancellationToken cancellationToken = default)
    {
        var errores = ReservasValidator.ValidarCrear(request);
        if (errores.Count > 0)
            throw new ValidationException("Errores de validación al crear reserva.", errores);

        var cliente = await _clienteDataService.ObtenerPorGuidAsync(request.GuidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{request.GuidCliente}'.");

        if (!string.Equals(cliente.Estado, "ACT", StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("No se pueden crear reservas para clientes inactivos o suspendidos.");

        // TODO: validar existencia y disponibilidad del servicio vía gRPC al Catálogo.
        // Por ahora confiamos en el snapshot que viene del frontend.

        var dataModel = ReservasBusinessMapper.ToDataModelFromCreate(request, cliente.IdCliente);
        var creada = await _reservasDataService.CrearAsync(dataModel, cancellationToken);
        creada.GuidClienteRef = cliente.GuidCliente;

        return ReservasBusinessMapper.ToResponse(creada);
    }

    private static void NormalizarPaginacion(ref int pagina, ref int tamanio)
    {
        if (pagina < 1) pagina = 1;
        if (tamanio < 1) tamanio = 10;
        if (tamanio > 100) tamanio = 100;
    }
}
