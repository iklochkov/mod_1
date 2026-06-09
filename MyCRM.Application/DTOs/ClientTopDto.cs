using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Application.DTOs
{
    public class ClientTopDto
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public decimal Amount { get; set; }

        public ClientTopDto(int id, string fullName, decimal amount)
        {
            this.Id = id;
            this.FullName = fullName;
            this.Amount = amount;
        }
    }
}