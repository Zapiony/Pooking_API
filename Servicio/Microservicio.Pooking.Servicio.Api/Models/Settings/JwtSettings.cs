namespace Microservicio.Pooking.Servicio.Api.Models.Settings;

public class JwtSettings
{
    public bool Enabled { get; set; } = false;
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
