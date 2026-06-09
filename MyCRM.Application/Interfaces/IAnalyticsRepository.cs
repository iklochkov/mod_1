using MyCRM.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Interfaces
{
    public interface IAnalyticsRepository
    {
        Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken token = default);
    }
}