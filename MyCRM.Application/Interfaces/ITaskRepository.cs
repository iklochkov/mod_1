using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;
using MyCRM.Domain.Constants;



namespace MyCRM.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<bool> AddTaskAsync(WorkTask task, CancellationToken token = default);
        Task<List<WorkTask>> GetAllTasksAsync(CancellationToken token = default);
        Task<List<WorkTask>> GetTasksByDealIdAsync(int dealId, CancellationToken token = default);
        Task<bool> ChangeTaskStatusAsync(int taskId, WorkTaskStatus newStatus, CancellationToken token = default);
    }
}