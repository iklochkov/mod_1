using MyCRM.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Application.Interfaces;
using MyCRM.Infrastructure.Persistence;

namespace MyCRM.Api.Controllers;

[ApiController]
[Route("api/analytics")]
[Produces("application/json")]
[Authorize(Roles = "Admin")] 
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetSummary(CancellationToken token)
    {
        _logger.LogInformation("Запрос аналитики");
        var result = await _analyticsService.GetSummaryAsync(token);
        return Ok(result);
    }
}