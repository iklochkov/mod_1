using MyCRM.Application.Queries;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MyCRM.Application.DTOs;
using MyCRM.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Application.Handlers.Deals;

public class GetDealQueryHandler : IRequestHandler<GetDealQuery, DealDto?>
{
    private readonly IDealService _dealService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDealQueryHandler> _logger;

    public GetDealQueryHandler(IDealService dealService, IMapper mapper, ILogger<GetDealQueryHandler> logger)
    {
        _dealService = dealService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DealDto?> Handle(GetDealQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Запрос сделки ID={Id}", request.Id);

        var deal = await _dealService.GetDealByIdAsync(request.Id, cancellationToken);
        
        if (deal == null)
        {
            _logger.LogWarning("Сделка ID={Id} не найдена", request.Id);
            return null;
        }

        return _mapper.Map<DealDto>(deal);
    }
}
