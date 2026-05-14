using Microservicio.Pooking.Servicio.DataAccess.Queries;
using Microservicio.Pooking.Servicio.DataAccess.Repositories.Interfaces;

namespace Microservicio.Pooking.Servicio.DataManagement.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IServicioRepository ServicioRepository { get; }
    ITipoServicioRepository TipoServicioRepository { get; }
    ServicioQueryRepository ServicioQueryRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
