using Microservicio.Pooking.Servicio.Api.Extensions;
using Microservicio.Pooking.Servicio.Api.Middleware;
using Microservicio.Pooking.Servicio.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ── Servicios base ────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var esGuidInvalido = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Any(e =>
                string.Equals(e.Key, "guid", StringComparison.OrdinalIgnoreCase) ||
                e.Value!.Errors.Any(error =>
                    error.Exception is FormatException ||
                    error.ErrorMessage.Contains("Guid", StringComparison.OrdinalIgnoreCase)));

        if (esGuidInvalido)
        {
            return new BadRequestObjectResult(ApiErrorResponse.Crear(
                "El identificador proporcionado no tiene un formato válido.",
                Array.Empty<string>()));
        }

        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage))
            .ToList();

        return new BadRequestObjectResult(ApiErrorResponse.Crear(
            "Datos de entrada inválidos.",
            errors));
    };
});

// ── Configuraciones transversales ─────────────────────────────────────────
builder.Services.AddServicioCors(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomSwagger();
builder.Services.AddAuthorization();

// ── Módulo Servicio (DbContext + DataManagement + Business) ────────────────
builder.Services.AddServicioModule(builder.Configuration);

// ── Pipeline HTTP ─────────────────────────────────────────────────────────
var app = builder.Build();

// Middleware global de errores — debe ser el primero del pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Pooking Servicio API v2");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
