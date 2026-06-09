using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Commands.Clients
{
    public class UpdateClientCommand :IRequest<ClientDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        
    }
}