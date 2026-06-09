using MyCRM.Application.Queries;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MyCRM.Application.DTOs;
using MyCRM.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Application.Handlers.Deals;

public class GetAllDealsQueryHandler : IRequestHandler<GetAllDealsQuery, List<DealDto>>
{
    private readonly IDealService _dealService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllDealsQueryHandler> _logger;

    public GetAllDealsQueryHandler(IDealService dealService, IMapper mapper, ILogger<GetAllDealsQueryHandler> logger)
    {
        _dealService = dealService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<DealDto>> Handle(GetAllDealsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Запрос всех сделок");
        
        var deals = await _dealService.GetAllDealsAsync(cancellationToken);
        
        _logger.LogInformation("Возвращено {Count} сделок", deals.Count);
        
        return _mapper.Map<List<DealDto>>(deals);
    }
}
