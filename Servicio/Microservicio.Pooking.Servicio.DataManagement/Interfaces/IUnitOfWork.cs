using Microservicio.Pooking.Servicio.DataAcces.Queries;
using Microservicio.Pooking.Servicio.DataAcces.Repositories.Interfaces;

namespace Microservicio.Pooking.Servicio.DataManagement.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IServicioRepository ServicioRepository { get; }
    ITipoServicioRepository TipoServicioRepository { get; }
    ServicioQueryRepository ServicioQueryRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
