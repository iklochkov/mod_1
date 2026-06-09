using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Queries
{
    public class GetAllDealsQuery : IRequest<List<DealDto>>
    {
        
    }
}