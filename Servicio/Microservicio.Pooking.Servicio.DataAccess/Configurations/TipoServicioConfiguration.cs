using Microservicio.Pooking.Servicio.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Pooking.Servicio.DataAccess.Configurations;

public class TipoServicioConfiguration : IEntityTypeConfiguration<TipoServicioEntity>
{
    public void Configure(EntityTypeBuilder<TipoServicioEntity> builder)
    {
        builder.ToTable("tipo_servicio", "booking", t =>
        {
            t.HasComment("Catalogo cerrado de categorias de servicio; nuevos valores requieren migracion controlada");
            t.HasCheckConstraint(
                "chk_tipo_servicio_nombre",
                "nombre IN ('Vuelos', 'Alojamiento', 'Atracciones', 'Alquiler de Carros')");
            t.HasCheckConstraint(
                "chk_tipo_servicio_estado",
                "estado IN ('ACT', 'INA')");
        });

        // ── Identificación ────────────────────────────────────────────────
        builder.HasKey(ts => ts.IdTipoServicio)
               .HasName("pk_tipo_servicio");

        builder.Property(ts => ts.IdTipoServicio)
               .HasColumnName("id_tipo_servicio")
               .UseIdentityColumn();

        builder.Property(ts => ts.GuidTipoServicio)
               .HasColumnName("guid_tipo_servicio")
               .HasDefaultValueSql("gen_random_uuid()")
               .IsRequired();

        builder.HasIndex(ts => ts.GuidTipoServicio)
               .IsUnique()
               .HasDatabaseName("uq_tipo_servicio_guid");

        // ── Datos funcionales ─────────────────────────────────────────────
        builder.Property(ts => ts.Nombre)
               .HasColumnName("nombre")
               .HasMaxLength(60)
               .HasComment("Valores permitidos: Vuelos | Alojamiento | Atracciones | Alquiler de Carros")
               .IsRequired();

        builder.HasIndex(ts => ts.Nombre)
               .IsUnique()
               .HasDatabaseName("uq_tipo_servicio_nombre");

        builder.Property(ts => ts.Descripcion)
               .HasColumnName("descripcion")
               .HasMaxLength(500)
               .IsRequired(false);

        // ── Estado ────────────────────────────────────────────────────────
        builder.Property(ts => ts.Estado)
               .HasColumnName("estado")
               .HasColumnType("char(3)")
               .HasDefaultValue("ACT")
               .IsRequired();

        builder.HasIndex(ts => ts.Estado)
               .HasDatabaseName("idx_tipo_servicio_estado");

        builder.Property(ts => ts.EsEliminado)
               .HasColumnName("es_eliminado")
               .HasDefaultValue(false)
               .IsRequired();

        // ── Auditoría ─────────────────────────────────────────────────────
        builder.Property(ts => ts.CreadoPorUsuario)
               .HasColumnName("creado_por_usuario")
               .HasMaxLength(150)
               .IsRequired(false);

        builder.Property(ts => ts.FechaRegistroUtc)
               .HasColumnName("fecha_registro_utc")
               .HasColumnType("timestamp with time zone")
               .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')")
               .IsRequired();

        builder.Property(ts => ts.ModificadoPorUsuario)
               .HasColumnName("modificado_por_usuario")
               .HasMaxLength(150)
               .IsRequired(false);

        builder.Property(ts => ts.FechaModificacionUtc)
               .HasColumnName("fecha_modificacion_utc")
               .HasColumnType("timestamp with time zone")
               .IsRequired(false);

        builder.Property(ts => ts.ModificacionIp)
               .HasColumnName("modificacion_ip")
               .HasMaxLength(45)
               .IsRequired(false);

        builder.Property(ts => ts.ServicioOrigen)
               .HasColumnName("servicio_origen")
               .HasMaxLength(100)
               .IsRequired(false);

        // ── Concurrencia optimista (PostgreSQL xmin) ──────────────────────
        builder.Property<uint>("xmin")
               .HasColumnName("xmin")
               .HasColumnType("xid")
               .ValueGeneratedOnAddOrUpdate()
               .IsConcurrencyToken();

        // ── Navegación ────────────────────────────────────────────────────
        builder.HasMany(ts => ts.Servicios)
               .WithOne(s => s.TipoServicio)
               .HasForeignKey(s => s.IdTipoServicio)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
