using MyCRM.Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using MyCRM.Domain.Entities;
using MyCRM.Application.Interfaces;
using MyCRM.Application.DTOs;
using MyCRM.Api.Services;
using MyCRM.Api.Services;

namespace MyCRM.Api.Services;

public class ClientWebService : IClientWebService
{
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;
    private readonly ILogger<ClientWebService> _logger;

    private readonly IJwtService _jwtService;

    public ClientWebService(IClientService clientService, IMapper mapper, ILogger<ClientWebService> logger, IJwtService jwtService )
    {
        _clientService = clientService;
        _mapper = mapper;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<ClientDto?> AddClientAsync(ClientCreateDto dto, string userId, CancellationToken token = default)
    {
        _logger.LogInformation("Создание клиента: {Name} {Surname}", dto.Name, dto.Surname);


        var client = await _clientService.AddClientAsync(dto.Name, dto.Surname, dto.Age, userId, token);

                Console.WriteLine($"=== DEBUG: userId из токена в сервисе web = '{userId}' ===");

        
        if (client == null)
        {
            _logger.LogWarning("Не удалось создать клиента {Name} {Surname}", dto.Name, dto.Surname);
            return null;
        }

        _logger.LogInformation("Клиент создан: ID={Id}, Имя={Name}", client.Id, client.Name);
        return _mapper.Map<ClientDto>(client);
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id, CancellationToken token = default)
    {
        _logger.LogDebug("Запрос клиента ID={Id}", id);

        
        var client = await _clientService.GetClientByIdAsync(id, token);
        
        
        if (client == null)
        {
            _logger.LogWarning("Клиент ID={Id} не найден", id);
            return null;
        }
        await _clientService.IncrementViewCountAsync(id, token);

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<PagedResponse<ClientDto>> GetPagedClientsAsync(
        int page, 
        int pageSize, 
        string? name = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        string sortBy = "Id",
        string sortOrder = "asc",
        CancellationToken token = default)
    {
        _logger.LogDebug(
            "GET clients: page={Page}, pageSize={PageSize}, name={Name}, sortBy={SortBy}, sortOrder={SortOrder}",
            page, pageSize, name, sortBy, sortOrder);

        var pagedResult = await _clientService.GetPagedClientsAsync(
            page, pageSize, name, createdFrom, createdTo, sortBy, sortOrder, token);
            
        var dtos = _mapper.Map<List<ClientDto>>(pagedResult.Items);
        _logger.LogInformation("Возвращено {Count} из {Total}", dtos.Count, pagedResult.TotalCount);

        return new PagedResponse<ClientDto>(dtos, page, pageSize, pagedResult.TotalCount);
    }

    public async Task<ClientDto?> UpdateClientAsync(int id, ClientUpdateDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("Обновление клиента ID={Id}", id);

        var client = await _clientService.UpdateClientAsync(id, dto.Name, dto.Surname, dto.Age, token);
        
        if (client == null)
        {
            _logger.LogWarning("Клиент ID={Id} не найден для обновления", id);
            return null;
        }

        _logger.LogInformation("Клиент ID={Id} обновлён", id);
        return _mapper.Map<ClientDto>(client);
    }

    public async Task<bool> DeleteClientAsync(int id, CancellationToken token = default)
    {
        _logger.LogInformation("Удаление клиента ID={Id}", id);

        var result = await _clientService.DeleteClientAsync(id, token);
        
        if (!result)
            _logger.LogWarning("Клиент ID={Id} не найден для удаления", id);
        else
            _logger.LogInformation("Клиент ID={Id} удалён", id);

        return result;
    }

    public async Task<ClientAvatarDto?> UploadAvatarAsync(int id, IFormFile file, CancellationToken token)
    {
    
      
        
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("=== ШАГ 2: Файл пустой ===");
            throw new ArgumentException("Файл не выбран");
        }

        if (file.Length > 2 * 1024 * 1024)
        {
            throw new ArgumentException("Файл слишком большой. Максимум 2 MB");
        }

        Console.WriteLine($"=== ContentType файла: '{file.ContentType}' ===");
        Console.WriteLine($"=== Имя файла: '{file.FileName}' ===");

         var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            throw new ArgumentException("Недопустимый тип файла. Разрешены: JPEG, PNG, GIF");
        }

        if(!await CheckFileSignatureAsync(file))
        {
            throw new ArgumentException("Файл не соответствует заявленному типу");

        }

        var client = await _clientService.GetClientByIdAsync(id, token);
        
        if (client == null)
        {
            
            return null;
        }

        var avatarsFolder = Path.Combine(Directory.GetCurrentDirectory(), "avatars");
        if (!Directory.Exists(avatarsFolder))
        {
            Directory.CreateDirectory(avatarsFolder);
        }

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{id}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(avatarsFolder, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        Console.WriteLine("=== ШАГ 8: Обновляем аватар в БД ===");
        await _clientService.UpdateAvatarAsync(id, fileName, token);

        return new ClientAvatarDto(id, filePath, file.Length, fileName);
    
    }

   private async Task<bool> CheckFileSignatureAsync(IFormFile file)
    {
    var header = new byte[16];
    await using var stream = file.OpenReadStream();
    var bytesRead = await stream.ReadAsync(header, 0, header.Length);
    
    if (bytesRead < 8) return false;

    // Проверка JPEG
    if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        return true;

    // Проверка PNG
    if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && 
        header[3] == 0x47 && header[4] == 0x0D && header[5] == 0x0A && 
        header[6] == 0x1A && header[7] == 0x0A)
        return true;

    // Проверка GIF
    if ((header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) &&
        (header[4] == 0x37 || header[4] == 0x39) && header[5] == 0x61)
        return true;

    return false;
    }
}