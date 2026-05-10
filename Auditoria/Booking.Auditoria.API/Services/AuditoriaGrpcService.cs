using Booking.Auditoria.DataManagement.Entities;
using Booking.Auditoria.DataManagement.Interfaces;
using Booking.Auditoria.API.Protos;
using Grpc.Core;

namespace Booking.Auditoria.API.Services;

/// <summary>
/// Implementación del servidor gRPC de Auditoría.
/// Recibe eventos del Booking.Middleware y los persiste en log_auditoria.
/// No requiere JWT (la comunicación gRPC interna es de red privada o mTLS).
/// </summary>
public class AuditoriaGrpcService : Booking.Auditoria.API.Protos.AuditoriaGrpcService.AuditoriaGrpcServiceBase
{
    private readonly IAuditoriaLogRepository _repo;
    private readonly ILogger<AuditoriaGrpcService> _logger;

    public AuditoriaGrpcService(IAuditoriaLogRepository repo, ILogger<AuditoriaGrpcService> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    /// <summary>
    /// Registra un evento de auditoría recibido desde el Middleware FIFO.
    /// </summary>
    public override async Task<AuditoriaEventReply> RecordAuditEvent(
        AuditoriaEventRequest request,
        ServerCallContext context)
    {
        try
        {
            var log = new LogAuditoria
            {
                TablaAfectada     = request.TablaAfectada,
                EsquemaAfectado   = string.IsNullOrWhiteSpace(request.EsquemaAfectado)
                                        ? "booking"
                                        : request.EsquemaAfectado,
                Operacion         = request.Operacion.ToUpperInvariant(),
                IdRegistro        = string.IsNullOrWhiteSpace(request.IdRegistro)
                                        ? null : request.IdRegistro,
                DatosAnteriores   = string.IsNullOrWhiteSpace(request.DatosAnteriores)
                                        ? null : request.DatosAnteriores,
                DatosNuevos       = string.IsNullOrWhiteSpace(request.DatosNuevos)
                                        ? null : request.DatosNuevos,
                CreadoPorUsuario  = string.IsNullOrWhiteSpace(request.CreadoPorUsuario)
                                        ? null : request.CreadoPorUsuario,
                Ip                = string.IsNullOrWhiteSpace(request.Ip)
                                        ? null : request.Ip,
                ServicioOrigen    = string.IsNullOrWhiteSpace(request.ServicioOrigen)
                                        ? null : request.ServicioOrigen,
                EquipoOrigen      = string.IsNullOrWhiteSpace(request.EquipoOrigen)
                                        ? null : request.EquipoOrigen,
                FechaUtc          = DateTimeOffset.UtcNow
            };

            var id = await _repo.InsertAsync(log, context.CancellationToken);

            _logger.LogInformation(
                "[AuditoriaGrpc] Evento registrado: id_log={IdLog} tabla={Tabla} op={Op} servicio={Svc}",
                id, log.TablaAfectada, log.Operacion, log.ServicioOrigen);

            return new AuditoriaEventReply { Success = true, IdLog = id, Message = "OK" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AuditoriaGrpc] Error al registrar evento de auditoría");
            return new AuditoriaEventReply { Success = false, IdLog = 0, Message = ex.Message };
        }
    }

    /// <summary>
    /// Devuelve un registro de log por su id (uso interno entre microservicios).
    /// </summary>
    public override async Task<AuditoriaLogReply> GetAuditLogById(
        AuditoriaLogByIdRequest request,
        ServerCallContext context)
    {
        var dto = await _repo.GetByIdAsync(request.IdLog, context.CancellationToken);

        if (dto is null)
            return new AuditoriaLogReply { Found = false };

        return new AuditoriaLogReply
        {
            Found             = true,
            IdLog             = dto.IdLog,
            TablaAfectada     = dto.TablaAfectada,
            EsquemaAfectado   = dto.EsquemaAfectado,
            Operacion         = dto.Operacion,
            IdRegistro        = dto.IdRegistro ?? string.Empty,
            DatosAnteriores   = dto.DatosAnteriores?.ToString() ?? string.Empty,
            DatosNuevos       = dto.DatosNuevos?.ToString() ?? string.Empty,
            CreadoPorUsuario  = dto.CreadoPorUsuario ?? string.Empty,
            FechaUtc          = dto.FechaUtc.ToString("O"),
            Ip                = dto.Ip ?? string.Empty,
            ServicioOrigen    = dto.ServicioOrigen ?? string.Empty,
            EquipoOrigen      = dto.EquipoOrigen ?? string.Empty
        };
    }
}
