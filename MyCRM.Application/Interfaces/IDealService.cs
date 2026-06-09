using MyCRM.Application.DTOs;
using MyCRM.Domain.Constants;
using MyCRM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;
using MyCRM.Domain.Constants;
using System.Threading.Tasks;

namespace MyCRM.Application.Interfaces
{
    public interface IDealService
    {
        Task<Deal?> AddDealAsync(string name, int amount, int count, DealStatus status, int clientId, DateTime deadline, string userId, CancellationToken token = default);
        Task<List<Deal>> GetDealsByClientIdAsync(int clientId, CancellationToken token = default);
        Task<bool> UpdateDealStatusAsync(int dealId, DealStatus newStatus, CancellationToken token = default);
        Task<Dictionary<DealStatus, int>> GetDealStatisticsAsync(CancellationToken token = default);
        Task<List<Deal>> GetOverdueDealsAsync(CancellationToken token = default);
        Task<decimal> CalculateTotalAmountAsync(List<Deal> deals, CancellationToken token = default);
        Task<List<Deal>> GetAllDealsAsync(CancellationToken token = default);
        Task<Deal?> UpdateDealAsync(int id, string name, int amount, int count, DealStatus status, DateTime deadline, CancellationToken token = default);
        Task<bool> DeleteDealAsync(int id, CancellationToken token = default);

        Task<Deal?> GetDealByIdAsync(int id, CancellationToken token = default);

        Task<DealDto?> GetDealWithDetailsAsync(int id, CancellationToken token = default);
    }
}