using Microservicio.Pooking.Servicio.Business.DTOs.TipoServicio;
using Microservicio.Pooking.Servicio.Business.Exceptions;

namespace Microservicio.Pooking.Servicio.Business.Validators;

public static class TipoServicioValidator
{
    public const int TamanoPaginaMaximo = 100;

    private static readonly HashSet<string> EstadosValidos =
        new(StringComparer.OrdinalIgnoreCase) { "ACT", "INA" };

    private static readonly HashSet<string> NombresValidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Vuelos", "Alojamiento", "Atracciones", "Alquiler de Carros"
        };

    public static void ValidarPaginacion(int paginaActual, int tamanoPagina)
    {
        var errores = new List<string>();
        if (paginaActual < 1) errores.Add("PaginaActual debe ser >= 1.");
        if (tamanoPagina < 1) errores.Add("TamanoPagina debe ser >= 1.");
        if (tamanoPagina > TamanoPaginaMaximo) errores.Add($"TamanoPagina no puede superar {TamanoPaginaMaximo}.");
        if (errores.Count > 0) throw new ValidationException("Error de paginación.", errores);
    }

    public static void ValidarCrear(CrearTipoServicioRequest r)
    {
        ArgumentNullException.ThrowIfNull(r);
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(r.Nombre))
            errores.Add("Nombre es obligatorio.");
        else if (!NombresValidos.Contains(r.Nombre.Trim()))
            errores.Add("Nombre debe ser: Vuelos, Alojamiento, Atracciones o Alquiler de Carros.");

        ValidarEstado(r.Estado, errores);
        if (errores.Count > 0) throw new ValidationException("Error al crear tipo de servicio.", errores);
    }

    public static void ValidarActualizar(ActualizarTipoServicioRequest r)
    {
        ArgumentNullException.ThrowIfNull(r);
        var errores = new List<string>();

        if (r.GuidTipoServicio == Guid.Empty) errores.Add("GuidTipoServicio es obligatorio.");
        if (string.IsNullOrWhiteSpace(r.Nombre))
            errores.Add("Nombre es obligatorio.");
        else if (!NombresValidos.Contains(r.Nombre.Trim()))
            errores.Add("Nombre debe ser: Vuelos, Alojamiento, Atracciones o Alquiler de Carros.");

        ValidarEstado(r.Estado, errores);
        if (errores.Count > 0) throw new ValidationException("Error al actualizar tipo de servicio.", errores);
    }

    private static void ValidarEstado(string estado, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(estado))
            errores.Add("Estado es obligatorio.");
        else if (!EstadosValidos.Contains(estado.Trim()))
            errores.Add("Estado debe ser ACT o INA.");
    }
}
