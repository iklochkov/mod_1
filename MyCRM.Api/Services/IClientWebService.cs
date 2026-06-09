using AutoMapper;
using MyCRM.Application.Interfaces;
using MyCRM.Application.DTOs;
using MyCRM.Api.Services;

namespace MyCRM.Api.Services
{
    public interface IClientWebService
{
    Task<ClientDto?> AddClientAsync(ClientCreateDto dto, string userId, CancellationToken token = default);
    Task<ClientDto?> GetClientByIdAsync(int id, CancellationToken token = default);
    Task<PagedResponse<ClientDto>> GetPagedClientsAsync(
        int page, 
        int pageSize, 
        string? name = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        string sortBy = "Id",
        string sortOrder = "asc",
        CancellationToken token = default);
    Task<ClientDto?> UpdateClientAsync(int id, ClientUpdateDto dto, CancellationToken token = default);
    Task<bool> DeleteClientAsync(int id, CancellationToken token = default);

   Task<ClientAvatarDto?> UploadAvatarAsync(int id, IFormFile file, CancellationToken token);
}
}