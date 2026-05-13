using System.Net;
using System.Text.Json;

namespace Booking.Auditoria.API.Middleware;

/// <summary>
/// Middleware global de manejo de excepciones.
/// Convierte excepciones no controladas en respuestas JSON estandarizadas.
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada en {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;

            var error = new
            {
                status  = 500,
                error   = "Error interno del servidor.",
                detalle = ex.Message   // solo en Development; en Prod omitir
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(error, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}
