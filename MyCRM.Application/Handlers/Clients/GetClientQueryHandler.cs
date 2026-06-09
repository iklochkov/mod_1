using MyCRM.Application.Queries;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MyCRM.Application.DTOs;
using MyCRM.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Application.Handlers.Clients;

public class GetClientQueryHandler : IRequestHandler<GetClientQuery, ClientDto?>
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClientQueryHandler> _logger;

    public GetClientQueryHandler(IClientService clientService, IMapper mapper, ILogger<GetClientQueryHandler> logger)
    {
        _clientService = clientService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ClientDto?> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Запрос клиента ID={Id}", request.Id);

        var client = await _clientService.GetClientByIdAsync(request.Id, cancellationToken);
        
        if (client == null)
        {
            _logger.LogWarning("Клиент ID={Id} не найден", request.Id);
            return null;
        }

        await _clientService.IncrementViewCountAsync(request.Id, cancellationToken);

        return _mapper.Map<ClientDto>(client);
    }
}
