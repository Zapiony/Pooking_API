using System.ComponentModel.DataAnnotations;

namespace Microservicio.Pooking.Servicio.Business.DTOs.TipoServicio;

public sealed class ActualizarTipoServicioRequest
{
    public Guid GuidTipoServicio { get; set; }
    [Required(ErrorMessage = "El campo nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ModificadoPorUsuario { get; set; }
    public string? ModificacionIp { get; set; }
    public string? ServicioOrigen { get; set; }
}
