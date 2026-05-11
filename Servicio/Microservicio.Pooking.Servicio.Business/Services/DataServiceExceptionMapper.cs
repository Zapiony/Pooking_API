using Microservicio.Pooking.Servicio.Business.Exceptions;

namespace Microservicio.Pooking.Servicio.Business.Services;

internal static class DataServiceExceptionMapper
{
    public static void PropagarSiInvalidOperation(InvalidOperationException ex)
    {
        if (ex.Message.Contains("No se encontró", StringComparison.OrdinalIgnoreCase))
            throw new NotFoundException(ex.Message);
        throw new ValidationException(ex.Message);
    }
}
