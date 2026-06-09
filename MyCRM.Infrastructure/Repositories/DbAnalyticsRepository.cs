using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCRM.Infrastructure.Persistence;
using MyCRM.Application.DTOs;


namespace MyCRM.Infrastructure.Repositories
{
    public class DbAnalyticsRepository : IAnalyticsRepository
    {
        private readonly ApplicationDbContext _context;

        public DbAnalyticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken token = default)
        {
            var sql = @"
            SELECT 
                COUNT(*) AS TotalClients,
                (SELECT COUNT(*) FROM ""Deals"") AS TotalDeals,
                (SELECT COALESCE(SUM(""Amount""), 0) FROM ""Deals"") AS TotalAmount,
                (SELECT AVG(""Amount"") FROM ""Deals"") AS AvgDealAmount,
                (SELECT COUNT(*) FROM ""Deals"" WHERE ""Status"" = 'Won') AS WonDeals,
                (SELECT COUNT(*) FROM ""Deals"" WHERE ""Status"" = 'Lost') AS LostDeals
            FROM ""Clients""
        ";

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            await _context.Database.OpenConnectionAsync(token);
            using var reader = await command.ExecuteReaderAsync(token);

            if (await reader.ReadAsync(token))
            {
                return new AnalyticsSummaryDto
                {
                    TotalClients = reader.GetInt32(0),
                    TotalDeals = reader.GetInt32(1),
                    TotalAmount = reader.GetDecimal(2),
                    AvgDealAmount = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                    WonDeals = reader.GetInt32(4),
                    LostDeals = reader.GetInt32(5)
                };
            }

            return new AnalyticsSummaryDto();
        }
    }
}