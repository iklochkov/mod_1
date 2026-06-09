using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Application.DTOs
{
    public class AnalyticsSummaryDto
    {
        public int TotalClients { get; set; }
        public int TotalDeals { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AvgDealAmount { get; set; }
        public int WonDeals { get; set; }
        public int LostDeals { get; set; }
    }

}