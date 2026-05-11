using Microservicio.Pooking.Servicio.DataAcces.Context;
using Microservicio.Pooking.Servicio.DataAcces.Queries;
using Microservicio.Pooking.Servicio.DataAcces.Repositories;
using Microservicio.Pooking.Servicio.DataAcces.Repositories.Interfaces;
using Microservicio.Pooking.Servicio.DataManagement.Interfaces;

namespace Microservicio.Pooking.Servicio.DataManagement.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ServicioDbContext _context;

    public IServicioRepository ServicioRepository { get; }
    public ITipoServicioRepository TipoServicioRepository { get; }
    public ServicioQueryRepository ServicioQueryRepository { get; }

    public UnitOfWork(ServicioDbContext context)
    {
        _context = context;
        ServicioRepository = new ServicioRepository(_context);
        TipoServicioRepository = new TipoServicioRepository(_context);
        ServicioQueryRepository = new ServicioQueryRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
