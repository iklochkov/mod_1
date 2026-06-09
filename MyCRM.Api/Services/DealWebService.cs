using MyCRM.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MyCRM.Domain.Constants;
using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using MyCRM.Application.Interfaces;
using MyCRM.Application.DTOs;
using MyCRM.Api.Services;

namespace MyCRM.Api.Services.Impl;

public class DealWebService : IDealWebService
{
    private readonly IDealRepository _dealRepository;
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;
    private readonly ILogger<DealWebService> _logger;

    public DealWebService(IDealRepository dealRepository, IClientService clientService, IMapper mapper, ILogger<DealWebService> logger)
    {
        _dealRepository = dealRepository;
        _clientService = clientService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DealDto?> AddDealAsync(DealCreateDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("Создание сделки: {DealName}, Клиент ID={ClientId}", dto.Name, dto.ClientId);

        var client = await _clientService.GetClientByIdAsync(dto.ClientId, token);
        if (client == null)
        {
            _logger.LogWarning("Клиент ID={ClientId} не найден при создании сделки", dto.ClientId);
            return null;
        }

        if (!Enum.TryParse<DealStatus>(dto.Status, true, out var status))
        {
            _logger.LogWarning("Некорректный статус сделки: {Status}", dto.Status);
            return null;
        }

        var deal = new Deal(dto.Name, (int)dto.Amount, (int)dto.Count, status, dto.ClientId, dto.Deadline);
        var result = await _dealRepository.AddDealInRepoAsync(deal, token);
        
        if (!result)
        {
            _logger.LogError("Ошибка при сохранении сделки {DealName}", dto.Name);
            return null;
        }

        _logger.LogInformation("Сделка создана: ID={DealId}, Название={DealName}", deal.Id, deal.Name);
        return _mapper.Map<DealDto>(deal);
    }

    public async Task<DealDto?> AddDealAsync(DealCreateDto dto, string userId, CancellationToken token = default)
    {
        _logger.LogInformation("Создание сделки: {DealName}, Клиент ID={ClientId}", dto.Name, dto.ClientId);

        Console.WriteLine($"=== DealWebService userId: {userId} ===");

        var client = await _clientService.GetClientByIdAsync(dto.ClientId, token);
        if (client == null)
        {
            _logger.LogWarning("Клиент ID={ClientId} не найден при создании сделки", dto.ClientId);
            return null;
        }

        if (!Enum.TryParse<DealStatus>(dto.Status, true, out var status))
        {
            _logger.LogWarning("Некорректный статус сделки: {Status}", dto.Status);
            return null;
        }

        var deal = new Deal(dto.Name, (int)dto.Amount, (int)dto.Count, status, dto.ClientId, dto.Deadline,  userId);
        var result = await _dealRepository.AddDealInRepoAsync(deal, token);
        
        if (!result)
        {
            _logger.LogError("Ошибка при сохранении сделки {DealName}", dto.Name);
            return null;
        }

        _logger.LogInformation("Сделка создана: ID={DealId}, Название={DealName}", deal.Id, deal.Name);
        return _mapper.Map<DealDto>(deal);
    }

    public async Task<List<DealDto>> GetAllDealsAsync(CancellationToken token = default)
    {
        _logger.LogDebug("Запрос всех сделок");
        var deals = await _dealRepository.GetAllDealsAsync(token);
        _logger.LogInformation("Возвращено {Count} сделок", deals.Count);
        return _mapper.Map<List<DealDto>>(deals);
    }

    public async Task<DealDto?> GetDealByIdAsync(int id, CancellationToken token = default)
    {
        _logger.LogDebug("Запрос сделки ID={Id}", id);
        var deal = await _dealRepository.GetDealByIdAsync(id, token);
        
        if (deal == null)
            _logger.LogWarning("Сделка ID={Id} не найдена", id);

        return deal == null ? null : _mapper.Map<DealDto>(deal);
    }

    public async Task<DealDto?> GetDealWithDetailsAsync(int id, CancellationToken token = default)
    {
        _logger.LogDebug("Запрос сделки с деталями ID={Id}", id);
        var dtoFromRepo = await _dealRepository.GetDealWithDetailsAsync(id, token);
        
        if (dtoFromRepo == null)
            _logger.LogWarning("Сделка с деталями ID={Id} не найдена", id);

        return _mapper.Map<DealDto>(dtoFromRepo);
    }

    public async Task<DealDto?> UpdateDealAsync(int id, DealUpdateDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("Обновление сделки ID={Id}", id);

        if (!Enum.TryParse<DealStatus>(dto.Status, true, out var status))
        {
            _logger.LogWarning("Некорректный статус при обновлении сделки: {Status}", dto.Status);
            return null;
        }

        var existingDeal = await _dealRepository.GetDealByIdAsync(id, token);
        if (existingDeal == null)
        {
            _logger.LogWarning("Сделка ID={Id} не найдена для обновления", id);
            return null;
        }

        existingDeal.Name = dto.Name;
        existingDeal.Amount = dto.Amount;
        existingDeal.Count = dto.Count;
        existingDeal.Status = status;
        existingDeal.Deadline = dto.Deadline;

        var result = await _dealRepository.UpdateDealAsync(existingDeal, token);
        if (!result)
        {
            _logger.LogError("Ошибка при обновлении сделки ID={Id}", id);
            return null;
        }

        _logger.LogInformation("Сделка ID={Id} обновлена", id);
        return _mapper.Map<DealDto>(existingDeal);
    }

    public async Task<bool> DeleteDealAsync(int id, CancellationToken token = default)
    {
        _logger.LogInformation("Удаление сделки ID={Id}", id);
        var result = await _dealRepository.DeleteDealAsync(id, token);
        
        if (!result)
            _logger.LogWarning("Сделка ID={Id} не найдена для удаления", id);
        else
            _logger.LogInformation("Сделка ID={Id} удалена", id);

        return result;
    }
}