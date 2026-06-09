using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Application.Interfaces;
using MyCRM.Infrastructure.Persistence;
using MyCRM.Infrastructure.Repositories;

namespace MyCRM.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDealRepository? _deals;
        private ITaskRepository? _tasks;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IDealRepository Deals => _deals ??= new DbDealRepository(_context);
        public ITaskRepository Tasks => _tasks ??= new DbTaskRepository(_context);

        public async Task BeginTransactionAsync()
        {
             await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken token = default)
        {
           return await _context.SaveChangesAsync(token);
        }
    }
}