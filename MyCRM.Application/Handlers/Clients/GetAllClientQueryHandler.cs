using MyCRM.Application.Queries;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MyCRM.Application.DTOs;
using MyCRM.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Application.Handlers.Clients;

public class GetAllClientQueryHandler : IRequestHandler<GetAllClientQuery, PagedResponse<ClientDto>>
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllClientQueryHandler> _logger;

    public GetAllClientQueryHandler(IClientService clientService, IMapper mapper, ILogger<GetAllClientQueryHandler> logger)
    {
        _clientService = clientService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResponse<ClientDto>> Handle(GetAllClientQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("GET clients: page={Page}, pageSize={PageSize}, name={Name}, sortBy={SortBy}, sortOrder={SortOrder}",
            request.Page, request.PageSize, request.Name, request.SortBy, request.SortOrder);

        var pagedResult = await _clientService.GetPagedClientsAsync(
            request.Page, request.PageSize, request.Name, request.CreatedFrom, 
            request.CreatedTo, request.SortBy, request.SortOrder, cancellationToken);

        var dtos = _mapper.Map<List<ClientDto>>(pagedResult.Items);
        _logger.LogInformation("Возвращено {Count} из {Total}", dtos.Count, pagedResult.TotalCount);

        return new PagedResponse<ClientDto>(dtos, request.Page, request.PageSize, pagedResult.TotalCount);
    }
}
