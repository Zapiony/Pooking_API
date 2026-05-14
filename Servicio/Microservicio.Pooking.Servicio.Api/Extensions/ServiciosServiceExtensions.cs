using Microservicio.Pooking.Servicio.Business.Interfaces;
using Microservicio.Pooking.Servicio.Business.Services;
using Microservicio.Pooking.Servicio.DataAccess.Context;
using Microservicio.Pooking.Servicio.DataManagement.Interfaces;
using Microservicio.Pooking.Servicio.DataManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Servicio.Api.Extensions;

/// <summary>
/// Registra todas las dependencias del módulo Servicio.
/// Se llama desde Program.cs con builder.Services.AddServicioModule(configuration).
/// </summary>
public static class ServiciosServiceExtensions
{
    public static IServiceCollection AddServicioModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Capa 1 — DbContext (PostgreSQL)
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Falta 'ConnectionStrings:Default' en la configuración. " +
                "Configura la variable de entorno o el archivo de configuración local.");

        services.AddDbContext<ServicioDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ServicioDbContext>());

        // Capa 2 — Unit of Work y servicios de datos
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IServicioDataService, ServicioDataService>();
        services.AddScoped<ITipoServicioDataService, TipoServicioDataService>();

        // Capa 3 — Servicios de negocio
        services.AddScoped<IServicioService, ServicioService>();
        services.AddScoped<ITipoServicioService, TipoServicioService>();

        return services;
    }
}
