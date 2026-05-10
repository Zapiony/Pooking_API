using Booking.Auditoria.DataManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Auditoria.DataAccess.Context;

/// <summary>
/// DbContext del dominio Auditoría.
/// Apunta a la base de datos booking_auditoria (variable DATABASE_URL).
/// El esquema es 'booking' para coincidir con 04_auditoria.sql.
/// </summary>
public class AuditoriaDbContext : DbContext
{
    public AuditoriaDbContext(DbContextOptions<AuditoriaDbContext> options) : base(options) { }

    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("booking");

        modelBuilder.Entity<LogAuditoria>(entity =>
        {
            entity.ToTable("log_auditoria");

            entity.HasKey(e => e.IdLog);
            entity.Property(e => e.IdLog)
                  .HasColumnName("id_log")
                  .UseIdentityAlwaysColumn();   // BIGSERIAL en Postgres

            entity.Property(e => e.TablaAfectada)
                  .HasColumnName("tabla_afectada")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.EsquemaAfectado)
                  .HasColumnName("esquema_afectado")
                  .HasMaxLength(100)
                  .HasDefaultValue("booking")
                  .IsRequired();

            entity.Property(e => e.Operacion)
                  .HasColumnName("operacion")
                  .HasMaxLength(10)
                  .IsRequired();

            entity.Property(e => e.IdRegistro)
                  .HasColumnName("id_registro")
                  .HasColumnType("text");

            // JSONB se mapea como string en EF Core; el proveedor Npgsql gestiona la serialización
            entity.Property(e => e.DatosAnteriores)
                  .HasColumnName("datos_anteriores")
                  .HasColumnType("jsonb");

            entity.Property(e => e.DatosNuevos)
                  .HasColumnName("datos_nuevos")
                  .HasColumnType("jsonb");

            entity.Property(e => e.CreadoPorUsuario)
                  .HasColumnName("creado_por_usuario")
                  .HasMaxLength(150);

            entity.Property(e => e.FechaUtc)
                  .HasColumnName("fecha_utc")
                  .HasColumnType("timestamptz")
                  .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
                  .IsRequired();

            entity.Property(e => e.Ip)
                  .HasColumnName("ip")
                  .HasMaxLength(45);

            entity.Property(e => e.ServicioOrigen)
                  .HasColumnName("servicio_origen")
                  .HasMaxLength(100);

            entity.Property(e => e.EquipoOrigen)
                  .HasColumnName("equipo_origen")
                  .HasMaxLength(100);

            entity.Property(e => e.EsEliminadoLog)
                  .HasColumnName("es_eliminado_log")
                  .HasDefaultValue(false)
                  .IsRequired();

            // Índices definidos en 04_auditoria.sql
            entity.HasIndex(e => e.TablaAfectada).HasDatabaseName("idx_log_tabla");
            entity.HasIndex(e => e.Operacion).HasDatabaseName("idx_log_operacion");
            entity.HasIndex(e => e.FechaUtc).HasDatabaseName("idx_log_fecha");
            entity.HasIndex(e => e.CreadoPorUsuario).HasDatabaseName("idx_log_usuario");
        });
    }
}
