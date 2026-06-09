using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyCRM.Domain.Constants;
using MyCRM.Application.DTOs;
using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;

namespace MyCRM.Infrastructure.Services
{
    public class AnalyticsService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsService(
            IClientRepository clientRepository,
            IDealRepository dealRepository,
            IAnalyticsRepository analyticsRepository)
        {
            _clientRepository = clientRepository;
            _dealRepository = dealRepository;
            _analyticsRepository = analyticsRepository;
        }

        public async Task<List<ClientTopDto>> GetTopClientsAsync(int count, CancellationToken token = default)
        {
            return await _clientRepository.GetTopClientsRawAsync(count, token);
        }

        public async Task<Dictionary<DealStatus, int>> GetDealByStatusAsync(CancellationToken token = default)
        {
            return await _dealRepository.GetDealStatusStatsRawAsync(token);
        }

        public async Task<List<Deal>> GetDealsOverdueAsync(CancellationToken token = default)
        {
            return await _dealRepository.GetOverdueDealsRawAsync(token);
        }

        public async Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken token = default)
        {
            return await _analyticsRepository.GetSummaryAsync(token);
        }
    }
}