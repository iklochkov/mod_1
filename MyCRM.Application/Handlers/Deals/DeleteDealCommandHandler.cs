using MyCRM.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MyCRM.Application.Commands.Deals;
using MyCRM.Application.Interfaces;

namespace MyCRM.Application.Handlers.Deals
{
    public class DeleteDealCommandHandler : IRequestHandler<DeleteDealCommand, bool>
    {

     public readonly IDealService _dealService;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteDealCommandHandler> _logger;

    public DeleteDealCommandHandler(IDealService dealService, IMapper mapper, ILogger<DeleteDealCommandHandler> logger)
    {
        _dealService = dealService;
        _mapper = mapper;
        _logger = logger;
    }
        public async Task<bool> Handle(DeleteDealCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Удаление сделки ID={Id}", request.Id);

            var result = await _dealService.DeleteDealAsync(request.Id, cancellationToken);

            if (!result)
            {
                _logger.LogWarning("Сделка ID={Id} не найдена для удаления", request.Id);
            } else
            {
                _logger.LogInformation("Сделка ID={Id} удалена", request.Id);
            }

            return result;
        }
    }
}