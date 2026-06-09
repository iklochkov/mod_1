using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Constants;

namespace MyCRM.Domain.Entities
{
    public class KpiData
    {
        public DateTime Timestamp { get; set; }
        public int TotalClients { get; set; }
        public int TotalDeals { get; set; }
        public decimal TotalAmount { get; set; }
        public Dictionary<DealStatus, int> DealsByStatus { get; set; } = new();
    }
}