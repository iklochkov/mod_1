using MyCRM.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MyCRM.Application.Commands.Clients;
using MyCRM.Application.DTOs;
using MyCRM.Application.Interfaces;

namespace MyCRM.Application.Handlers.Clients
{
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDto>
    {
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateClientCommandHandler> _logger;

     public CreateClientCommandHandler(IClientService clientService, IMapper mapper,  ILogger<CreateClientCommandHandler> logger)
    {
        _clientService = clientService;
        _mapper = mapper;
        _logger = logger;
    }


        public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {

         _logger.LogInformation("Создание клиента: {Name} {Surname}", request.Name, request.Surname);

           var client = await _clientService.AddClientAsync(request.Name, request.Surname, request.Age, request.UserId, cancellationToken);

        if (client == null)
        {
            _logger.LogWarning("Не удалось создать клиента {Name} {Surname}", request.Name, request.Surname);
            return null;
        }

         _logger.LogInformation("Клиент создан: ID={Id}, Имя={Name}", client.Id, request.Name);

           return _mapper.Map<ClientDto>(client);
        }
    }
}