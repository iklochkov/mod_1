using MyCRM.Infrastructure.Cache;
using MyCRM.Domain.Builders;
using MyCRM.Infrastructure.Cache;
using MyCRM.Application.DTOs;
using MyCRM.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;

namespace MyCRM.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IHotClientsCache _cache;
        private readonly ILogger<ClientService> _logger;

        public ClientService(
            IClientRepository clientRepository,
            IDealRepository dealRepository,
            IHotClientsCache cache,
            ILogger<ClientService> logger)
        {
            _clientRepository = clientRepository;
            _dealRepository = dealRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Client?> AddClientAsync(string name, string surname, int age, string userId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            Console.WriteLine($"=== DEBUG: userId из токена из консольного сервиса = '{userId}' ===");


            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Попытка добавить клиента с пустым именем");
                throw new ArgumentException("Имя не может быть пустым");
            }

            if (string.IsNullOrWhiteSpace(surname))
            {
                _logger.LogWarning("Попытка добавить клиента с пустой фамилией");
                throw new ArgumentException("Фамилия не может быть пустой");
            }

            if (age <= 0 || age > 120)
            {
                _logger.LogWarning("Попытка добавить клиента с неверным возрастом: {Age}", age);
                throw new ArgumentException("Возраст должен быть в диапазоне 1-120");
            }

            var client = new ClientBuilder()
                .SetName(name)
                .SetSurname(surname)
                .SetAge(age)
                .SetCreatedByUserId(userId)
                .Build();

            var result = await _clientRepository.AddUserInRepoAsync(client, token);

            if (result)
            {
                _logger.LogInformation("Добавлен клиент: ID={Id}, Имя={Name}", client.Id, client.Name);
                _cache.AddOrUpdate(client);
            }
            else
            {
                _logger.LogError("Ошибка при добавлении клиента");
            }

            return result ? client : null;
        }

        public async Task<Client?> AddClientAsync(string name, string surname, int age, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Попытка добавить клиента с пустым именем");
                throw new ArgumentException("Имя не может быть пустым");
            }

            if (string.IsNullOrWhiteSpace(surname))
            {
                _logger.LogWarning("Попытка добавить клиента с пустой фамилией");
                throw new ArgumentException("Фамилия не может быть пустой");
            }

            if (age <= 0 || age > 120)
            {
                _logger.LogWarning("Попытка добавить клиента с неверным возрастом: {Age}", age);
                throw new ArgumentException("Возраст должен быть в диапазоне 1-120");
            }

            var client = new ClientBuilder()
                .SetName(name)
                .SetSurname(surname)
                .SetAge(age)
                .Build();

            var result = await _clientRepository.AddUserInRepoAsync(client, token);

            if (result)
            {
                _logger.LogInformation("Добавлен клиент: ID={Id}, Имя={Name}", client.Id, client.Name);
                _cache.AddOrUpdate(client);
            }
            else
            {
                _logger.LogError("Ошибка при добавлении клиента");
            }

            return result ? client : null;
        }

        public async Task<List<Client>> GetAllClientsAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _clientRepository.GetAllClientAsync(token);
        }

        public async Task<Client?> GetClientByIdAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var cached = _cache.GetClient(id);
            if (cached != null)
            {
                _logger.LogDebug("Клиент {Id} взят из кэша", id);
                return cached;
            }

            var client = await _clientRepository.GetUserByIdAsync(id, token);

            if (client != null)
            {
                _cache.AddOrUpdate(client);
                _logger.LogDebug("Клиент {Id} добавлен в кэш", id);
            }

            return client;
        }

        public async Task<List<ClientTopDto>> GetTopClientsAsync(int count, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var analyticsService = new AnalyticsService(_clientRepository, _dealRepository, null);
            return await analyticsService.GetTopClientsAsync(count, token);
        }

        public async Task<int> GetClientsCountAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var clients = await _clientRepository.GetAllClientAsync(token);
            return clients.Count;
        }

        public async Task<bool> ClientExistsAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var client = await _clientRepository.GetUserByIdAsync(id, token);
            return client != null;
        }

        public async Task<PagedResult<Client>> GetPagedClientsAsync(
            int page,
            int pageSize,
            string? name = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            string sortBy = "Id",
            string sortOrder = "asc",
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var (items, totalCount) = await _clientRepository.GetPagedClientsAsync(
                page, pageSize, name, createdFrom, createdTo, sortBy, sortOrder, token);

            return new PagedResult<Client>(items, page, pageSize, totalCount);
        }

        public async Task<Client?> UpdateClientAsync(int id, string name, string surname, int age, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var existingClient = await _clientRepository.GetUserByIdAsync(id, token);
            if (existingClient == null)
                return null;

            existingClient.Name = name;
            existingClient.Surname = surname;
            existingClient.Age = age;

            var result = await _clientRepository.UpdateClientAsync(existingClient, token);

            if (result)
            {
                _cache.AddOrUpdate(existingClient);
                _logger.LogInformation("Клиент {Id} обновлён", id);
                return existingClient;
            }

            return null;
        }

        public async Task<bool> DeleteClientAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var existingClient = await _clientRepository.GetUserByIdAsync(id, token);
            if (existingClient == null)
                return false;

            var result = await _clientRepository.DeleteClientAsync(id, token);

            return result;
        }

        public async Task<bool> IncrementViewCountAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var result = await _clientRepository.IncrementViewCountAsync(id, token);

            if (result)
            {
                var cached = _cache.GetClient(id);
                if (cached != null)
                {
                    cached.IncrementViewCount();
                    _cache.AddOrUpdate(cached);
                }
            }

            return result;
        }

        public async Task<bool> UpdateAvatarAsync(int id, string avatarPath, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var client = await _clientRepository.GetUserByIdAsync(id, token);
            if (client == null)
                return false;

            client.AvatarPath = avatarPath;
            return await _clientRepository.UpdateClientAsync(client, token);
        }
    }
}