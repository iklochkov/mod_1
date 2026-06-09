using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MyCRM.Application.DTOs;

namespace MyCRM.Application.Commands.Deals
{
    public class UpdateDealCommand : IRequest<DealDto>
    {
    public int Id;
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal Count { get; set; }
    public string Status { get; set; }
    public DateTime Deadline { get; set; }
    }
}