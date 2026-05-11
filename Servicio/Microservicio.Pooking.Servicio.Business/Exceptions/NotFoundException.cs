namespace Microservicio.Pooking.Servicio.Business.Exceptions;

public class NotFoundException : BusinessException
{
    public NotFoundException(string mensaje, string? codigo = null)
        : base(mensaje, codigo) { }
}
