using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Commands.Deals
{
    public class CreateDealCommand : IRequest<DealDto>
    {
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Count { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime Deadline { get; set; }
    public string UserId { get; set; } = string.Empty;

    }
}