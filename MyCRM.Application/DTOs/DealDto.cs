using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Application.DTOs
{
public class DealDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal Count { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Deadline { get; set; }
}
}