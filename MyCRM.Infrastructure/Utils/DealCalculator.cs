using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;

namespace MyCRM.Infrastructure.Utils
{
    public class DealCalculator
    {

        public static decimal CalculateTotalAmount(List<Deal> deals)
        {
            decimal total = 0;

            foreach (Deal dl in deals)
            {
                total += dl.Amount * dl.Count;
            }

            return total;
        }

    }
}