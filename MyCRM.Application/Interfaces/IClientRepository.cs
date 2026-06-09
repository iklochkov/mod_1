using MyCRM.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using MyCRM.Domain.Entities;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Interfaces
{
    public interface IClientRepository
    {
        Task<bool> AddUserInRepoAsync(Client client, CancellationToken token = default);
        Task<List<Client>> GetAllClientAsync(CancellationToken token = default);
        Task<Client?> GetUserByIdAsync(int id, CancellationToken token = default);

        Task<List<ClientTopDto>> GetTopClientsRawAsync(int count, CancellationToken token = default);

        Task<(List<Client> Items, int TotalCount)> GetPagedClientsAsync(
    int page,
    int pageSize,
    string? name = null,
    DateTime? createdFrom = null,
    DateTime? createdTo = null,
    string sortBy = "Id",
    string sortOrder = "asc",
    CancellationToken token = default);
        Task<bool> UpdateClientAsync(Client client, CancellationToken token = default);
        Task<bool> DeleteClientAsync(int id, CancellationToken token = default);
        Task<bool> IncrementViewCountAsync(int id, CancellationToken token = default);
    }
}