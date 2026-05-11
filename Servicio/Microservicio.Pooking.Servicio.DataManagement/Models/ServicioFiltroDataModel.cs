namespace Microservicio.Pooking.Servicio.DataManagement.Models;

public sealed class ServicioFiltroDataModel
{
    /// <summary>Término parcial sobre razón social o nombre comercial.</summary>
    public string? Termino { get; set; }

    /// <summary>Filtra por tipo de servicio (opcional).</summary>
    public Guid? GuidTipoServicio { get; set; }

    public int PaginaActual { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}
