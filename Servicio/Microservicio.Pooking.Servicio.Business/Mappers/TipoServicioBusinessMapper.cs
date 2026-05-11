using Microservicio.Pooking.Servicio.Business.DTOs;
using Microservicio.Pooking.Servicio.Business.DTOs.TipoServicio;
using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.Business.Mappers;

public static class TipoServicioBusinessMapper
{
    public static TipoServicioDataModel ACrearDataModel(CrearTipoServicioRequest r) => new()
    {
        Nombre = r.Nombre.Trim(),
        Descripcion = r.Descripcion?.Trim(),
        Estado = r.Estado.Trim(),
        CreadoPorUsuario = r.CreadoPorUsuario,
        ModificacionIp = r.ModificacionIp,
        ServicioOrigen = r.ServicioOrigen
    };

    public static void AplicarActualizacion(ActualizarTipoServicioRequest r, TipoServicioDataModel destino)
    {
        var rv = RowVersionMapper.DesdeBase64(r.RowVersionBase64);
        if (rv is not null) destino.RowVersion = rv;

        destino.Nombre = r.Nombre.Trim();
        destino.Descripcion = r.Descripcion?.Trim();
        destino.Estado = r.Estado.Trim();
        destino.ModificadoPorUsuario = r.ModificadoPorUsuario;
        destino.ModificacionIp = r.ModificacionIp;
        destino.ServicioOrigen = r.ServicioOrigen;
    }

    public static TipoServicioResponse ARespuesta(TipoServicioDataModel m) => new()
    {
        GuidTipoServicio = m.GuidTipoServicio,
        Nombre = m.Nombre,
        Descripcion = m.Descripcion,
        Estado = m.Estado,
        EsEliminado = m.EsEliminado,
        CreadoPorUsuario = m.CreadoPorUsuario,
        FechaRegistroUtc = m.FechaRegistroUtc,
        ModificadoPorUsuario = m.ModificadoPorUsuario,
        FechaModificacionUtc = m.FechaModificacionUtc,
        RowVersionBase64 = RowVersionMapper.ABase64(m.RowVersion)
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
