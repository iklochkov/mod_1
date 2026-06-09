using System;
using System.Collections.Generic;
using System.Linq;
using MyCRM.Domain.Entities;
using MyCRM.Domain.Constants;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Interfaces
{
    public interface IDealRepository
    {

        event Action<int, DealStatus, DealStatus> OnDealStatusChanged;
        Task<bool> AddDealInRepoAsync(Deal deal, CancellationToken token = default);
        Task<List<Deal>> GetDealByUserIdAsync(int clientId, CancellationToken token = default);
        Task<List<Deal>> GetAllDealsAsync(CancellationToken token = default);
        Task<bool> UpdateDealStatusAsync(int dealId, DealStatus newStatus, CancellationToken token = default);
        Task<Dictionary<DealStatus, int>> GetDealStatusStatsRawAsync(CancellationToken token = default);
        Task<List<Deal>> GetOverdueDealsRawAsync(CancellationToken token = default);

        Task<Deal?> GetDealByIdAsync(int id, CancellationToken token = default);

        Task<bool> UpdateDealAsync(Deal deal, CancellationToken token = default);
        Task<bool> DeleteDealAsync(int id, CancellationToken token = default);

        Task<DealDto?> GetDealWithDetailsAsync(int id, CancellationToken token = default);
    }
}