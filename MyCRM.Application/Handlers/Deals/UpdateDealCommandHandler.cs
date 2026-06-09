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
    public class UpdateDealCommandHandler : IRequestHandler<UpdateDealCommand, DealDto>
    {


    public readonly IDealService _dealService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateDealCommandHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    public UpdateDealCommandHandler(IDealService dealService, IMapper mapper, ILogger<UpdateDealCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _dealService = dealService;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
        public async Task<DealDto> Handle(UpdateDealCommand request, CancellationToken cancellationToken)
        {
                _logger.LogInformation("Обновление сделки ID={Id}", request.Id);

            if (!Enum.TryParse<DealStatus>(request.Status, true, out var status))
            {
                _logger.LogWarning("Некорректный статус при обновлении сделки: {Status}", request.Status);
                return null;
            }

            var oldDeal =  await _dealService.GetDealByIdAsync(request.Id, cancellationToken);


            if(status == DealStatus.Won && oldDeal != null && oldDeal.Status != DealStatus.Won)
            {
                await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var deal = await _unitOfWork.Deals.GetDealByIdAsync(request.Id, cancellationToken);
                        
                        deal.Name = request.Name;
                        deal.Amount = (int)request.Amount;
                        deal.Count = (int)request.Count;
                        deal.Status = status;
                        deal.Deadline = request.Deadline;
                        
                        await _unitOfWork.Deals.UpdateDealAsync(deal, cancellationToken);
                        
                        var task = new WorkTask(deal.Id, "Подписать документы", DateTime.UtcNow.AddDays(7));
                        await _unitOfWork.Tasks.AddTaskAsync(task, cancellationToken);
                        
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync();
                        
                        return _mapper.Map<DealDto>(deal);
                    }
                    catch
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        throw;
                    }
            }
            else
            {
                var dealUpdate = await _dealService.UpdateDealAsync(request.Id, request.Name, (int)request.Amount, (int)request.Count, status, request.Deadline, cancellationToken);

                if (dealUpdate == null)
                {
                    _logger.LogError("Ошибка при обновлении сделки ID={Id}", request.Id);
                    return null;
                }

                _logger.LogInformation("Сделка ID={Id} обновлена", request.Id);

                return _mapper.Map<DealDto>(dealUpdate);
            }
        }
    }
}