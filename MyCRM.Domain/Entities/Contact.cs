using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Domain.Entities
{
    public record Contact
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public Contact(string phone, string email, int clientId)
        {
            Phone = phone;
            Email = email;
            ClientId = clientId;
        }
    }
}