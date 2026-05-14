using Microsoft.OpenApi;

namespace Microservicio.Pooking.Servicio.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "Pooking — Módulo Servicio API",
                Version = "v2",
                Description = "Catálogo de servicios integrables en la plataforma Pooking. " +
                              "Gestiona tipos de servicio (Vuelos, Alojamiento, Atracciones, " +
                              "Alquiler de Carros) y los proveedores registrados."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization. Ejemplo: Bearer {token}"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", document, null),
                    []
                }
            });
        });

        return services;
    }
}
