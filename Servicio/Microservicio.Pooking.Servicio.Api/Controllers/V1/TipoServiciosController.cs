using Asp.Versioning;
using Microservicio.Pooking.Servicio.Api.Models.Common;
using Microservicio.Pooking.Servicio.Business.DTOs.TipoServicio;
using Microservicio.Pooking.Servicio.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Pooking.Servicio.Api.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/tipos-servicio")]
[AllowAnonymous]
public sealed class TipoServiciosController : ControllerBase
{
    private readonly ITipoServicioService _tiposServicio;

    public TipoServiciosController(ITipoServicioService tiposServicio)
    {
        _tiposServicio = tiposServicio;
    }

    /// <summary>Listado paginado de tipos de servicio.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> ListarPaginadoAsync(
        [FromQuery] int paginaActual = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken ct = default)
    {
        var resultado = await _tiposServicio.ListarPaginadoAsync(paginaActual, tamanoPagina, ct);
        return Ok(ApiResponse<object>.Exitoso(resultado, "Consulta exitosa."));
    }

    /// <summary>Todos los tipos de servicio activos (sin paginación).</summary>
    [HttpGet("activos")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TipoServicioResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TipoServicioResponse>>>> ListarActivosAsync(
        CancellationToken ct = default)
    {
        var items = await _tiposServicio.ListarActivosAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<TipoServicioResponse>>.Exitoso(items, "Consulta exitosa."));
    }

    /// <summary>Obtiene un tipo de servicio por nombre exacto.</summary>
    [HttpGet("por-nombre")]
    [ProducesResponseType(typeof(ApiResponse<TipoServicioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorNombreAsync(
        [FromQuery] string nombre,
        CancellationToken ct = default)
    {
        var data = await _tiposServicio.ObtenerPorNombreAsync(nombre, ct);
        if (data is null)
            return NotFound(ApiErrorResponse.Crear(
                "Tipo de servicio no encontrado.",
                new[] { $"No existe tipo con nombre '{nombre}'." }));

        return Ok(ApiResponse<TipoServicioResponse>.Exitoso(data, "Consulta exitosa."));
    }

    /// <summary>Obtiene un tipo de servicio por GUID.</summary>
    [HttpGet("{guid:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TipoServicioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorGuidAsync(
        Guid guid,
        CancellationToken ct = default)
    {
        var data = await _tiposServicio.ObtenerPorGuidAsync(guid, ct);
        if (data is null)
            return NotFound(ApiErrorResponse.Crear(
                "Tipo de servicio no encontrado.",
                new[] { $"No existe tipo con Guid {guid}." }));

        return Ok(ApiResponse<TipoServicioResponse>.Exitoso(data, "Consulta exitosa."));
    }

    /// <summary>Crea un nuevo tipo de servicio.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TipoServicioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TipoServicioResponse>>> CrearAsync(
        [FromBody] CrearTipoServicioRequest request,
        CancellationToken ct = default)
    {
        var creado = await _tiposServicio.CrearAsync(request, ct);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<TipoServicioResponse>.Exitoso(creado, "Tipo de servicio creado."));
    }

    /// <summary>Actualiza un tipo de servicio existente.</summary>
    [HttpPut("{guid:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TipoServicioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TipoServicioResponse>>> ActualizarAsync(
        Guid guid,
        [FromBody] ActualizarTipoServicioRequest request,
        CancellationToken ct = default)
    {
        request.GuidTipoServicio = guid;
        var actualizado = await _tiposServicio.ActualizarAsync(request, ct);
        return Ok(ApiResponse<TipoServicioResponse>.Exitoso(actualizado, "Tipo de servicio actualizado."));
    }

    /// <summary>Eliminación lógica de un tipo de servicio.</summary>
    [HttpDelete("{guid:guid}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarAsync(Guid guid, CancellationToken ct = default)
    {
        await _tiposServicio.EliminarAsync(guid, ct);
        return Ok(ApiResponse<string>.Exitoso("OK", "Tipo de servicio eliminado lógicamente."));
    }
}
