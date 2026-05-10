using Booking.Auditoria.DataAccess.Context;
using Booking.Auditoria.DataAccess.Repositories;
using Booking.Auditoria.DataManagement.Interfaces;
using Booking.Auditoria.API.Middleware;
using Booking.Auditoria.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// 1. Base de datos — booking_auditoria  (variable DATABASE_URL)
// ─────────────────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration["DATABASE_URL"]
    ?? throw new InvalidOperationException("Variable DATABASE_URL no configurada.");

builder.Services.AddDbContext<AuditoriaDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// ─────────────────────────────────────────────────────────────────────────────
// 2. Repositorio (DI)
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuditoriaLogRepository, AuditoriaLogRepository>();

// ─────────────────────────────────────────────────────────────────────────────
// 3. Autenticación JWT Bearer
//    Clave compartida con TODAS las soluciones (JWT__SECRET).
//    Auditoria valida el token pero NO lo emite.
// ─────────────────────────────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["JWT__SECRET"]
    ?? throw new InvalidOperationException("Variable JWT__SECRET no configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer           = true,
            ValidIssuer              = "booking-auth",
            ValidateAudience         = true,
            ValidAudience            = "booking-api",
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero
        };
    });

// ─────────────────────────────────────────────────────────────────────────────
// 4. Autorización — políticas definidas en el Roadmap
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdmin",      p => p.RequireRole("ADMINISTRADOR"));
    options.AddPolicy("AdminOVendedor", p => p.RequireRole("ADMINISTRADOR", "VENDEDOR"));
    options.AddPolicy("Autenticado",    p => p.RequireAuthenticatedUser());
});

// ─────────────────────────────────────────────────────────────────────────────
// 5. Controladores + gRPC
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddGrpc();

// ─────────────────────────────────────────────────────────────────────────────
// 6. Swagger / OpenAPI
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Booking.Auditoria API",
        Version     = "v1",
        Description = "Microservicio de Auditoría — Booking Prototipo Reto 2. " +
                      "Puerto 5006. Base: 04_auditoria.sql."
    });

    // Soporte JWT en Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Introduce tu JWT: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────────────────────────────────────
// 7. Build
// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// Aplicar migraciones pendientes al arrancar (útil en Railway/Render)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuditoriaDbContext>();
    await db.Database.MigrateAsync();
}

// ─────────────────────────────────────────────────────────────────────────────
// 8. Pipeline HTTP
// ─────────────────────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking.Auditoria v1");
        c.RoutePrefix = string.Empty; // Swagger en raíz
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// gRPC endpoint para el Middleware FIFO
app.MapGrpcService<AuditoriaGrpcService>();

// Health-check mínimo
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Booking.Auditoria", port = 5006 }));

app.Run();
