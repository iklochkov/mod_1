using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;
using MyCRM.Application.DTOs;
using System.Threading.Tasks;

namespace MyCRM.Application.Interfaces
{
    public interface IClientService
    {
        Task<Client?> AddClientAsync(string name, string surname, int age, string userId, CancellationToken token = default);
        Task<Client?> AddClientAsync(string name, string surname, int age, CancellationToken token = default);
        Task<List<Client>> GetAllClientsAsync(CancellationToken token = default);
        Task<Client?> GetClientByIdAsync(int id, CancellationToken token = default);
        Task<List<ClientTopDto>> GetTopClientsAsync(int count, CancellationToken token = default);
        Task<int> GetClientsCountAsync(CancellationToken token = default);
        Task<bool> ClientExistsAsync(int id, CancellationToken token = default);
        Task<PagedResult<Client>> GetPagedClientsAsync(
            int page,
            int pageSize,
            string? name = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            string sortBy = "Id",
            string sortOrder = "asc",
            CancellationToken token = default);
        Task<Client?> UpdateClientAsync(int id, string name, string surname, int age, CancellationToken token = default);
        Task<bool> DeleteClientAsync(int id, CancellationToken token = default);

        Task<bool> IncrementViewCountAsync(int id, CancellationToken token = default);

        Task<bool> UpdateAvatarAsync(int id, string avatarPath, CancellationToken token = default);

    }
}