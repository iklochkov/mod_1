using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Application.DTOs
{
    public class DealCreateDto
{
    public string Name { get; set; }

    public decimal Amount { get; set; }

    public decimal Count { get; set; }

    public string Status { get; set; }
    public int ClientId { get; set; }

    public DateTime Deadline { get; set; }
}
}