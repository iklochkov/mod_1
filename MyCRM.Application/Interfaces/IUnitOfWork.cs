using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Application.Interfaces
{
public interface IUnitOfWork : IDisposable
{
    IDealRepository Deals { get; }
    ITaskRepository Tasks { get; }
    
    Task<int> SaveChangesAsync(CancellationToken token = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
}