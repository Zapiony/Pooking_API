using Microservicio.Pooking.Servicio.Api.Extensions;
using Microservicio.Pooking.Servicio.Api.Middleware;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ── Servicios base ────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Configuraciones transversales ─────────────────────────────────────────
builder.Services.AddCustomApiVersioning();
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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pooking Servicio API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
