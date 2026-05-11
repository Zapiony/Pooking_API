using Microservicio.Pooking.Servicio.DataAcces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Pooking.Servicio.DataAcces.Configurations;

public class ServicioConfiguration : IEntityTypeConfiguration<ServicioEntity>
{
    public void Configure(EntityTypeBuilder<ServicioEntity> builder)
    {
        builder.ToTable("servicio", "booking", t =>
        {
            t.HasComment("Proveedores registrados; guid_servicio es la referencia publica consumida por otros dominios");
            t.HasCheckConstraint(
                "chk_servicio_tipo_id",
                "tipo_identificacion IN ('RUC', 'CI', 'PASS', 'EXT')");
            t.HasCheckConstraint(
                "chk_servicio_estado",
                "estado IN ('ACT', 'INA', 'SUS')");
        });

        // ── Identificación ────────────────────────────────────────────────
        builder.HasKey(s => s.IdServicio)
               .HasName("pk_servicio");

        builder.Property(s => s.IdServicio)
               .HasColumnName("id_servicio")
               .UseIdentityColumn();

        builder.Property(s => s.GuidServicio)
               .HasColumnName("guid_servicio")
               .HasDefaultValueSql("gen_random_uuid()")
               .HasComment("UUID publico expuesto a dominios externos como referencia logica cross-dominio")
               .IsRequired();

        builder.HasIndex(s => s.GuidServicio)
               .IsUnique()
               .HasDatabaseName("uq_servicio_guid");

        // ── Relación con tipo ─────────────────────────────────────────────
        builder.Property(s => s.IdTipoServicio)
               .HasColumnName("id_tipo_servicio")
               .IsRequired();

        builder.HasIndex(s => s.IdTipoServicio)
               .HasDatabaseName("idx_servicio_tipo");

        // ── Datos del proveedor ───────────────────────────────────────────
        builder.Property(s => s.RazonSocial)
               .HasColumnName("razon_social")
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(s => s.RazonSocial)
               .HasDatabaseName("idx_servicio_razon_social");

        builder.Property(s => s.NombreComercial)
               .HasColumnName("nombre_comercial")
               .HasMaxLength(200)
               .IsRequired(false);

        builder.Property(s => s.TipoIdentificacion)
               .HasColumnName("tipo_identificacion")
               .HasMaxLength(10)
               .HasComment("RUC | CI | PASS | EXT")
               .IsRequired();

        builder.Property(s => s.NumeroIdentificacion)
               .HasColumnName("numero_identificacion")
               .HasMaxLength(20)
               .IsRequired();

        builder.HasIndex(s => new { s.TipoIdentificacion, s.NumeroIdentificacion })
               .IsUnique()
               .HasDatabaseName("uq_servicio_numero_id");

        builder.Property(s => s.CorreoContacto)
               .HasColumnName("correo_contacto")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(s => s.TelefonoContacto)
               .HasColumnName("telefono_contacto")
               .HasMaxLength(30)
               .IsRequired(false);

        builder.Property(s => s.Direccion)
               .HasColumnName("direccion")
               .HasMaxLength(500)
               .IsRequired(false);

        builder.Property(s => s.SitioWeb)
               .HasColumnName("sitio_web")
               .HasMaxLength(500)
               .IsRequired(false);

        builder.Property(s => s.LogoUrl)
               .HasColumnName("logo_url")
               .HasMaxLength(500)
               .IsRequired(false);

        // ── Estado ────────────────────────────────────────────────────────
        builder.Property(s => s.Estado)
               .HasColumnName("estado")
               .HasColumnType("char(3)")
               .HasDefaultValue("ACT")
               .HasComment("ACT=Activo | INA=Inactivo | SUS=Suspendido")
               .IsRequired();

        builder.HasIndex(s => s.Estado)
               .HasDatabaseName("idx_servicio_estado");

        builder.Property(s => s.EsEliminado)
               .HasColumnName("es_eliminado")
               .HasDefaultValue(false)
               .IsRequired();

        // ── Auditoría ─────────────────────────────────────────────────────
        builder.Property(s => s.CreadoPorUsuario)
               .HasColumnName("creado_por_usuario")
               .HasMaxLength(150)
               .IsRequired(false);

        builder.Property(s => s.FechaRegistroUtc)
               .HasColumnName("fecha_registro_utc")
               .HasColumnType("timestamp with time zone")
               .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')")
               .IsRequired();

        builder.Property(s => s.ModificadoPorUsuario)
               .HasColumnName("modificado_por_usuario")
               .HasMaxLength(150)
               .IsRequired(false);

        builder.Property(s => s.FechaModificacionUtc)
               .HasColumnName("fecha_modificacion_utc")
               .HasColumnType("timestamp with time zone")
               .IsRequired(false);

        builder.Property(s => s.ModificacionIp)
               .HasColumnName("modificacion_ip")
               .HasMaxLength(45)
               .IsRequired(false);

        builder.Property(s => s.ServicioOrigen)
               .HasColumnName("servicio_origen")
               .HasMaxLength(100)
               .IsRequired(false);

        // ── Concurrencia optimista (PostgreSQL xmin) ──────────────────────
        builder.Property<uint>("xmin")
               .HasColumnName("xmin")
               .HasColumnType("xid")
               .ValueGeneratedOnAddOrUpdate()
               .IsConcurrencyToken();

        // ── Relación FK ───────────────────────────────────────────────────
        builder.HasOne(s => s.TipoServicio)
               .WithMany(ts => ts.Servicios)
               .HasForeignKey(s => s.IdTipoServicio)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("fk_servicio_tipo_servicio");
    }
}
