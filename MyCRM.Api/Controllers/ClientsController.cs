using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Application.DTOs;
using MyCRM.Api.Services;

namespace MyCRM.Api.Controllers;

[ApiController]
[Route("api/clients")]
[Produces("application/json")]
[Authorize] 
public class ClientsController : ControllerBase
{
    private readonly IClientWebService _clientService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientWebService clientService, ILogger<ClientsController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDto>> GetById(int id, CancellationToken token = default)
    {
        var dto = await _clientService.GetClientByIdAsync(id, token);
        if (dto == null)
        {
            _logger.LogWarning("Client {Id} not found", id);
            return NotFound(new { message = $"Client with ID {id} not found" });
        }

        return Ok(dto);
    }
}