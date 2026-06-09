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
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientDto>
    {
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateClientCommandHandler> _logger;

     public UpdateClientCommandHandler(IClientService clientService, IMapper mapper, ILogger<UpdateClientCommandHandler> logger)
    {
        _clientService = clientService;
        _mapper = mapper;
        _logger = logger;
    }


        public async Task<ClientDto> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {

         _logger.LogInformation("Обновление клиента ID={Id}", request.Id);

         var client = await _clientService.UpdateClientAsync(request.Id, request.Name, request.Surname, request.Age, cancellationToken);

        if (client == null)
        {
            _logger.LogWarning("Клиент ID={Id} не найден для обновления", request.Id);
            return null;
        }

        _logger.LogInformation("Клиент ID={Id} обновлён", request.Id);
        return _mapper.Map<ClientDto>(client);
        }
    }
}