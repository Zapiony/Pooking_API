using Microservicio.Pooking.Servicio.Business.DTOs;
using Microservicio.Pooking.Servicio.Business.DTOs.Servicio;
using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.Business.Mappers;

public static class ServicioBusinessMapper
{
    public static ServicioFiltroDataModel AFiltroData(ServicioFiltroRequest r) => new()
    {
        Termino = r.Termino,
        GuidTipoServicio = r.GuidTipoServicio,
        PaginaActual = r.PaginaActual,
        TamanoPagina = r.TamanoPagina
    };

    public static ServicioDataModel ACrearDataModel(CrearServicioRequest r) => new()
    {
        GuidTipoServicio = r.GuidTipoServicio,
        RazonSocial = r.RazonSocial.Trim(),
        NombreComercial = r.NombreComercial?.Trim(),
        TipoIdentificacion = r.TipoIdentificacion.Trim(),
        NumeroIdentificacion = r.NumeroIdentificacion.Trim(),
        CorreoContacto = r.CorreoContacto?.Trim() ?? string.Empty,
        TelefonoContacto = r.TelefonoContacto?.Trim(),
        Direccion = r.Direccion?.Trim(),
        SitioWeb = r.SitioWeb?.Trim(),
        LogoUrl = r.LogoUrl?.Trim(),
        Estado = r.Estado.Trim(),
        CreadoPorUsuario = r.CreadoPorUsuario,
        ModificacionIp = r.ModificacionIp,
        ServicioOrigen = r.ServicioOrigen
    };

    public static void AplicarActualizacion(ActualizarServicioRequest r, ServicioDataModel destino)
    {
        destino.GuidTipoServicio = r.GuidTipoServicio;
        destino.RazonSocial = r.RazonSocial.Trim();
        destino.NombreComercial = r.NombreComercial?.Trim();
        destino.TipoIdentificacion = r.TipoIdentificacion.Trim();
        destino.NumeroIdentificacion = r.NumeroIdentificacion.Trim();
        destino.CorreoContacto = r.CorreoContacto?.Trim() ?? string.Empty;
        destino.TelefonoContacto = r.TelefonoContacto?.Trim();
        destino.Direccion = r.Direccion?.Trim();
        destino.SitioWeb = r.SitioWeb?.Trim();
        destino.LogoUrl = r.LogoUrl?.Trim();
        destino.ModificadoPorUsuario = r.ModificadoPorUsuario;
        destino.ModificacionIp = r.ModificacionIp;
        destino.ServicioOrigen = r.ServicioOrigen;
    }

    public static ServicioResponse ARespuesta(ServicioDataModel m) => new()
    {
        GuidServicio = m.GuidServicio,
        GuidTipoServicio = m.GuidTipoServicio,
        TipoServicioNombre = m.TipoServicioNombre,
        RazonSocial = m.RazonSocial,
        NombreComercial = m.NombreComercial,
        TipoIdentificacion = m.TipoIdentificacion,
        NumeroIdentificacion = m.NumeroIdentificacion,
        CorreoContacto = m.CorreoContacto,
        TelefonoContacto = m.TelefonoContacto,
        Direccion = m.Direccion,
        SitioWeb = m.SitioWeb,
        LogoUrl = m.LogoUrl,
        Estado = m.Estado,
        EsEliminado = m.EsEliminado,
        CreadoPorUsuario = m.CreadoPorUsuario,
        FechaRegistroUtc = m.FechaRegistroUtc,
        ModificadoPorUsuario = m.ModificadoPorUsuario,
        FechaModificacionUtc = m.FechaModificacionUtc,
        RowVersionBase64 = RowVersionMapper.ABase64(m.RowVersion)
    };

    public static ServicioResumenResponse AResumenRespuesta(ServicioResumenDataModel m) => new()
    {
        GuidServicio = m.GuidServicio,
        RazonSocial = m.RazonSocial,
        NombreComercial = m.NombreComercial,
        TipoServicioNombre = m.TipoServicioNombre,
        Estado = m.Estado
    };

    public static PagedResultado<TDestino> APaginado<TOrigen, TDestino>(
        DataPagedResult<TOrigen> origen, Func<TOrigen, TDestino> mapear) => new()
    {
        Items = origen.Items.Select(mapear).ToList(),
        PaginaActual = origen.PaginaActual,
        TamanoPagina = origen.TamanoPagina,
        TotalRegistros = origen.TotalRegistros,
        TotalPaginas = origen.TotalPaginas,
        TienePaginaAnterior = origen.TienePaginaAnterior,
        TienePaginaSiguiente = origen.TienePaginaSiguiente
    };
}
