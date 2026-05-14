using Microservicio.Pooking.Servicio.DataAccess.Entities;
using Microservicio.Pooking.Servicio.DataAccess.Queries;
using Microservicio.Pooking.Servicio.DataManagement.Models;

namespace Microservicio.Pooking.Servicio.DataManagement.Mappers;

public static class ServicioDataMapper
{
    /// <summary>
    /// Usa la navegación TipoServicio si está cargada; si no, usa el parámetro opcional.
    /// </summary>
    public static ServicioDataModel AModelo(ServicioEntity e, TipoServicioEntity? tipo = null)
    {
        var t = e.TipoServicio ?? tipo;
        return new ServicioDataModel
        {
            IdServicio = e.IdServicio,
            GuidServicio = e.GuidServicio,
            IdTipoServicio = e.IdTipoServicio,
            GuidTipoServicio = t?.GuidTipoServicio ?? Guid.Empty,
            RazonSocial = e.RazonSocial,
            NombreComercial = e.NombreComercial,
            TipoIdentificacion = e.TipoIdentificacion,
            NumeroIdentificacion = e.NumeroIdentificacion,
            CorreoContacto = e.CorreoContacto,
            TelefonoContacto = e.TelefonoContacto,
            Direccion = e.Direccion,
            SitioWeb = e.SitioWeb,
            LogoUrl = e.LogoUrl,
            Estado = e.Estado,
            EsEliminado = e.EsEliminado,
            CreadoPorUsuario = e.CreadoPorUsuario,
            FechaRegistroUtc = e.FechaRegistroUtc,
            ModificadoPorUsuario = e.ModificadoPorUsuario,
            FechaModificacionUtc = e.FechaModificacionUtc,
            ModificacionIp = e.ModificacionIp,
            ServicioOrigen = e.ServicioOrigen,
            TipoServicioNombre = t?.Nombre
        };
    }

    public static ServicioResumenDataModel AResumen(ServicioResumenDto dto) =>
        new(dto.GuidServicio, dto.RazonSocial, dto.NombreComercial, dto.TipoServicioNombre, dto.Estado);

    public static ServicioDataModel AModeloDesdeDetalle(ServicioDetalleDto dto) => new()
    {
        GuidServicio = dto.GuidServicio,
        RazonSocial = dto.RazonSocial,
        NombreComercial = dto.NombreComercial,
        TipoIdentificacion = dto.TipoIdentificacion,
        NumeroIdentificacion = dto.NumeroIdentificacion,
        CorreoContacto = dto.CorreoContacto,
        TelefonoContacto = dto.TelefonoContacto,
        Direccion = dto.Direccion,
        SitioWeb = dto.SitioWeb,
        LogoUrl = dto.LogoUrl,
        Estado = dto.Estado,
        EsEliminado = dto.EsEliminado,
        GuidTipoServicio = dto.GuidTipoServicio,
        TipoServicioNombre = dto.TipoServicioNombre,
        CreadoPorUsuario = dto.CreadoPorUsuario,
        FechaRegistroUtc = dto.FechaRegistroUtc,
        ModificadoPorUsuario = dto.ModificadoPorUsuario,
        FechaModificacionUtc = dto.FechaModificacionUtc
    };

    public static ServicioEntity ANuevaEntidad(ServicioDataModel m, int idTipoServicio) => new()
    {
        GuidServicio = m.GuidServicio != Guid.Empty ? m.GuidServicio : Guid.NewGuid(),
        IdTipoServicio = idTipoServicio,
        RazonSocial = m.RazonSocial,
        NombreComercial = m.NombreComercial,
        TipoIdentificacion = m.TipoIdentificacion,
        NumeroIdentificacion = m.NumeroIdentificacion,
        CorreoContacto = m.CorreoContacto,
        TelefonoContacto = m.TelefonoContacto,
        Direccion = m.Direccion,
        SitioWeb = m.SitioWeb,
        LogoUrl = m.LogoUrl,
        Estado = string.IsNullOrWhiteSpace(m.Estado) ? "ACT" : m.Estado,
        EsEliminado = false,
        CreadoPorUsuario = m.CreadoPorUsuario,
        FechaRegistroUtc = m.FechaRegistroUtc != default ? m.FechaRegistroUtc : DateTimeOffset.UtcNow,
        ModificacionIp = m.ModificacionIp,
        ServicioOrigen = m.ServicioOrigen
    };

    public static void AplicarCambios(ServicioEntity destino, ServicioDataModel origen)
    {
        if (origen.IdTipoServicio > 0)
            destino.IdTipoServicio = origen.IdTipoServicio;
        destino.RazonSocial = origen.RazonSocial;
        destino.NombreComercial = origen.NombreComercial;
        destino.TipoIdentificacion = origen.TipoIdentificacion;
        destino.NumeroIdentificacion = origen.NumeroIdentificacion;
        destino.CorreoContacto = origen.CorreoContacto;
        destino.TelefonoContacto = origen.TelefonoContacto;
        destino.Direccion = origen.Direccion;
        destino.SitioWeb = origen.SitioWeb;
        destino.LogoUrl = origen.LogoUrl;
        destino.ModificadoPorUsuario = origen.ModificadoPorUsuario;
        destino.FechaModificacionUtc = origen.FechaModificacionUtc ?? DateTimeOffset.UtcNow;
        destino.ModificacionIp = origen.ModificacionIp;
        destino.ServicioOrigen = origen.ServicioOrigen;
    }
}
