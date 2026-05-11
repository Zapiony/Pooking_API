using Microservicio.Pooking.Servicio.DataAcces.Entities;
using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.DataManagement.Mappers;

public static class TipoServicioDataMapper
{
    public static TipoServicioDataModel AModelo(TipoServicioEntity e) => new()
    {
        IdTipoServicio = e.IdTipoServicio,
        GuidTipoServicio = e.GuidTipoServicio,
        Nombre = e.Nombre,
        Descripcion = e.Descripcion,
        Estado = e.Estado,
        EsEliminado = e.EsEliminado,
        CreadoPorUsuario = e.CreadoPorUsuario,
        FechaRegistroUtc = e.FechaRegistroUtc,
        ModificadoPorUsuario = e.ModificadoPorUsuario,
        FechaModificacionUtc = e.FechaModificacionUtc,
        ModificacionIp = e.ModificacionIp,
        ServicioOrigen = e.ServicioOrigen
    };

    public static TipoServicioEntity ANuevaEntidad(TipoServicioDataModel m) => new()
    {
        GuidTipoServicio = m.GuidTipoServicio != Guid.Empty ? m.GuidTipoServicio : Guid.NewGuid(),
        Nombre = m.Nombre,
        Descripcion = m.Descripcion,
        Estado = string.IsNullOrWhiteSpace(m.Estado) ? "ACT" : m.Estado,
        EsEliminado = false,
        CreadoPorUsuario = m.CreadoPorUsuario,
        FechaRegistroUtc = m.FechaRegistroUtc != default ? m.FechaRegistroUtc : DateTimeOffset.UtcNow,
        ModificacionIp = m.ModificacionIp,
        ServicioOrigen = m.ServicioOrigen
    };

    public static void AplicarCambios(TipoServicioEntity destino, TipoServicioDataModel origen)
    {
        destino.Nombre = origen.Nombre;
        destino.Descripcion = origen.Descripcion;
        destino.Estado = origen.Estado;
        destino.ModificadoPorUsuario = origen.ModificadoPorUsuario;
        destino.FechaModificacionUtc = origen.FechaModificacionUtc ?? DateTimeOffset.UtcNow;
        destino.ModificacionIp = origen.ModificacionIp;
        destino.ServicioOrigen = origen.ServicioOrigen;
    }
}
