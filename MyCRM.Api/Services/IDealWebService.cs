using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Application.DTOs;
using System;

namespace MyCRM.Api.Services
{
    public interface IDealWebService
    {
    Task<DealDto?> AddDealAsync(DealCreateDto dto, string userId, CancellationToken token = default);

    Task<DealDto?> AddDealAsync(DealCreateDto dto, CancellationToken token = default);
    Task<List<DealDto>> GetAllDealsAsync(CancellationToken token = default);
    Task<DealDto?> GetDealByIdAsync(int id, CancellationToken token = default);
    Task<DealDto?> GetDealWithDetailsAsync(int id, CancellationToken token = default);
    Task<DealDto?> UpdateDealAsync(int id, DealUpdateDto dto, CancellationToken token = default);
    Task<bool> DeleteDealAsync(int id, CancellationToken token = default);

    }
}