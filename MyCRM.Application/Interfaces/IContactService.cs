using MyCRM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Entities;
using System.Threading.Tasks;

namespace MyCRM.Application.Interfaces
{
    public interface IContactService
    {
        Task<Contact?> AddContactAsync(string phone, string email, int clientId, CancellationToken token = default);
        Task<List<Contact>> GetContactsByClientIdAsync(int clientId, CancellationToken token = default);
    }
}