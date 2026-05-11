using Microservicio.Pooking.Servicio.Api.Models.Settings;

namespace Microservicio.Pooking.Servicio.Api.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "PookingServicioPolicy";

    public static IServiceCollection AddServicioCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("Cors").Get<CorsSettings>() ?? new CorsSettings();

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (settings.AllowedOrigins.Count > 0)
                    policy.WithOrigins(settings.AllowedOrigins.ToArray())
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                else
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
            });
        });

        return services;
    }
}
