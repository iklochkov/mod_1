using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using MyCRM.Domain.Constants;

namespace MyCRM.Domain.Entities
{
    public class Deal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Count { get; set; }
        public DealStatus Status { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }
        public decimal Total { get; }
        public ICollection<WorkTask>? Tasks { get; set; } = new List<WorkTask>();
        public string? CreatedByUserId { get; set; }
        public virtual IdentityUser? CreatedBy { get; set; }


        public Deal() { }
        public Deal(string name, decimal amount, decimal count, DealStatus status, int clientId, DateTime deadline, string userId)
        {
            this.Name = name;
            this.Amount = amount;
            this.Count = count;
            this.Status = status;
            this.ClientId = clientId;
            this.CreatedAt = DateTime.Now;
            this.Deadline = deadline;
            this.Total = amount * count;
            CreatedByUserId = userId;
            Console.WriteLine($"=== Конструктор Deal: CreatedByUserId = '{this.CreatedByUserId}' ===");

        }

        public Deal(string name, decimal amount, decimal count, DealStatus status, int clientId, DateTime deadline)
        {
            this.Name = name;
            this.Amount = amount;
            this.Count = count;
            this.Status = status;
            this.ClientId = clientId;
            this.CreatedAt = DateTime.Now;
            this.Deadline = deadline;
            this.Total = amount * count;


        }
    }
}