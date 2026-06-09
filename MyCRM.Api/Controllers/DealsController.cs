using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using MyCRM.Application.Commands.Deals;
using MyCRM.Application.DTOs;
using MyCRM.Application.Queries;
using MyCRM.Infrastructure.Services;

namespace MyCRM.Api.Controllers;

[ApiController]
[Route("api/deals")]
[Authorize]
public class DealsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DealsController> _logger;
    private readonly NotificationService _notificationService;


    public DealsController(IMediator mediator, ILogger<DealsController> logger, NotificationService notificationService)
    {
        _mediator = mediator;
        _logger = logger;
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<ActionResult<DealDto>> Create([FromBody] CreateDealCommand command, CancellationToken token)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        command.UserId = userId;
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(nameof(GetById), new { id = result?.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<DealDto>>> GetAll([FromQuery] GetAllDealsQuery query, CancellationToken token)
    {
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DealDto>> GetById(int id, CancellationToken token)
    {
        var result = await _mediator.Send(new GetDealQuery { Id = id }, token);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DealDto>> Update(int id, [FromBody] UpdateDealCommand command, CancellationToken token)
    {
        command.Id = id;
        var result = await _mediator.Send(command, token);
        if (result == null)
        {
            return NotFound();
        } 
        if (result.Status == "Won" || result.Status == "Lost")
        {
            await _notificationService.SendDealNotification(result.Id, result.Name, result.Status);
        }
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        var deleted = await _mediator.Send(new DeleteDealCommand { Id = id }, token);
        if (!deleted) return NotFound();
        return NoContent();
    }

}
