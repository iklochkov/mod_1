using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Constants;

namespace MyCRM.Application.DTOs
{
public class DealUpdateDto
{
    public string Name { get; set; }
    public decimal Amount { get; set; }

    public decimal Count { get; set; }
    public string Status { get; set; }
    public DateTime Deadline { get; set; }
}
}