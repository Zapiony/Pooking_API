using System.ComponentModel.DataAnnotations;

namespace Microservicio.Pooking.Servicio.Business.DTOs.TipoServicio;

public sealed class CrearTipoServicioRequest
{
    /// <summary>Vuelos | Alojamiento | Atracciones | Alquiler de Carros</summary>
    [Required(ErrorMessage = "El campo nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = "ACT";
    public string? CreadoPorUsuario { get; set; }
    public string? ModificacionIp { get; set; }
    public string? ServicioOrigen { get; set; }
}
