using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Application.DTOs;
using MyCRM.Api.Services;
using MyCRM.Application.Commands.Clients;
using MediatR;

using MyCRM.Application.Queries;

namespace MyCRM.Api.Controllers;

[ApiController]
[Route("api/v1/clients")]
[Produces("application/json")]
[Authorize] 
public class ClientsV1Controller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientWebService _clientService;
    private readonly ILogger<ClientsV1Controller> _logger;

    public ClientsV1Controller( IMediator mediator, IClientWebService clientService, ILogger<ClientsV1Controller> logger)
    {
        _mediator = mediator;
        _clientService = clientService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientCommand command, CancellationToken token)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        _logger.LogInformation($"userId из токена: {userId}");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        command.UserId = userId;

        var result = await _mediator.Send(command, token);
        return CreatedAtAction(nameof(GetById), new { id = result?.Id }, result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "page", "pageSize", "name", "createdFrom", "createdTo", "sortBy", "sortOrder" })]
    public async Task<ActionResult<PagedResponse<ClientDto>>> GetAll(
        [FromQuery] GetAllClientQuery query,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(query, token);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(int id, CancellationToken token)
    {
        var query = new GetClientQuery(id);
        var result = await _mediator.Send( query, token);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> Update(
        int id,
        [FromBody] UpdateClientCommand command,
        CancellationToken token)
    {
        command.Id = id;

        var result = await _mediator.Send(command, token);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        var request = new DeleteClientCommand(id);

        var deleted = await _mediator.Send(request, token);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/avatar")]
    [RequestSizeLimit(2_000_000)]
    public async Task<IActionResult> UploadAvatar(int id, IFormFile file, CancellationToken token)
    {
        var result = await _clientService.UploadAvatarAsync(id, file, token);

        if(result == null)
        {
            return NotFound(new { error = "Клиент не найден" });
        }

          return Ok(result);
    }
}