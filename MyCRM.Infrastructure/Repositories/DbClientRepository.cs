using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Application.DTOs;
using MyCRM.Domain.Entities;
using MyCRM.Infrastructure.Persistence;

namespace MyCRM.Infrastructure.Repositories
{
    public class DbClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public DbClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClientTopDto>> GetTopClientsRawAsync(int count, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var sql = @"
                SELECT 
                    c.""Id"" AS Id, 
                    (c.""Name"" || ' ' || c.""Surname"") AS FullName, 
                    COALESCE(SUM(d.""Total""), 0) AS Amount
                FROM ""Clients"" c
                LEFT JOIN ""Deals"" d ON c.""Id"" = d.""ClientId""
                GROUP BY c.""Id"", c.""Name"", c.""Surname""
                ORDER BY Amount DESC
                LIMIT {0}";

            return await _context.Database
                .SqlQueryRaw<ClientTopDto>(sql, count)
                .ToListAsync(token);
        }

        // ========== ОСНОВНЫЕ МЕТОДЫ (РАБОЧИЕ) ==========

        public async Task<bool> AddUserInRepoAsync(Client client, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            await _context.Clients.AddAsync(client, token);
            await _context.SaveChangesAsync(token);
            return true;
        }

        public async Task<List<Client>> GetAllClientAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _context.Clients
                .AsNoTracking()
                .Include(c => c.Contacts)
                .Include(c => c.Deals)
                .ToListAsync(token);
        }

        public async Task<Client?> GetUserByIdAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return await _context.Clients
                .Include(c => c.Contacts)
                .Include(c => c.Deals)
                .FirstOrDefaultAsync(c => c.Id == id, token);
        }

        public async Task<(List<Client> Items, int TotalCount)> GetPagedClientsAsync(
    int page,
    int pageSize,
    string? name = null,
    DateTime? createdFrom = null,
    DateTime? createdTo = null,
    string sortBy = "Id",
    string sortOrder = "asc",
    CancellationToken token = default)
        {
            var query = _context.Clients.AsQueryable();

            // Фильтрация по имени (частичное совпадение)
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Name.Contains(name));

            // Фильтрация по дате создания
            if (createdFrom.HasValue)
            {
                var from = DateTime.SpecifyKind(createdFrom.Value, DateTimeKind.Utc);
                query = query.Where(c => c.CreatedAt >= from);
            }

            if (createdTo.HasValue)
            {
                var to = DateTime.SpecifyKind(createdTo.Value, DateTimeKind.Utc);
                query = query.Where(c => c.CreatedAt <= to);
            }

            // Сортировка
            var desc = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);
            query = sortBy.ToLower() switch
            {
                "name" => desc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "age" => desc ? query.OrderByDescending(c => c.Age) : query.OrderBy(c => c.Age),
                "createdat" => desc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                _ => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };

            var totalCount = await query.CountAsync(token);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token);

            return (items, totalCount);
        }

        public async Task<bool> UpdateClientAsync(Client client, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (client == null)
                return false;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> DeleteClientAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var client = await _context.Clients.FindAsync(id, token);
            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> IncrementViewCountAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var rowsAffected = await _context.Database
                .ExecuteSqlRawAsync(
                    "UPDATE \"Clients\" SET \"ViewCount\" = \"ViewCount\" + 1 WHERE \"Id\" = {0}",
                    id);

            return rowsAffected > 0;
        }
    }
}