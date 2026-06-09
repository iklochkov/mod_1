using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCRM.Infrastructure.Persistence;
using MyCRM.Domain.Constants;
using MyCRM.Application.DTOs;
using MyCRM.Domain.Entities;

namespace MyCRM.Infrastructure.Repositories
{
    public class DbTaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public DbTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTaskAsync(WorkTask task, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (task == null)
                return false;

            await _context.WorkTasks.AddAsync(task, token);
            await _context.SaveChangesAsync(token);

            return true;
        }


        public async Task<List<WorkTask>> GetAllTasksAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return await _context.WorkTasks
                .AsNoTracking()
                .Include(t => t.Deal)
                .ToListAsync(token);
        }

        public async Task<List<WorkTask>> GetTasksByDealIdAsync(int dealId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return await _context.WorkTasks
                .AsNoTracking()
                .Where(t => t.DealId == dealId)
                .Include(t => t.Deal)
                .ToListAsync(token);
        }

        public async Task<bool> ChangeTaskStatusAsync(int taskId, WorkTaskStatus newStatus, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var task = await _context.WorkTasks.FindAsync(new object[] { taskId }, token);
            if (task == null)
                return false;

            task.Status = newStatus;
            await _context.SaveChangesAsync(token);

            return true;
        }
    }
}