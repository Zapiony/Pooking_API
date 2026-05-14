using Microservicio.Pooking.Servicio.Business.DTOs.Servicio;
using Microservicio.Pooking.Servicio.Business.Exceptions;

namespace Microservicio.Pooking.Servicio.Business.Validators;

public static class ServicioValidator
{
    public const int TamanoPaginaMaximo = 100;

    private static readonly HashSet<string> EstadosValidos =
        new(StringComparer.OrdinalIgnoreCase) { "ACT", "INA", "SUS" };

    private static readonly HashSet<string> TiposIdentificacionValidos =
        new(StringComparer.OrdinalIgnoreCase) { "RUC", "CI", "PASS", "EXT" };

    public static void ValidarPaginacion(int paginaActual, int tamanoPagina)
    {
        var errores = new List<string>();
        if (paginaActual < 1) errores.Add("PaginaActual debe ser >= 1.");
        if (tamanoPagina < 1) errores.Add("TamanoPagina debe ser >= 1.");
        if (tamanoPagina > TamanoPaginaMaximo) errores.Add($"TamanoPagina no puede superar {TamanoPaginaMaximo}.");
        if (errores.Count > 0) throw new ValidationException("Error de paginacion.", errores);
    }

    public static void ValidarFiltro(ServicioFiltroRequest r)
    {
        ArgumentNullException.ThrowIfNull(r);
        ValidarPaginacion(r.PaginaActual, r.TamanoPagina);
    }

    public static void ValidarCrear(CrearServicioRequest r)
    {
        ArgumentNullException.ThrowIfNull(r);
        var errores = new List<string>();

        if (r.GuidTipoServicio == Guid.Empty) errores.Add("GuidTipoServicio es obligatorio.");
        ValidarCamposEditables(r.RazonSocial, r.TipoIdentificacion, r.NumeroIdentificacion, r.CorreoContacto, errores);
        ValidarEstado(r.Estado, errores);
        ValidarUrlOpcional(r.SitioWeb, "SitioWeb", errores);
        ValidarUrlOpcional(r.LogoUrl, "LogoUrl", errores);
        if (errores.Count > 0) throw new ValidationException("Error al crear servicio.", errores);
    }

    public static void ValidarActualizar(ActualizarServicioRequest r)
    {
        ArgumentNullException.ThrowIfNull(r);
        var errores = new List<string>();

        if (r.GuidServicio == Guid.Empty) errores.Add("GuidServicio es obligatorio.");
        if (r.GuidTipoServicio == Guid.Empty) errores.Add("GuidTipoServicio es obligatorio.");
        ValidarCamposEditables(r.RazonSocial, r.TipoIdentificacion, r.NumeroIdentificacion, r.CorreoContacto, errores);
        ValidarUrlOpcional(r.SitioWeb, "SitioWeb", errores);
        ValidarUrlOpcional(r.LogoUrl, "LogoUrl", errores);
        if (errores.Count > 0) throw new ValidationException("Error al actualizar servicio.", errores);
    }

    private static void ValidarCamposEditables(
        string razonSocial, string tipoId, string numeroId, string? correo, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(razonSocial)) errores.Add("RazonSocial es obligatorio.");
        if (string.IsNullOrWhiteSpace(tipoId))
            errores.Add("TipoIdentificacion es obligatorio.");
        else if (!TiposIdentificacionValidos.Contains(tipoId.Trim()))
            errores.Add("TipoIdentificacion debe ser: RUC, CI, PASS o EXT.");
        if (string.IsNullOrWhiteSpace(numeroId)) errores.Add("NumeroIdentificacion es obligatorio.");
        if (!string.IsNullOrWhiteSpace(correo) && (!correo.Contains('@') || !correo.Contains('.')))
            errores.Add("CorreoContacto no tiene formato valido.");
    }

    private static void ValidarEstado(string estado, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(estado))
            errores.Add("Estado es obligatorio.");
        else if (!EstadosValidos.Contains(estado.Trim()))
            errores.Add("Estado debe ser ACT, INA o SUS.");
    }

    private static void ValidarUrlOpcional(string? url, string campo, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            errores.Add($"{campo} debe ser una URL valida (debe comenzar con http:// o https://).");
    }
}
