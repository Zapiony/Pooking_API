namespace Microservicio.Pooking.Servicio.Business.Exceptions;

public class BusinessException : Exception
{
    public string? Codigo { get; }

    public BusinessException(string mensaje, string? codigo = null, Exception? interna = null)
        : base(mensaje, interna)
    {
        Codigo = codigo;
    }
}
