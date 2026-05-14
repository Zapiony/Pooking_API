using System.ComponentModel.DataAnnotations;

namespace Microservicio.Pooking.Servicio.Business.DTOs.Servicio;

public sealed class ActualizarServicioRequest
{
    public Guid GuidServicio { get; set; }
    public Guid GuidTipoServicio { get; set; }
    [Required(ErrorMessage = "El campo razonSocial es obligatorio.")]
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    [Required(ErrorMessage = "El campo tipoIdentificacion es obligatorio.")]
    public string TipoIdentificacion { get; set; } = string.Empty;
    [Required(ErrorMessage = "El campo numeroIdentificacion es obligatorio.")]
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string? CorreoContacto { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? Direccion { get; set; }
    [Url(ErrorMessage = "La URL no tiene un formato valido. Debe comenzar con http:// o https://")]
    public string? SitioWeb { get; set; }
    [Url(ErrorMessage = "La URL no tiene un formato valido. Debe comenzar con http:// o https://")]
    public string? LogoUrl { get; set; }
    public string? ModificadoPorUsuario { get; set; }
    public string? ModificacionIp { get; set; }
    public string? ServicioOrigen { get; set; }
}
