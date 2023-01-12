using System;
using System.Threading.Tasks;
using System.Threading;

namespace Loyalty.API.DataAccess.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
