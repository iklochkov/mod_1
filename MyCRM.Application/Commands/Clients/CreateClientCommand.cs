using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Commands.Clients
{
    public class CreateClientCommand : IRequest<ClientDto>
    {
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public int Age { get; set; }
    public string UserId { get; set; } = string.Empty;
    }
}