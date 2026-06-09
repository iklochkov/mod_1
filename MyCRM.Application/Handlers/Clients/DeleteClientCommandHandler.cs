using MyCRM.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MyCRM.Application.Commands.Clients;
using MyCRM.Application.Interfaces;

namespace MyCRM.Application.Handlers.Clients
{
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
    {
    private readonly IClientService _clientService;
    private readonly ILogger<DeleteClientCommandHandler> _logger;
    public DeleteClientCommandHandler(IClientService clientService, ILogger<DeleteClientCommandHandler> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }
        public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
           _logger.LogInformation("Удаление клиента ID={Id}", request.Id);

            var result = await _clientService.DeleteClientAsync(request.Id, cancellationToken);

            if (!result)
            {
                _logger.LogWarning("Клиент ID={Id} не найден для удаления", request.Id);
            }
            else
            {
                _logger.LogInformation("Клиент ID={Id} удалён", request.Id);
            }
        
            return result;
        }
    }
}