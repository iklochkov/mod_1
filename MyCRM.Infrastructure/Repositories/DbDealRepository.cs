using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Constants;
using MyCRM.Application.DTOs;
using MyCRM.Domain.Entities;
using MyCRM.Infrastructure.Persistence;

namespace MyCRM.Infrastructure.Repositories
{
    public class DbDealRepository : IDealRepository
    {
        private readonly ApplicationDbContext _context;

        public DbDealRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public event Action<int, DealStatus, DealStatus>? OnDealStatusChanged;

        public async Task<Dictionary<DealStatus, int>> GetDealStatusStatsRawAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var sql = @"
                SELECT 
                    d.""Status"" AS Status, 
                    COUNT(*) AS Count
                FROM ""Deals"" d
                GROUP BY d.""Status""
                ORDER BY Count DESC";

            var result = await _context.Database
                .SqlQueryRaw<DealStatusStatDto>(sql)
                .ToListAsync(token);

            return result.ToDictionary(r => Enum.Parse<DealStatus>(r.Status), r => r.Count);
        }

        public async Task<List<Deal>> GetOverdueDealsRawAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var sql = @"
                SELECT d.*
                FROM ""Deals"" d
                WHERE d.""Deadline"" < NOW() 
                  AND d.""Status"" != 'Won' 
                  AND d.""Status"" != 'Lost'
                ORDER BY d.""CreatedAt"" ASC";

            return await _context.Deals
                .FromSqlRaw(sql)
                .ToListAsync(token);
        }


        public async Task<bool> AddDealInRepoAsync(Deal deal, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (deal == null)
                return false;

            Console.WriteLine($"=== РЕПОЗИТОРИЙ: CreatedByUserId = '{deal.CreatedByUserId}' ===");

            // Приводим даты к UTC
            deal.CreatedAt = DateTime.UtcNow;
            if (deal.Deadline.Kind != DateTimeKind.Utc)
            {
                deal.Deadline = DateTime.SpecifyKind(deal.Deadline, DateTimeKind.Utc);
            }

            await _context.Deals.AddAsync(deal, token);

            Console.WriteLine($"=== РЕПОЗИТОРИЙ ПЕРЕД SAVE: CreatedByUserId = '{deal.CreatedByUserId}' ===");

            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<List<Deal>> GetDealByUserIdAsync(int clientId, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _context.Deals
                .Where(d => d.ClientId == clientId)
                .Include(d => d.Tasks)
                .ToListAsync(token);
        }

        public async Task<List<Deal>> GetAllDealsAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _context.Deals
                .AsNoTracking()
                .Include(d => d.Client)
                .Include(d => d.Tasks)
                .ToListAsync(token);
        }

        public async Task<bool> UpdateDealStatusAsync(int dealId, DealStatus newStatus, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var deal = await _context.Deals.FindAsync(dealId, token);
            if (deal == null) return false;

            var oldStatus = deal.Status;
            deal.Status = newStatus;
            await _context.SaveChangesAsync(token);

            if (newStatus == DealStatus.Won)
            {
                OnDealStatusChanged?.Invoke(dealId, oldStatus, newStatus);
            }
            return true;
        }


        public async Task<Deal?> GetDealByIdAsync(int id, CancellationToken token = default)
        {
            return await _context.Deals
                .FirstOrDefaultAsync(d => d.Id == id, token);
        }

        public async Task<DealDto?> GetDealWithDetailsAsync(int id, CancellationToken token = default)
        {
            return await _context.Deals
                .Where(d => d.Id == id)
                .Select(d => new DealDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Amount = d.Amount,
                    Count = d.Count,
                    Total = d.Amount * d.Count,
                    Status = d.Status.ToString(),
                    ClientId = d.ClientId,
                    ClientName = d.Client != null ? d.Client.Name + " " + d.Client.Surname : "Unknown",
                    CreatedAt = d.CreatedAt,
                    Deadline = d.Deadline
                })
                .FirstOrDefaultAsync(token);
        }

        public async Task<bool> UpdateDealAsync(Deal deal, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (deal == null) return false;

            _context.Deals.Update(deal);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> DeleteDealAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var deal = await _context.Deals.FindAsync(new object[] { id }, token);
            if (deal == null) return false;

            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync(token);

            return true;
        }

    }


}