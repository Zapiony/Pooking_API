using Asp.Versioning;
using Microservicio.Pooking.Servicio.Api.Models.Common;
using Microservicio.Pooking.Servicio.Business.DTOs.Servicio;
using Microservicio.Pooking.Servicio.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Pooking.Servicio.Api.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/servicios")]
[AllowAnonymous]
public sealed class ServiciosController : ControllerBase
{
    private readonly IServicioService _servicios;

    public ServiciosController(IServicioService servicios)
    {
        _servicios = servicios;
    }

    /// <summary>Listado paginado con filtros opcionales (término, tipo de servicio).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> ListarAsync(
        [FromQuery] string? termino,
        [FromQuery] Guid? guidTipo,
        [FromQuery] int paginaActual = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken ct = default)
    {
        var filtro = new ServicioFiltroRequest
        {
            Termino = termino,
            GuidTipoServicio = guidTipo,
            PaginaActual = paginaActual,
            TamanoPagina = tamanoPagina
        };

        var resultado = await _servicios.ListarOBuscarAsync(filtro, ct);
        return Ok(ApiResponse<object>.Exitoso(resultado, "Consulta exitosa."));
    }

    /// <summary>Listado completo (entidades con todos los campos) paginado.</summary>
    [HttpGet("pagina-completa")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> ListarPaginaCompletaAsync(
        [FromQuery] int paginaActual = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken ct = default)
    {
        var resultado = await _servicios.ListarEntidadesPaginadoAsync(paginaActual, tamanoPagina, ct);
        return Ok(ApiResponse<object>.Exitoso(resultado, "Consulta exitosa."));
    }

    /// <summary>Obtiene el detalle completo de un servicio por GUID.</summary>
    [HttpGet("{guid:guid}/detalle")]
    [ProducesResponseType(typeof(ApiResponse<ServicioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerDetalleAsync(Guid guid, CancellationToken ct = default)
    {
        var data = await _servicios.ObtenerDetallePorGuidAsync(guid, ct);
        if (data is null)
            return NotFound(ApiErrorResponse.Crear(
                "Servicio no encontrado.",
                new[] { $"No existe servicio con Guid {guid}." }));

        return Ok(ApiResponse<ServicioResponse>.Exitoso(data, "Consulta exitosa."));
    }

    /// <summary>Obtiene un servicio por GUID.</summary>
    [HttpGet("{guid:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServicioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        var data = await _servicios.ObtenerPorGuidAsync(guid, ct);
        if (data is null)
            return NotFound(ApiErrorResponse.Crear(
                "Servicio no encontrado.",
                new[] { $"No existe servicio con Guid {guid}." }));

        return Ok(ApiResponse<ServicioResponse>.Exitoso(data, "Consulta exitosa."));
    }

    /// <summary>Registra un nuevo servicio (proveedor integrable) en el catálogo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ServicioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ServicioResponse>>> CrearAsync(
        [FromBody] CrearServicioRequest request,
        CancellationToken ct = default)
    {
        var creado = await _servicios.CrearAsync(request, ct);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<ServicioResponse>.Exitoso(creado, "Servicio creado."));
    }

    /// <summary>Actualiza los datos de un servicio existente.</summary>
    [HttpPut("{guid:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServicioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ServicioResponse>>> ActualizarAsync(
        Guid guid,
        [FromBody] ActualizarServicioRequest request,
        CancellationToken ct = default)
    {
        request.GuidServicio = guid;
        var actualizado = await _servicios.ActualizarAsync(request, ct);
        return Ok(ApiResponse<ServicioResponse>.Exitoso(actualizado, "Servicio actualizado."));
    }

    /// <summary>Eliminación lógica de un servicio.</summary>
    [HttpDelete("{guid:guid}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarAsync(Guid guid, CancellationToken ct = default)
    {
        await _servicios.EliminarAsync(guid, ct);
        return Ok(ApiResponse<string>.Exitoso("OK", "Servicio eliminado lógicamente."));
    }
}
