namespace Booking.Auditoria.DataManagement.DTOs;

/// <summary>
/// Respuesta paginada genérica.
/// </summary>
public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalRegistros { get; set; }
    public int Pagina { get; set; }
    public int TamanoPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);
}
