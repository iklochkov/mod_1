using MyCRM.Application.DTOs;
namespace MyCRM.Application.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken token = default);
}
