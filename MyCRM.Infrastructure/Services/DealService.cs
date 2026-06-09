using MyCRM.Infrastructure.Utils;
using MyCRM.Application.Interfaces;


using MyCRM.Application.DTOs;
using MyCRM.Domain.Constants;
using MyCRM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace MyCRM.Infrastructure.Services
{
    public class DealService : IDealService
    {
        private readonly IDealRepository _dealRepository;
        private readonly IClientRepository _clientRepository;

        public DealService(IDealRepository dealRepository, IClientRepository clientRepository)
        {
            _dealRepository = dealRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Deal?> AddDealAsync(string name, int amount, int count, DealStatus status, int clientId, DateTime deadline, string userId,
        CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            Console.WriteLine($"=== DealService.AddDealAsync ВХОД: userId = '{userId}' (длина: {userId?.Length}) ===");

            var client = await _clientRepository.GetUserByIdAsync(clientId, token);
            if (client == null)
                throw new ArgumentException($"Клиент с ID {clientId} не найден");

            Console.WriteLine($"=== ВЫЗЫВАЕМ КОНСТРУКТОР С {name}, {amount}, {count}, {status}, {clientId}, {deadline}, {userId} ===");

            var deal = new Deal(name, amount, count, status, clientId, deadline, userId);

            Console.WriteLine($"=== DealService.AddDealAsync перед репозиторием : userId = '{userId}' (длина: {userId?.Length}) ===");
            var result = await _dealRepository.AddDealInRepoAsync(deal, token);



            return result ? deal : null;
        }

        public async Task<List<Deal>> GetDealsByClientIdAsync(int clientId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _dealRepository.GetDealByUserIdAsync(clientId, token);
        }

        public async Task<bool> UpdateDealStatusAsync(int dealId, DealStatus newStatus, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _dealRepository.UpdateDealStatusAsync(dealId, newStatus, token);
        }

        public async Task<Dictionary<DealStatus, int>> GetDealStatisticsAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var analyticsService = new AnalyticsService(_clientRepository, _dealRepository, null);
            return await analyticsService.GetDealByStatusAsync(token);
        }

        public async Task<List<Deal>> GetOverdueDealsAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var analyticsService = new AnalyticsService(_clientRepository, _dealRepository, null);
            return await analyticsService.GetDealsOverdueAsync(token);
        }

        public Task<decimal> CalculateTotalAmountAsync(List<Deal> deals, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(DealCalculator.CalculateTotalAmount(deals));
        }

        public async Task<List<Deal>> GetAllDealsAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _dealRepository.GetAllDealsAsync(token);
        }

        public async Task<Deal?> GetDealByIdAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _dealRepository.GetDealByIdAsync(id, token);
        }

        public async Task<Deal?> UpdateDealAsync(int id, string name, int amount, int count, DealStatus status, DateTime deadline, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var deal = await _dealRepository.GetDealByIdAsync(id, token);
            if (deal == null) return null;

            deal.Name = name;
            deal.Amount = amount;
            deal.Count = count;
            deal.Status = status;
            deal.Deadline = deadline;

            var success = await _dealRepository.UpdateDealAsync(deal, token);
            return success ? deal : null;
        }

        public async Task<bool> DeleteDealAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _dealRepository.DeleteDealAsync(id, token);
        }


        public async Task<DealDto?> GetDealWithDetailsAsync(int id, CancellationToken token = default)
        {
            return await _dealRepository.GetDealWithDetailsAsync(id, token);
        }



    }
}
