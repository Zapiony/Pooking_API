namespace Microservicio.Pooking.Servicio.Business.Mappers;

public static class RowVersionMapper
{
    public static string? ABase64(byte[]? rowVersion) =>
        rowVersion is { Length: > 0 } ? Convert.ToBase64String(rowVersion) : null;

}
