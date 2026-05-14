using Microservicio.Pooking.Servicio.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Servicio.DataAccess.Context;

public class ServicioDbContext : DbContext
{
    public ServicioDbContext(DbContextOptions<ServicioDbContext> options)
        : base(options) { }

    public DbSet<TipoServicioEntity> TiposServicio => Set<TipoServicioEntity>();
    public DbSet<ServicioEntity> Servicios => Set<ServicioEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica automáticamente todas las IEntityTypeConfiguration del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ServicioDbContext).Assembly
        );
    }
}
