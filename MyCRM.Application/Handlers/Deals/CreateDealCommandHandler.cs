using MyCRM.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MyCRM.Application.Commands.Deals;
using MyCRM.Application.DTOs;
using MyCRM.Application.Interfaces;
using MyCRM.Domain.Constants;
using MyCRM.Domain.Entities;

namespace MyCRM.Application.Handlers.Deals
{
    public class CreateDealCommandHandler : IRequestHandler<CreateDealCommand, DealDto>
    {

    public readonly IDealService _dealService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateDealCommandHandler> _logger;

    public CreateDealCommandHandler(IDealService dealService, IMapper mapper, ILogger<CreateDealCommandHandler> logger)
    {
        _dealService = dealService;
        _mapper = mapper;
        _logger = logger;
    }

        public async Task<DealDto> Handle(CreateDealCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Создание сделки: {DealName}, Клиент ID={ClientId}", request.Name, request.ClientId);

        if (!Enum.TryParse<DealStatus>(request.Status, true, out var status))
        {
            _logger.LogWarning("Некорректный статус сделки: {Status}", request.Status);
            return null;
        }

       var deal = await _dealService.AddDealAsync(request.Name, (int)request.Amount, (int)request.Count, status, request.ClientId, 
       request.Deadline, request.UserId, cancellationToken);
        
        if(deal == null)
        {
            _logger.LogError("Ошибка при сохранении сделки {DealName}", request.Name);
              throw new InvalidOperationException("Не удалось создать сделку");
        }

          _logger.LogInformation("Сделка создана: ID={DealId}, Название={DealName}", deal.Id, deal.Name);

          return _mapper.Map<DealDto>(deal);

        }
    }
}