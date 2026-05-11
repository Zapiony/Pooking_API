namespace Microservicio.Pooking.Servicio.Business.Mappers;

public static class RowVersionMapper
{
    public static string? ABase64(byte[]? rowVersion) =>
        rowVersion is { Length: > 0 } ? Convert.ToBase64String(rowVersion) : null;

    public static byte[]? DesdeBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return null;
        try { return Convert.FromBase64String(base64); }
        catch { return null; }
    }
}
